using System;
using System.IO;
using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;

namespace SevenZip.Compression.LZMA
{
	public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
	{
		static Encoder()
		{
			int num = 2;
			Encoder.g_FastPos[0] = 0;
			Encoder.g_FastPos[1] = 1;
			for (byte b = 2; b < 22; b += 1)
			{
				uint num2 = 1U << (b >> 1) - 1;
				uint num3 = 0U;
				while (num3 < num2)
				{
					Encoder.g_FastPos[num] = b;
					num3 += 1U;
					num++;
				}
			}
		}

		private static uint GetPosSlot(uint pos)
		{
			if (pos < 2048U)
			{
				return (uint)Encoder.g_FastPos[(int)pos];
			}
			if (pos < 2097152U)
			{
				return (uint)(Encoder.g_FastPos[(int)(pos >> 10)] + 20);
			}
			return (uint)(Encoder.g_FastPos[(int)(pos >> 20)] + 40);
		}

		private static uint GetPosSlot2(uint pos)
		{
			if (pos < 131072U)
			{
				return (uint)(Encoder.g_FastPos[(int)(pos >> 6)] + 12);
			}
			if (pos < 134217728U)
			{
				return (uint)(Encoder.g_FastPos[(int)(pos >> 16)] + 32);
			}
			return (uint)(Encoder.g_FastPos[(int)(pos >> 26)] + 52);
		}

		private void BaseInit()
		{
			this._state.Init();
			this._previousByte = 0;
			for (uint num = 0U; num < 4U; num += 1U)
			{
				this._repDistances[(int)num] = 0U;
			}
		}

		private void Create()
		{
			if (this._matchFinder == null)
			{
				BinTree binTree = new BinTree();
				int type = 4;
				if (this._matchFinderType == Encoder.EMatchFinderType.BT2)
				{
					type = 2;
				}
				binTree.SetType(type);
				this._matchFinder = binTree;
			}
			this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);
			if (this._dictionarySize == this._dictionarySizePrev && this._numFastBytesPrev == this._numFastBytes)
			{
				return;
			}
			this._matchFinder.Create(this._dictionarySize, 4096U, this._numFastBytes, 274U);
			this._dictionarySizePrev = this._dictionarySize;
			this._numFastBytesPrev = this._numFastBytes;
		}

		public Encoder()
		{
			int num = 0;
			while ((long)num < 4096L)
			{
				this._optimum[num] = new Encoder.Optimal();
				num++;
			}
			int num2 = 0;
			while ((long)num2 < 4L)
			{
				this._posSlotEncoder[num2] = new BitTreeEncoder(6);
				num2++;
			}
		}

		private void SetWriteEndMarkerMode(bool writeEndMarker)
		{
			this._writeEndMark = writeEndMarker;
		}

		private void Init()
		{
			this.BaseInit();
			this._rangeEncoder.Init();
			for (uint num = 0U; num < 12U; num += 1U)
			{
				for (uint num2 = 0U; num2 <= this._posStateMask; num2 += 1U)
				{
					uint num3 = (num << 4) + num2;
					this._isMatch[(int)num3].Init();
					this._isRep0Long[(int)num3].Init();
				}
				this._isRep[(int)num].Init();
				this._isRepG0[(int)num].Init();
				this._isRepG1[(int)num].Init();
				this._isRepG2[(int)num].Init();
			}
			this._literalEncoder.Init();
			for (uint num = 0U; num < 4U; num += 1U)
			{
				this._posSlotEncoder[(int)num].Init();
			}
			for (uint num = 0U; num < 114U; num += 1U)
			{
				this._posEncoders[(int)num].Init();
			}
			this._lenEncoder.Init(1U << this._posStateBits);
			this._repMatchLenEncoder.Init(1U << this._posStateBits);
			this._posAlignEncoder.Init();
			this._longestMatchWasFound = false;
			this._optimumEndIndex = 0U;
			this._optimumCurrentIndex = 0U;
			this._additionalOffset = 0U;
		}

		private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
		{
			lenRes = 0U;
			numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
			if (numDistancePairs > 0U)
			{
				lenRes = this._matchDistances[(int)(numDistancePairs - 2U)];
				if (lenRes == this._numFastBytes)
				{
					lenRes += this._matchFinder.GetMatchLen((int)(lenRes - 1U), this._matchDistances[(int)(numDistancePairs - 1U)], 273U - lenRes);
				}
			}
			this._additionalOffset += 1U;
		}

		private void MovePos(uint num)
		{
			if (num > 0U)
			{
				this._matchFinder.Skip(num);
				this._additionalOffset += num;
			}
		}

		private uint GetRepLen1Price(Base.State state, uint posState)
		{
			return this._isRepG0[(int)state.Index].GetPrice0() + this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice0();
		}

		private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
		{
			uint num;
			if (repIndex == 0U)
			{
				num = this._isRepG0[(int)state.Index].GetPrice0();
				num += this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice1();
			}
			else
			{
				num = this._isRepG0[(int)state.Index].GetPrice1();
				if (repIndex == 1U)
				{
					num += this._isRepG1[(int)state.Index].GetPrice0();
				}
				else
				{
					num += this._isRepG1[(int)state.Index].GetPrice1();
					num += this._isRepG2[(int)state.Index].GetPrice(repIndex - 2U);
				}
			}
			return num;
		}

		private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
		{
			return this._repMatchLenEncoder.GetPrice(len - 2U, posState) + this.GetPureRepPrice(repIndex, state, posState);
		}

		private uint GetPosLenPrice(uint pos, uint len, uint posState)
		{
			uint lenToPosState = Base.GetLenToPosState(len);
			uint num;
			if (pos < 128U)
			{
				num = this._distancesPrices[(int)(lenToPosState * 128U + pos)];
			}
			else
			{
				num = this._posSlotPrices[(int)((lenToPosState << 6) + Encoder.GetPosSlot2(pos))] + this._alignPrices[(int)(pos & 15U)];
			}
			return num + this._lenEncoder.GetPrice(len - 2U, posState);
		}

		private uint Backward(out uint backRes, uint cur)
		{
			this._optimumEndIndex = cur;
			uint posPrev = this._optimum[(int)cur].PosPrev;
			uint backPrev = this._optimum[(int)cur].BackPrev;
			do
			{
				if (this._optimum[(int)cur].Prev1IsChar)
				{
					this._optimum[(int)posPrev].MakeAsChar();
					this._optimum[(int)posPrev].PosPrev = posPrev - 1U;
					if (this._optimum[(int)cur].Prev2)
					{
						this._optimum[(int)(posPrev - 1U)].Prev1IsChar = false;
						this._optimum[(int)(posPrev - 1U)].PosPrev = this._optimum[(int)cur].PosPrev2;
						this._optimum[(int)(posPrev - 1U)].BackPrev = this._optimum[(int)cur].BackPrev2;
					}
				}
				uint num = posPrev;
				uint backPrev2 = backPrev;
				backPrev = this._optimum[(int)num].BackPrev;
				posPrev = this._optimum[(int)num].PosPrev;
				this._optimum[(int)num].BackPrev = backPrev2;
				this._optimum[(int)num].PosPrev = cur;
				cur = num;
			}
			while (cur > 0U);
			backRes = this._optimum[0].BackPrev;
			this._optimumCurrentIndex = this._optimum[0].PosPrev;
			return this._optimumCurrentIndex;
		}

		private uint GetOptimum(uint position, out uint backRes)
		{
			if (this._optimumEndIndex != this._optimumCurrentIndex)
			{
				uint result = this._optimum[(int)this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
				backRes = this._optimum[(int)this._optimumCurrentIndex].BackPrev;
				this._optimumCurrentIndex = this._optimum[(int)this._optimumCurrentIndex].PosPrev;
				return result;
			}
			this._optimumCurrentIndex = (this._optimumEndIndex = 0U);
			uint longestMatchLength;
			uint num;
			if (!this._longestMatchWasFound)
			{
				this.ReadMatchDistances(out longestMatchLength, out num);
			}
			else
			{
				longestMatchLength = this._longestMatchLength;
				num = this._numDistancePairs;
				this._longestMatchWasFound = false;
			}
			uint num2 = this._matchFinder.GetNumAvailableBytes() + 1U;
			if (num2 < 2U)
			{
				backRes = uint.MaxValue;
				return 1U;
			}
			if (num2 > 273U)
			{
			}
			uint num3 = 0U;
			for (uint num4 = 0U; num4 < 4U; num4 += 1U)
			{
				this.reps[(int)num4] = this._repDistances[(int)num4];
				this.repLens[(int)num4] = this._matchFinder.GetMatchLen(-1, this.reps[(int)num4], 273U);
				if (this.repLens[(int)num4] > this.repLens[(int)num3])
				{
					num3 = num4;
				}
			}
			if (this.repLens[(int)num3] >= this._numFastBytes)
			{
				backRes = num3;
				uint num5 = this.repLens[(int)num3];
				this.MovePos(num5 - 1U);
				return num5;
			}
			if (longestMatchLength >= this._numFastBytes)
			{
				backRes = this._matchDistances[(int)(num - 1U)] + 4U;
				this.MovePos(longestMatchLength - 1U);
				return longestMatchLength;
			}
			byte indexByte = this._matchFinder.GetIndexByte(-1);
			byte indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - 1U));
			if (longestMatchLength < 2U && indexByte != indexByte2 && this.repLens[(int)num3] < 2U)
			{
				backRes = uint.MaxValue;
				return 1U;
			}
			this._optimum[0].State = this._state;
			uint num6 = position & this._posStateMask;
			this._optimum[1].Price = this._isMatch[(int)((this._state.Index << 4) + num6)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._previousByte).GetPrice(!this._state.IsCharState(), indexByte2, indexByte);
			this._optimum[1].MakeAsChar();
			uint num7 = this._isMatch[(int)((this._state.Index << 4) + num6)].GetPrice1();
			uint num8 = num7 + this._isRep[(int)this._state.Index].GetPrice1();
			if (indexByte2 == indexByte)
			{
				uint num9 = num8 + this.GetRepLen1Price(this._state, num6);
				if (num9 < this._optimum[1].Price)
				{
					this._optimum[1].Price = num9;
					this._optimum[1].MakeAsShortRep();
				}
			}
			uint num10 = (longestMatchLength >= this.repLens[(int)num3]) ? longestMatchLength : this.repLens[(int)num3];
			if (num10 < 2U)
			{
				backRes = this._optimum[1].BackPrev;
				return 1U;
			}
			this._optimum[1].PosPrev = 0U;
			this._optimum[0].Backs0 = this.reps[0];
			this._optimum[0].Backs1 = this.reps[1];
			this._optimum[0].Backs2 = this.reps[2];
			this._optimum[0].Backs3 = this.reps[3];
			uint num11 = num10;
			do
			{
				this._optimum[(int)num11--].Price = 268435455U;
			}
			while (num11 >= 2U);
			for (uint num4 = 0U; num4 < 4U; num4 += 1U)
			{
				uint num12 = this.repLens[(int)num4];
				if (num12 >= 2U)
				{
					uint num13 = num8 + this.GetPureRepPrice(num4, this._state, num6);
					do
					{
						uint num14 = num13 + this._repMatchLenEncoder.GetPrice(num12 - 2U, num6);
						Encoder.Optimal optimal = this._optimum[(int)num12];
						if (num14 < optimal.Price)
						{
							optimal.Price = num14;
							optimal.PosPrev = 0U;
							optimal.BackPrev = num4;
							optimal.Prev1IsChar = false;
						}
					}
					while ((num12 -= 1U) >= 2U);
				}
			}
			uint num15 = num7 + this._isRep[(int)this._state.Index].GetPrice0();
			num11 = ((this.repLens[0] >= 2U) ? (this.repLens[0] + 1U) : 2U);
			if (num11 <= longestMatchLength)
			{
				uint num16 = 0U;
				while (num11 > this._matchDistances[(int)num16])
				{
					num16 += 2U;
				}
				for (;;)
				{
					uint num17 = this._matchDistances[(int)(num16 + 1U)];
					uint num18 = num15 + this.GetPosLenPrice(num17, num11, num6);
					Encoder.Optimal optimal2 = this._optimum[(int)num11];
					if (num18 < optimal2.Price)
					{
						optimal2.Price = num18;
						optimal2.PosPrev = 0U;
						optimal2.BackPrev = num17 + 4U;
						optimal2.Prev1IsChar = false;
					}
					if (num11 == this._matchDistances[(int)num16])
					{
						num16 += 2U;
						if (num16 == num)
						{
							break;
						}
					}
					num11 += 1U;
				}
			}
			uint num19 = 0U;
			uint num20;
			for (;;)
			{
				num19 += 1U;
				if (num19 == num10)
				{
					break;
				}
				this.ReadMatchDistances(out num20, out num);
				if (num20 >= this._numFastBytes)
				{
					goto Block_24;
				}
				position += 1U;
				uint num21 = this._optimum[(int)num19].PosPrev;
				Base.State state;
				if (this._optimum[(int)num19].Prev1IsChar)
				{
					num21 -= 1U;
					if (this._optimum[(int)num19].Prev2)
					{
						state = this._optimum[(int)this._optimum[(int)num19].PosPrev2].State;
						if (this._optimum[(int)num19].BackPrev2 < 4U)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}
					else
					{
						state = this._optimum[(int)num21].State;
					}
					state.UpdateChar();
				}
				else
				{
					state = this._optimum[(int)num21].State;
				}
				if (num21 == num19 - 1U)
				{
					if (this._optimum[(int)num19].IsShortRep())
					{
						state.UpdateShortRep();
					}
					else
					{
						state.UpdateChar();
					}
				}
				else
				{
					uint num22;
					if (this._optimum[(int)num19].Prev1IsChar && this._optimum[(int)num19].Prev2)
					{
						num21 = this._optimum[(int)num19].PosPrev2;
						num22 = this._optimum[(int)num19].BackPrev2;
						state.UpdateRep();
					}
					else
					{
						num22 = this._optimum[(int)num19].BackPrev;
						if (num22 < 4U)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}
					Encoder.Optimal optimal3 = this._optimum[(int)num21];
					if (num22 < 4U)
					{
						if (num22 == 0U)
						{
							this.reps[0] = optimal3.Backs0;
							this.reps[1] = optimal3.Backs1;
							this.reps[2] = optimal3.Backs2;
							this.reps[3] = optimal3.Backs3;
						}
						else if (num22 == 1U)
						{
							this.reps[0] = optimal3.Backs1;
							this.reps[1] = optimal3.Backs0;
							this.reps[2] = optimal3.Backs2;
							this.reps[3] = optimal3.Backs3;
						}
						else if (num22 == 2U)
						{
							this.reps[0] = optimal3.Backs2;
							this.reps[1] = optimal3.Backs0;
							this.reps[2] = optimal3.Backs1;
							this.reps[3] = optimal3.Backs3;
						}
						else
						{
							this.reps[0] = optimal3.Backs3;
							this.reps[1] = optimal3.Backs0;
							this.reps[2] = optimal3.Backs1;
							this.reps[3] = optimal3.Backs2;
						}
					}
					else
					{
						this.reps[0] = num22 - 4U;
						this.reps[1] = optimal3.Backs0;
						this.reps[2] = optimal3.Backs1;
						this.reps[3] = optimal3.Backs2;
					}
				}
				this._optimum[(int)num19].State = state;
				this._optimum[(int)num19].Backs0 = this.reps[0];
				this._optimum[(int)num19].Backs1 = this.reps[1];
				this._optimum[(int)num19].Backs2 = this.reps[2];
				this._optimum[(int)num19].Backs3 = this.reps[3];
				uint price = this._optimum[(int)num19].Price;
				indexByte = this._matchFinder.GetIndexByte(-1);
				indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this.reps[0] - 1U - 1U));
				num6 = (position & this._posStateMask);
				uint num23 = price + this._isMatch[(int)((state.Index << 4) + num6)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._matchFinder.GetIndexByte(-2)).GetPrice(!state.IsCharState(), indexByte2, indexByte);
				Encoder.Optimal optimal4 = this._optimum[(int)(num19 + 1U)];
				bool flag = false;
				if (num23 < optimal4.Price)
				{
					optimal4.Price = num23;
					optimal4.PosPrev = num19;
					optimal4.MakeAsChar();
					flag = true;
				}
				num7 = price + this._isMatch[(int)((state.Index << 4) + num6)].GetPrice1();
				num8 = num7 + this._isRep[(int)state.Index].GetPrice1();
				if (indexByte2 == indexByte && (optimal4.PosPrev >= num19 || optimal4.BackPrev != 0U))
				{
					uint num24 = num8 + this.GetRepLen1Price(state, num6);
					if (num24 <= optimal4.Price)
					{
						optimal4.Price = num24;
						optimal4.PosPrev = num19;
						optimal4.MakeAsShortRep();
						flag = true;
					}
				}
				uint num25 = this._matchFinder.GetNumAvailableBytes() + 1U;
				num25 = Math.Min(4095U - num19, num25);
				num2 = num25;
				if (num2 >= 2U)
				{
					if (num2 > this._numFastBytes)
					{
						num2 = this._numFastBytes;
					}
					if (!flag && indexByte2 != indexByte)
					{
						uint limit = Math.Min(num25 - 1U, this._numFastBytes);
						uint matchLen = this._matchFinder.GetMatchLen(0, this.reps[0], limit);
						if (matchLen >= 2U)
						{
							Base.State state2 = state;
							state2.UpdateChar();
							uint num26 = position + 1U & this._posStateMask;
							uint num27 = num23 + this._isMatch[(int)((state2.Index << 4) + num26)].GetPrice1() + this._isRep[(int)state2.Index].GetPrice1();
							uint num28 = num19 + 1U + matchLen;
							while (num10 < num28)
							{
								this._optimum[(int)(num10 += 1U)].Price = 268435455U;
							}
							uint num29 = num27 + this.GetRepPrice(0U, matchLen, state2, num26);
							Encoder.Optimal optimal5 = this._optimum[(int)num28];
							if (num29 < optimal5.Price)
							{
								optimal5.Price = num29;
								optimal5.PosPrev = num19 + 1U;
								optimal5.BackPrev = 0U;
								optimal5.Prev1IsChar = true;
								optimal5.Prev2 = false;
							}
						}
					}
					uint num30 = 2U;
					for (uint num31 = 0U; num31 < 4U; num31 += 1U)
					{
						uint num32 = this._matchFinder.GetMatchLen(-1, this.reps[(int)num31], num2);
						if (num32 >= 2U)
						{
							uint num33 = num32;
							for (;;)
							{
								if (num10 >= num19 + num32)
								{
									uint num34 = num8 + this.GetRepPrice(num31, num32, state, num6);
									Encoder.Optimal optimal6 = this._optimum[(int)(num19 + num32)];
									if (num34 < optimal6.Price)
									{
										optimal6.Price = num34;
										optimal6.PosPrev = num19;
										optimal6.BackPrev = num31;
										optimal6.Prev1IsChar = false;
									}
									if ((num32 -= 1U) < 2U)
									{
										break;
									}
								}
								else
								{
									this._optimum[(int)(num10 += 1U)].Price = 268435455U;
								}
							}
							num32 = num33;
							if (num31 == 0U)
							{
								num30 = num32 + 1U;
							}
							if (num32 < num25)
							{
								uint limit2 = Math.Min(num25 - 1U - num32, this._numFastBytes);
								uint matchLen2 = this._matchFinder.GetMatchLen((int)num32, this.reps[(int)num31], limit2);
								if (matchLen2 >= 2U)
								{
									Base.State state3 = state;
									state3.UpdateRep();
									uint num35 = position + num32 & this._posStateMask;
									uint num36 = num8 + this.GetRepPrice(num31, num32, state, num6) + this._isMatch[(int)((state3.Index << 4) + num35)].GetPrice0() + this._literalEncoder.GetSubCoder(position + num32, this._matchFinder.GetIndexByte((int)(num32 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(num32 - 1U - (this.reps[(int)num31] + 1U))), this._matchFinder.GetIndexByte((int)(num32 - 1U)));
									state3.UpdateChar();
									num35 = (position + num32 + 1U & this._posStateMask);
									uint num37 = num36 + this._isMatch[(int)((state3.Index << 4) + num35)].GetPrice1() + this._isRep[(int)state3.Index].GetPrice1();
									uint num38 = num32 + 1U + matchLen2;
									while (num10 < num19 + num38)
									{
										this._optimum[(int)(num10 += 1U)].Price = 268435455U;
									}
									uint num39 = num37 + this.GetRepPrice(0U, matchLen2, state3, num35);
									Encoder.Optimal optimal7 = this._optimum[(int)(num19 + num38)];
									if (num39 < optimal7.Price)
									{
										optimal7.Price = num39;
										optimal7.PosPrev = num19 + num32 + 1U;
										optimal7.BackPrev = 0U;
										optimal7.Prev1IsChar = true;
										optimal7.Prev2 = true;
										optimal7.PosPrev2 = num19;
										optimal7.BackPrev2 = num31;
									}
								}
							}
						}
					}
					if (num20 > num2)
					{
						num20 = num2;
						num = 0U;
						while (num20 > this._matchDistances[(int)num])
						{
							num += 2U;
						}
						this._matchDistances[(int)num] = num20;
						num += 2U;
					}
					if (num20 >= num30)
					{
						num15 = num7 + this._isRep[(int)state.Index].GetPrice0();
						while (num10 < num19 + num20)
						{
							this._optimum[(int)(num10 += 1U)].Price = 268435455U;
						}
						uint num40 = 0U;
						while (num30 > this._matchDistances[(int)num40])
						{
							num40 += 2U;
						}
						uint num41 = num30;
						for (;;)
						{
							uint num42 = this._matchDistances[(int)(num40 + 1U)];
							uint num43 = num15 + this.GetPosLenPrice(num42, num41, num6);
							Encoder.Optimal optimal8 = this._optimum[(int)(num19 + num41)];
							if (num43 < optimal8.Price)
							{
								optimal8.Price = num43;
								optimal8.PosPrev = num19;
								optimal8.BackPrev = num42 + 4U;
								optimal8.Prev1IsChar = false;
							}
							if (num41 == this._matchDistances[(int)num40])
							{
								if (num41 < num25)
								{
									uint limit3 = Math.Min(num25 - 1U - num41, this._numFastBytes);
									uint matchLen3 = this._matchFinder.GetMatchLen((int)num41, num42, limit3);
									if (matchLen3 >= 2U)
									{
										Base.State state4 = state;
										state4.UpdateMatch();
										uint num44 = position + num41 & this._posStateMask;
										uint num45 = num43 + this._isMatch[(int)((state4.Index << 4) + num44)].GetPrice0() + this._literalEncoder.GetSubCoder(position + num41, this._matchFinder.GetIndexByte((int)(num41 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(num41 - (num42 + 1U) - 1U)), this._matchFinder.GetIndexByte((int)(num41 - 1U)));
										state4.UpdateChar();
										num44 = (position + num41 + 1U & this._posStateMask);
										uint num46 = num45 + this._isMatch[(int)((state4.Index << 4) + num44)].GetPrice1() + this._isRep[(int)state4.Index].GetPrice1();
										uint num47 = num41 + 1U + matchLen3;
										while (num10 < num19 + num47)
										{
											this._optimum[(int)(num10 += 1U)].Price = 268435455U;
										}
										num43 = num46 + this.GetRepPrice(0U, matchLen3, state4, num44);
										optimal8 = this._optimum[(int)(num19 + num47)];
										if (num43 < optimal8.Price)
										{
											optimal8.Price = num43;
											optimal8.PosPrev = num19 + num41 + 1U;
											optimal8.BackPrev = 0U;
											optimal8.Prev1IsChar = true;
											optimal8.Prev2 = true;
											optimal8.PosPrev2 = num19;
											optimal8.BackPrev2 = num42 + 4U;
										}
									}
								}
								num40 += 2U;
								if (num40 == num)
								{
									break;
								}
							}
							num41 += 1U;
						}
					}
				}
			}
			return this.Backward(out backRes, num19);
			Block_24:
			this._numDistancePairs = num;
			this._longestMatchLength = num20;
			this._longestMatchWasFound = true;
			return this.Backward(out backRes, num19);
		}

		private bool ChangePair(uint smallDist, uint bigDist)
		{
			return smallDist < 33554432U && bigDist >= smallDist << 7;
		}

		private void WriteEndMarker(uint posState)
		{
			if (!this._writeEndMark)
			{
				return;
			}
			this._isMatch[(int)((this._state.Index << 4) + posState)].Encode(this._rangeEncoder, 1U);
			this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
			this._state.UpdateMatch();
			uint num = 2U;
			this._lenEncoder.Encode(this._rangeEncoder, num - 2U, posState);
			uint symbol = 63U;
			uint lenToPosState = Base.GetLenToPosState(num);
			this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, symbol);
			int num2 = 30;
			uint num3 = (1U << num2) - 1U;
			this._rangeEncoder.EncodeDirectBits(num3 >> 4, num2 - 4);
			this._posAlignEncoder.ReverseEncode(this._rangeEncoder, num3 & 15U);
		}

		private void Flush(uint nowPos)
		{
			this.ReleaseMFStream();
			this.WriteEndMarker(nowPos & this._posStateMask);
			this._rangeEncoder.FlushData();
			this._rangeEncoder.FlushStream();
		}

		public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
		{
			inSize = 0L;
			outSize = 0L;
			finished = true;
			if (this._inStream != null)
			{
				this._matchFinder.SetStream(this._inStream);
				this._matchFinder.Init();
				this._needReleaseMFStream = true;
				this._inStream = null;
				if (this._trainSize > 0U)
				{
					this._matchFinder.Skip(this._trainSize);
				}
			}
			if (this._finished)
			{
				return;
			}
			this._finished = true;
			long num = this.nowPos64;
			if (this.nowPos64 == 0L)
			{
				if (this._matchFinder.GetNumAvailableBytes() == 0U)
				{
					this.Flush((uint)this.nowPos64);
					return;
				}
				uint num2;
				uint num3;
				this.ReadMatchDistances(out num2, out num3);
				uint num4 = (uint)this.nowPos64 & this._posStateMask;
				this._isMatch[(int)((this._state.Index << 4) + num4)].Encode(this._rangeEncoder, 0U);
				this._state.UpdateChar();
				byte indexByte = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
				this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte).Encode(this._rangeEncoder, indexByte);
				this._previousByte = indexByte;
				this._additionalOffset -= 1U;
				this.nowPos64 += 1L;
			}
			if (this._matchFinder.GetNumAvailableBytes() == 0U)
			{
				this.Flush((uint)this.nowPos64);
				return;
			}
			for (;;)
			{
				uint num5;
				uint optimum = this.GetOptimum((uint)this.nowPos64, out num5);
				uint num6 = (uint)this.nowPos64 & this._posStateMask;
				uint num7 = (this._state.Index << 4) + num6;
				if (optimum == 1U && num5 == 4294967295U)
				{
					this._isMatch[(int)num7].Encode(this._rangeEncoder, 0U);
					byte indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
					Encoder.LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte);
					if (!this._state.IsCharState())
					{
						byte indexByte3 = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - this._additionalOffset));
						subCoder.EncodeMatched(this._rangeEncoder, indexByte3, indexByte2);
					}
					else
					{
						subCoder.Encode(this._rangeEncoder, indexByte2);
					}
					this._previousByte = indexByte2;
					this._state.UpdateChar();
				}
				else
				{
					this._isMatch[(int)num7].Encode(this._rangeEncoder, 1U);
					if (num5 < 4U)
					{
						this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
						if (num5 == 0U)
						{
							this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
							if (optimum == 1U)
							{
								this._isRep0Long[(int)num7].Encode(this._rangeEncoder, 0U);
							}
							else
							{
								this._isRep0Long[(int)num7].Encode(this._rangeEncoder, 1U);
							}
						}
						else
						{
							this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
							if (num5 == 1U)
							{
								this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
							}
							else
							{
								this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
								this._isRepG2[(int)this._state.Index].Encode(this._rangeEncoder, num5 - 2U);
							}
						}
						if (optimum == 1U)
						{
							this._state.UpdateShortRep();
						}
						else
						{
							this._repMatchLenEncoder.Encode(this._rangeEncoder, optimum - 2U, num6);
							this._state.UpdateRep();
						}
						uint num8 = this._repDistances[(int)num5];
						if (num5 != 0U)
						{
							for (uint num9 = num5; num9 >= 1U; num9 -= 1U)
							{
								this._repDistances[(int)num9] = this._repDistances[(int)(num9 - 1U)];
							}
							this._repDistances[0] = num8;
						}
					}
					else
					{
						this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
						this._state.UpdateMatch();
						this._lenEncoder.Encode(this._rangeEncoder, optimum - 2U, num6);
						num5 -= 4U;
						uint posSlot = Encoder.GetPosSlot(num5);
						uint lenToPosState = Base.GetLenToPosState(optimum);
						this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, posSlot);
						if (posSlot >= 4U)
						{
							int num10 = (int)((posSlot >> 1) - 1U);
							uint num11 = (2U | (posSlot & 1U)) << num10;
							uint num12 = num5 - num11;
							if (posSlot < 14U)
							{
								BitTreeEncoder.ReverseEncode(this._posEncoders, num11 - posSlot - 1U, this._rangeEncoder, num10, num12);
							}
							else
							{
								this._rangeEncoder.EncodeDirectBits(num12 >> 4, num10 - 4);
								this._posAlignEncoder.ReverseEncode(this._rangeEncoder, num12 & 15U);
								this._alignPriceCount += 1U;
							}
						}
						uint num13 = num5;
						for (uint num14 = 3U; num14 >= 1U; num14 -= 1U)
						{
							this._repDistances[(int)num14] = this._repDistances[(int)(num14 - 1U)];
						}
						this._repDistances[0] = num13;
						this._matchPriceCount += 1U;
					}
					this._previousByte = this._matchFinder.GetIndexByte((int)(optimum - 1U - this._additionalOffset));
				}
				this._additionalOffset -= optimum;
				this.nowPos64 += (long)((ulong)optimum);
				if (this._additionalOffset == 0U)
				{
					if (this._matchPriceCount >= 128U)
					{
						this.FillDistancesPrices();
					}
					if (this._alignPriceCount >= 16U)
					{
						this.FillAlignPrices();
					}
					inSize = this.nowPos64;
					outSize = this._rangeEncoder.GetProcessedSizeAdd();
					if (this._matchFinder.GetNumAvailableBytes() == 0U)
					{
						break;
					}
					if (this.nowPos64 - num >= 4096L)
					{
						goto Block_24;
					}
				}
			}
			this.Flush((uint)this.nowPos64);
			return;
			Block_24:
			this._finished = false;
			finished = false;
		}

		private void ReleaseMFStream()
		{
			if (this._matchFinder != null && this._needReleaseMFStream)
			{
				this._matchFinder.ReleaseStream();
				this._needReleaseMFStream = false;
			}
		}

		private void SetOutStream(Stream outStream)
		{
			this._rangeEncoder.SetStream(outStream);
		}

		private void ReleaseOutStream()
		{
			this._rangeEncoder.ReleaseStream();
		}

		private void ReleaseStreams()
		{
			this.ReleaseMFStream();
			this.ReleaseOutStream();
		}

		private void SetStreams(Stream inStream, Stream outStream, long inSize, long outSize)
		{
			this._inStream = inStream;
			this._finished = false;
			this.Create();
			this.SetOutStream(outStream);
			this.Init();
			this.FillDistancesPrices();
			this.FillAlignPrices();
			this._lenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
			this._lenEncoder.UpdateTables(1U << this._posStateBits);
			this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
			this._repMatchLenEncoder.UpdateTables(1U << this._posStateBits);
			this.nowPos64 = 0L;
		}

		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			this._needReleaseMFStream = false;
			try
			{
				this.SetStreams(inStream, outStream, inSize, outSize);
				for (;;)
				{
					long inSize2;
					long outSize2;
					bool flag;
					this.CodeOneBlock(out inSize2, out outSize2, out flag);
					if (flag)
					{
						break;
					}
					if (progress != null)
					{
						progress.SetProgress(inSize2, outSize2);
					}
				}
			}
			finally
			{
				this.ReleaseStreams();
			}
		}

		public void WriteCoderProperties(Stream outStream)
		{
			this.properties[0] = (byte)((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9 + this._numLiteralContextBits);
			for (int i = 0; i < 4; i++)
			{
				this.properties[1 + i] = (byte)(this._dictionarySize >> 8 * i & 255U);
			}
			outStream.Write(this.properties, 0, 5);
		}

		private void FillDistancesPrices()
		{
			for (uint num = 4U; num < 128U; num += 1U)
			{
				uint posSlot = Encoder.GetPosSlot(num);
				int num2 = (int)((posSlot >> 1) - 1U);
				uint num3 = (2U | (posSlot & 1U)) << num2;
				this.tempPrices[(int)num] = BitTreeEncoder.ReverseGetPrice(this._posEncoders, num3 - posSlot - 1U, num2, num - num3);
			}
			for (uint num4 = 0U; num4 < 4U; num4 += 1U)
			{
				BitTreeEncoder bitTreeEncoder = this._posSlotEncoder[(int)num4];
				uint num5 = num4 << 6;
				for (uint num6 = 0U; num6 < this._distTableSize; num6 += 1U)
				{
					this._posSlotPrices[(int)(num5 + num6)] = bitTreeEncoder.GetPrice(num6);
				}
				for (uint num6 = 14U; num6 < this._distTableSize; num6 += 1U)
				{
					this._posSlotPrices[(int)(num5 + num6)] += (num6 >> 1) - 1U - 4U << 6;
				}
				uint num7 = num4 * 128U;
				uint num8;
				for (num8 = 0U; num8 < 4U; num8 += 1U)
				{
					this._distancesPrices[(int)(num7 + num8)] = this._posSlotPrices[(int)(num5 + num8)];
				}
				while (num8 < 128U)
				{
					this._distancesPrices[(int)(num7 + num8)] = this._posSlotPrices[(int)(num5 + Encoder.GetPosSlot(num8))] + this.tempPrices[(int)num8];
					num8 += 1U;
				}
			}
			this._matchPriceCount = 0U;
		}

		private void FillAlignPrices()
		{
			for (uint num = 0U; num < 16U; num += 1U)
			{
				this._alignPrices[(int)num] = this._posAlignEncoder.ReverseGetPrice(num);
			}
			this._alignPriceCount = 0U;
		}

		private static int FindMatchFinder(string s)
		{
			for (int i = 0; i < Encoder.kMatchFinderIDs.Length; i++)
			{
				if (s == Encoder.kMatchFinderIDs[i])
				{
					return i;
				}
			}
			return -1;
		}

		public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
		{
			uint num = 0U;
			while ((ulong)num < (ulong)((long)properties.Length))
			{
				object obj = properties[(int)num];
				switch (propIDs[(int)num])
				{
				case CoderPropID.DictionarySize:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num2 = (int)obj;
					if ((long)num2 < 1L || (long)num2 > 1073741824L)
					{
						throw new InvalidParamException();
					}
					this._dictionarySize = (uint)num2;
					int num3 = 0;
					while ((long)num3 < 30L && (long)num2 > (long)(1UL << (num3 & 31)))
					{
						num3++;
					}
					this._distTableSize = (uint)(num3 * 2);
					break;
				}
				case CoderPropID.UsedMemorySize:
				case CoderPropID.Order:
				case CoderPropID.BlockSize:
				case CoderPropID.MatchFinderCycles:
				case CoderPropID.NumPasses:
				case CoderPropID.NumThreads:
					goto IL_21C;
				case CoderPropID.PosStateBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num4 = (int)obj;
					if (num4 < 0 || (long)num4 > 4L)
					{
						throw new InvalidParamException();
					}
					this._posStateBits = num4;
					this._posStateMask = (1U << this._posStateBits) - 1U;
					break;
				}
				case CoderPropID.LitContextBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num5 = (int)obj;
					if (num5 < 0 || (long)num5 > 8L)
					{
						throw new InvalidParamException();
					}
					this._numLiteralContextBits = num5;
					break;
				}
				case CoderPropID.LitPosBits:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num6 = (int)obj;
					if (num6 < 0 || (long)num6 > 4L)
					{
						throw new InvalidParamException();
					}
					this._numLiteralPosStateBits = num6;
					break;
				}
				case CoderPropID.NumFastBytes:
				{
					if (!(obj is int))
					{
						throw new InvalidParamException();
					}
					int num7 = (int)obj;
					if (num7 < 5 || (long)num7 > 273L)
					{
						throw new InvalidParamException();
					}
					this._numFastBytes = (uint)num7;
					break;
				}
				case CoderPropID.MatchFinder:
				{
					if (!(obj is string))
					{
						throw new InvalidParamException();
					}
					Encoder.EMatchFinderType matchFinderType = this._matchFinderType;
					int num8 = Encoder.FindMatchFinder(((string)obj).ToUpper());
					if (num8 < 0)
					{
						throw new InvalidParamException();
					}
					this._matchFinderType = (Encoder.EMatchFinderType)num8;
					if (this._matchFinder != null && matchFinderType != this._matchFinderType)
					{
						this._dictionarySizePrev = uint.MaxValue;
						this._matchFinder = null;
					}
					break;
				}
				case CoderPropID.Algorithm:
					break;
				case CoderPropID.EndMarker:
					if (!(obj is bool))
					{
						throw new InvalidParamException();
					}
					this.SetWriteEndMarkerMode((bool)obj);
					break;
				default:
					goto IL_21C;
				}
				num += 1U;
				continue;
				IL_21C:
				throw new InvalidParamException();
			}
		}

		public void SetTrainSize(uint trainSize)
		{
			this._trainSize = trainSize;
		}

		private const uint kIfinityPrice = 268435455U;

		private static byte[] g_FastPos = new byte[2048];

		private Base.State _state;

		private byte _previousByte;

		private uint[] _repDistances = new uint[4];

		private const int kDefaultDictionaryLogSize = 22;

		private const uint kNumFastBytesDefault = 32U;

		private const uint kNumLenSpecSymbols = 16U;

		private const uint kNumOpts = 4096U;

		private Encoder.Optimal[] _optimum = new Encoder.Optimal[4096];

		private IMatchFinder _matchFinder;

		private SevenZip.Compression.RangeCoder.Encoder _rangeEncoder = new SevenZip.Compression.RangeCoder.Encoder();

		private BitEncoder[] _isMatch = new BitEncoder[192];

		private BitEncoder[] _isRep = new BitEncoder[12];

		private BitEncoder[] _isRepG0 = new BitEncoder[12];

		private BitEncoder[] _isRepG1 = new BitEncoder[12];

		private BitEncoder[] _isRepG2 = new BitEncoder[12];

		private BitEncoder[] _isRep0Long = new BitEncoder[192];

		private BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[4];

		private BitEncoder[] _posEncoders = new BitEncoder[114];

		private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(4);

		private Encoder.LenPriceTableEncoder _lenEncoder = new Encoder.LenPriceTableEncoder();

		private Encoder.LenPriceTableEncoder _repMatchLenEncoder = new Encoder.LenPriceTableEncoder();

		private Encoder.LiteralEncoder _literalEncoder = new Encoder.LiteralEncoder();

		private uint[] _matchDistances = new uint[548];

		private uint _numFastBytes = 32U;

		private uint _longestMatchLength;

		private uint _numDistancePairs;

		private uint _additionalOffset;

		private uint _optimumEndIndex;

		private uint _optimumCurrentIndex;

		private bool _longestMatchWasFound;

		private uint[] _posSlotPrices = new uint[256];

		private uint[] _distancesPrices = new uint[512];

		private uint[] _alignPrices = new uint[16];

		private uint _alignPriceCount;

		private uint _distTableSize = 44U;

		private int _posStateBits = 2;

		private uint _posStateMask = 3U;

		private int _numLiteralPosStateBits;

		private int _numLiteralContextBits = 3;

		private uint _dictionarySize = 4194304U;

		private uint _dictionarySizePrev = uint.MaxValue;

		private uint _numFastBytesPrev = uint.MaxValue;

		private long nowPos64;

		private bool _finished;

		private Stream _inStream;

		private Encoder.EMatchFinderType _matchFinderType = Encoder.EMatchFinderType.BT4;

		private bool _writeEndMark;

		private bool _needReleaseMFStream;

		private uint[] reps = new uint[4];

		private uint[] repLens = new uint[4];

		private const int kPropSize = 5;

		private byte[] properties = new byte[5];

		private uint[] tempPrices = new uint[128];

		private uint _matchPriceCount;

		private static string[] kMatchFinderIDs = new string[]
		{
			"BT2",
			"BT4"
		};

		private uint _trainSize;

		private enum EMatchFinderType
		{
			BT2,
			BT4
		}

		private class LiteralEncoder
		{
			public void Create(int numPosBits, int numPrevBits)
			{
				if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
				{
					return;
				}
				this.m_NumPosBits = numPosBits;
				this.m_PosMask = (1U << numPosBits) - 1U;
				this.m_NumPrevBits = numPrevBits;
				uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
				this.m_Coders = new Encoder.LiteralEncoder.Encoder2[num];
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					this.m_Coders[(int)num2].Create();
				}
			}

			public void Init()
			{
				uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					this.m_Coders[(int)num2].Init();
				}
			}

			public Encoder.LiteralEncoder.Encoder2 GetSubCoder(uint pos, byte prevByte)
			{
				return this.m_Coders[(int)(((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits))];
			}

			private Encoder.LiteralEncoder.Encoder2[] m_Coders;

			private int m_NumPrevBits;

			private int m_NumPosBits;

			private uint m_PosMask;

			public struct Encoder2
			{
				public void Create()
				{
					this.m_Encoders = new BitEncoder[768];
				}

				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						this.m_Encoders[i].Init();
					}
				}

				public void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, byte symbol)
				{
					uint num = 1U;
					for (int i = 7; i >= 0; i--)
					{
						uint num2 = (uint)(symbol >> i & 1);
						this.m_Encoders[(int)num].Encode(rangeEncoder, num2);
						num = (num << 1 | num2);
					}
				}

				public void EncodeMatched(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
				{
					uint num = 1U;
					bool flag = true;
					for (int i = 7; i >= 0; i--)
					{
						uint num2 = (uint)(symbol >> i & 1);
						uint num3 = num;
						if (flag)
						{
							uint num4 = (uint)(matchByte >> i & 1);
							num3 += 1U + num4 << 8;
							flag = (num4 == num2);
						}
						this.m_Encoders[(int)num3].Encode(rangeEncoder, num2);
						num = (num << 1 | num2);
					}
				}

				public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
				{
					uint num = 0U;
					uint num2 = 1U;
					int i = 7;
					if (matchMode)
					{
						while (i >= 0)
						{
							uint num3 = (uint)(matchByte >> i & 1);
							uint num4 = (uint)(symbol >> i & 1);
							num += this.m_Encoders[(int)((1U + num3 << 8) + num2)].GetPrice(num4);
							num2 = (num2 << 1 | num4);
							if (num3 != num4)
							{
								i--;
								break;
							}
							i--;
						}
					}
					while (i >= 0)
					{
						uint num5 = (uint)(symbol >> i & 1);
						num += this.m_Encoders[(int)num2].GetPrice(num5);
						num2 = (num2 << 1 | num5);
						i--;
					}
					return num;
				}

				private BitEncoder[] m_Encoders;
			}
		}

		private class LenEncoder
		{
			public LenEncoder()
			{
				for (uint num = 0U; num < 16U; num += 1U)
				{
					this._lowCoder[(int)num] = new BitTreeEncoder(3);
					this._midCoder[(int)num] = new BitTreeEncoder(3);
				}
			}

			public void Init(uint numPosStates)
			{
				this._choice.Init();
				this._choice2.Init();
				for (uint num = 0U; num < numPosStates; num += 1U)
				{
					this._lowCoder[(int)num].Init();
					this._midCoder[(int)num].Init();
				}
				this._highCoder.Init();
			}

			public void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				if (symbol < 8U)
				{
					this._choice.Encode(rangeEncoder, 0U);
					this._lowCoder[(int)posState].Encode(rangeEncoder, symbol);
					return;
				}
				symbol -= 8U;
				this._choice.Encode(rangeEncoder, 1U);
				if (symbol < 8U)
				{
					this._choice2.Encode(rangeEncoder, 0U);
					this._midCoder[(int)posState].Encode(rangeEncoder, symbol);
					return;
				}
				this._choice2.Encode(rangeEncoder, 1U);
				this._highCoder.Encode(rangeEncoder, symbol - 8U);
			}

			public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
			{
				uint price = this._choice.GetPrice0();
				uint price2 = this._choice.GetPrice1();
				uint num = price2 + this._choice2.GetPrice0();
				uint num2 = price2 + this._choice2.GetPrice1();
				uint num3;
				for (num3 = 0U; num3 < 8U; num3 += 1U)
				{
					if (num3 >= numSymbols)
					{
						return;
					}
					prices[(int)(st + num3)] = price + this._lowCoder[(int)posState].GetPrice(num3);
				}
				while (num3 < 16U)
				{
					if (num3 >= numSymbols)
					{
						return;
					}
					prices[(int)(st + num3)] = num + this._midCoder[(int)posState].GetPrice(num3 - 8U);
					num3 += 1U;
				}
				while (num3 < numSymbols)
				{
					prices[(int)(st + num3)] = num2 + this._highCoder.GetPrice(num3 - 8U - 8U);
					num3 += 1U;
				}
			}

			private BitEncoder _choice;

			private BitEncoder _choice2;

			private BitTreeEncoder[] _lowCoder = new BitTreeEncoder[16];

			private BitTreeEncoder[] _midCoder = new BitTreeEncoder[16];

			private BitTreeEncoder _highCoder = new BitTreeEncoder(8);
		}

		private class LenPriceTableEncoder : Encoder.LenEncoder
		{
			public void SetTableSize(uint tableSize)
			{
				this._tableSize = tableSize;
			}

			public uint GetPrice(uint symbol, uint posState)
			{
				return this._prices[(int)(posState * 272U + symbol)];
			}

			private void UpdateTable(uint posState)
			{
				base.SetPrices(posState, this._tableSize, this._prices, posState * 272U);
				this._counters[(int)posState] = this._tableSize;
			}

			public void UpdateTables(uint numPosStates)
			{
				for (uint num = 0U; num < numPosStates; num += 1U)
				{
					this.UpdateTable(num);
				}
			}

			public new void Encode(SevenZip.Compression.RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				base.Encode(rangeEncoder, symbol, posState);
				uint[] counters = this._counters;
				uint num = counters[(int)posState] - 1U;
				counters[(int)posState] = num;
				if (num == 0U)
				{
					this.UpdateTable(posState);
				}
			}

			private uint[] _prices = new uint[4352];

			private uint _tableSize;

			private uint[] _counters = new uint[16];
		}

		private class Optimal
		{
			public void MakeAsChar()
			{
				this.BackPrev = uint.MaxValue;
				this.Prev1IsChar = false;
			}

			public void MakeAsShortRep()
			{
				this.BackPrev = 0U;
				this.Prev1IsChar = false;
			}

			public bool IsShortRep()
			{
				return this.BackPrev == 0U;
			}

			public Base.State State;

			public bool Prev1IsChar;

			public bool Prev2;

			public uint PosPrev2;

			public uint BackPrev2;

			public uint Price;

			public uint PosPrev;

			public uint BackPrev;

			public uint Backs0;

			public uint Backs1;

			public uint Backs2;

			public uint Backs3;
		}
	}
}
