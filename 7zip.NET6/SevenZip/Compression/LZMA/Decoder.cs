using System;
using System.IO;
using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;
using Decoder = SevenZip.Compression.RangeCoder.Decoder;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x0200001F RID: 31
	public class Decoder : ICoder, ISetDecoderProperties
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00006528 File Offset: 0x00004728
		public Decoder()
		{
			this.m_DictionarySize = uint.MaxValue;
			int num = 0;
			while ((long)num < 4L)
			{
				this.m_PosSlotDecoder[num] = new BitTreeDecoder(6);
				num++;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00006614 File Offset: 0x00004814
		private void SetDictionarySize(uint dictionarySize)
		{
			if (this.m_DictionarySize != dictionarySize)
			{
				this.m_DictionarySize = dictionarySize;
				this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1U);
				uint windowSize = Math.Max(this.m_DictionarySizeCheck, 4096U);
				this.m_OutWindow.Create(windowSize);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00006660 File Offset: 0x00004860
		private void SetLiteralProperties(int lp, int lc)
		{
			if (lp > 8)
			{
				throw new InvalidParamException();
			}
			if (lc > 8)
			{
				throw new InvalidParamException();
			}
			this.m_LiteralDecoder.Create(lp, lc);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00006684 File Offset: 0x00004884
		private void SetPosBitsProperties(int pb)
		{
			if (pb > 4)
			{
				throw new InvalidParamException();
			}
			uint num = 1U << pb;
			this.m_LenDecoder.Create(num);
			this.m_RepLenDecoder.Create(num);
			this.m_PosStateMask = num - 1U;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000066C4 File Offset: 0x000048C4
		private void Init(Stream inStream, Stream outStream)
		{
			this.m_RangeDecoder.Init(inStream);
			this.m_OutWindow.Init(outStream, this._solid);
			for (uint num = 0U; num < 12U; num += 1U)
			{
				for (uint num2 = 0U; num2 <= this.m_PosStateMask; num2 += 1U)
				{
					uint num3 = (num << 4) + num2;
					this.m_IsMatchDecoders[(int)num3].Init();
					this.m_IsRep0LongDecoders[(int)num3].Init();
				}
				this.m_IsRepDecoders[(int)num].Init();
				this.m_IsRepG0Decoders[(int)num].Init();
				this.m_IsRepG1Decoders[(int)num].Init();
				this.m_IsRepG2Decoders[(int)num].Init();
			}
			this.m_LiteralDecoder.Init();
			for (uint num = 0U; num < 4U; num += 1U)
			{
				this.m_PosSlotDecoder[(int)num].Init();
			}
			for (uint num = 0U; num < 114U; num += 1U)
			{
				this.m_PosDecoders[(int)num].Init();
			}
			this.m_LenDecoder.Init();
			this.m_RepLenDecoder.Init();
			this.m_PosAlignDecoder.Init();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000067E8 File Offset: 0x000049E8
		public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
		{
			this.Init(inStream, outStream);
			Base.State state = default(Base.State);
			state.Init();
			uint num = 0U;
			uint num2 = 0U;
			uint num3 = 0U;
			uint num4 = 0U;
			ulong num5 = 0UL;
			if (num5 < (ulong)outSize)
			{
				if (this.m_IsMatchDecoders[(int)((int)state.Index << 4)].Decode(this.m_RangeDecoder) != 0U)
				{
					throw new DataErrorException();
				}
				state.UpdateChar();
				byte b = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0U, 0);
				this.m_OutWindow.PutByte(b);
				num5 += 1UL;
			}
			while (num5 < (ulong)outSize)
			{
				uint num6 = (uint)num5 & this.m_PosStateMask;
				if (this.m_IsMatchDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
				{
					byte @byte = this.m_OutWindow.GetByte(0U);
					byte b2;
					if (!state.IsCharState())
					{
						b2 = this.m_LiteralDecoder.DecodeWithMatchByte(this.m_RangeDecoder, (uint)num5, @byte, this.m_OutWindow.GetByte(num));
					}
					else
					{
						b2 = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, (uint)num5, @byte);
					}
					this.m_OutWindow.PutByte(b2);
					state.UpdateChar();
					num5 += 1UL;
				}
				else
				{
					uint num8;
					if (this.m_IsRepDecoders[(int)state.Index].Decode(this.m_RangeDecoder) == 1U)
					{
						if (this.m_IsRepG0Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
						{
							if (this.m_IsRep0LongDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
							{
								state.UpdateShortRep();
								this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(num));
								num5 += 1UL;
								continue;
							}
						}
						else
						{
							uint num7;
							if (this.m_IsRepG1Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
							{
								num7 = num2;
							}
							else
							{
								if (this.m_IsRepG2Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
								{
									num7 = num3;
								}
								else
								{
									num7 = num4;
									num4 = num3;
								}
								num3 = num2;
							}
							num2 = num;
							num = num7;
						}
						num8 = this.m_RepLenDecoder.Decode(this.m_RangeDecoder, num6) + 2U;
						state.UpdateRep();
					}
					else
					{
						num4 = num3;
						num3 = num2;
						num2 = num;
						num8 = 2U + this.m_LenDecoder.Decode(this.m_RangeDecoder, num6);
						state.UpdateMatch();
						uint num9 = this.m_PosSlotDecoder[(int)Base.GetLenToPosState(num8)].Decode(this.m_RangeDecoder);
						if (num9 >= 4U)
						{
							int num10 = (int)((num9 >> 1) - 1U);
							num = (2U | (num9 & 1U)) << num10;
							if (num9 < 14U)
							{
								num += BitTreeDecoder.ReverseDecode(this.m_PosDecoders, num - num9 - 1U, this.m_RangeDecoder, num10);
							}
							else
							{
								num += this.m_RangeDecoder.DecodeDirectBits(num10 - 4) << 4;
								num += this.m_PosAlignDecoder.ReverseDecode(this.m_RangeDecoder);
							}
						}
						else
						{
							num = num9;
						}
					}
					if ((ulong)num >= (ulong)this.m_OutWindow.TrainSize + num5 || num >= this.m_DictionarySizeCheck)
					{
						if (num != 4294967295U)
						{
							throw new DataErrorException();
						}
						break;
					}
					else
					{
						this.m_OutWindow.CopyBlock(num, num8);
						num5 += (ulong)num8;
					}
				}
			}
			this.m_OutWindow.Flush();
			this.m_OutWindow.ReleaseStream();
			this.m_RangeDecoder.ReleaseStream();
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006B40 File Offset: 0x00004D40
		public void SetDecoderProperties(byte[] properties)
		{
			if (properties.Length < 5)
			{
				throw new InvalidParamException();
			}
			int lc = (int)(properties[0] % 9);
			byte b = (byte)(properties[0] / 9);
			int lp = (int)(b % 5);
			int num = (int)(b / 5);
			if (num > 4)
			{
				throw new InvalidParamException();
			}
			uint num2 = 0U;
			for (int i = 0; i < 4; i++)
			{
				num2 += (uint)((uint)properties[1 + i] << i * 8);
			}
			this.SetDictionarySize(num2);
			this.SetLiteralProperties(lp, lc);
			this.SetPosBitsProperties(num);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006BB0 File Offset: 0x00004DB0
		public bool Train(Stream stream)
		{
			this._solid = true;
			return this.m_OutWindow.Train(stream);
		}

		// Token: 0x040000B0 RID: 176
		private OutWindow m_OutWindow = new OutWindow();

		// Token: 0x040000B1 RID: 177
		private SevenZip.Compression.RangeCoder.Decoder m_RangeDecoder = new SevenZip.Compression.RangeCoder.Decoder();

		// Token: 0x040000B2 RID: 178
		private BitDecoder[] m_IsMatchDecoders = new BitDecoder[192];

		// Token: 0x040000B3 RID: 179
		private BitDecoder[] m_IsRepDecoders = new BitDecoder[12];

		// Token: 0x040000B4 RID: 180
		private BitDecoder[] m_IsRepG0Decoders = new BitDecoder[12];

		// Token: 0x040000B5 RID: 181
		private BitDecoder[] m_IsRepG1Decoders = new BitDecoder[12];

		// Token: 0x040000B6 RID: 182
		private BitDecoder[] m_IsRepG2Decoders = new BitDecoder[12];

		// Token: 0x040000B7 RID: 183
		private BitDecoder[] m_IsRep0LongDecoders = new BitDecoder[192];

		// Token: 0x040000B8 RID: 184
		private BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[4];

		// Token: 0x040000B9 RID: 185
		private BitDecoder[] m_PosDecoders = new BitDecoder[114];

		// Token: 0x040000BA RID: 186
		private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(4);

		// Token: 0x040000BB RID: 187
		private Decoder.LenDecoder m_LenDecoder = new Decoder.LenDecoder();

		// Token: 0x040000BC RID: 188
		private Decoder.LenDecoder m_RepLenDecoder = new Decoder.LenDecoder();

		// Token: 0x040000BD RID: 189
		private Decoder.LiteralDecoder m_LiteralDecoder = new Decoder.LiteralDecoder();

		// Token: 0x040000BE RID: 190
		private uint m_DictionarySize;

		// Token: 0x040000BF RID: 191
		private uint m_DictionarySizeCheck;

		// Token: 0x040000C0 RID: 192
		private uint m_PosStateMask;

		// Token: 0x040000C1 RID: 193
		private bool _solid;

		// Token: 0x0200002A RID: 42
		private class LenDecoder
		{
			// Token: 0x060000FE RID: 254 RVA: 0x000097C0 File Offset: 0x000079C0
			public void Create(uint numPosStates)
			{
				for (uint num = this.m_NumPosStates; num < numPosStates; num += 1U)
				{
					this.m_LowCoder[(int)num] = new BitTreeDecoder(3);
					this.m_MidCoder[(int)num] = new BitTreeDecoder(3);
				}
				this.m_NumPosStates = numPosStates;
			}

			// Token: 0x060000FF RID: 255 RVA: 0x0000980C File Offset: 0x00007A0C
			public void Init()
			{
				this.m_Choice.Init();
				for (uint num = 0U; num < this.m_NumPosStates; num += 1U)
				{
					this.m_LowCoder[(int)num].Init();
					this.m_MidCoder[(int)num].Init();
				}
				this.m_Choice2.Init();
				this.m_HighCoder.Init();
			}

			// Token: 0x06000100 RID: 256 RVA: 0x00009870 File Offset: 0x00007A70
			public uint Decode(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint posState)
			{
				if (this.m_Choice.Decode(rangeDecoder) == 0U)
				{
					return this.m_LowCoder[(int)posState].Decode(rangeDecoder);
				}
				uint num = 8U;
				if (this.m_Choice2.Decode(rangeDecoder) == 0U)
				{
					num += this.m_MidCoder[(int)posState].Decode(rangeDecoder);
				}
				else
				{
					num += 8U;
					num += this.m_HighCoder.Decode(rangeDecoder);
				}
				return num;
			}

			// Token: 0x04000123 RID: 291
			private BitDecoder m_Choice;

			// Token: 0x04000124 RID: 292
			private BitDecoder m_Choice2;

			// Token: 0x04000125 RID: 293
			private BitTreeDecoder[] m_LowCoder = new BitTreeDecoder[16];

			// Token: 0x04000126 RID: 294
			private BitTreeDecoder[] m_MidCoder = new BitTreeDecoder[16];

			// Token: 0x04000127 RID: 295
			private BitTreeDecoder m_HighCoder = new BitTreeDecoder(8);

			// Token: 0x04000128 RID: 296
			private uint m_NumPosStates;
		}

		// Token: 0x0200002B RID: 43
		private class LiteralDecoder
		{
			// Token: 0x06000102 RID: 258 RVA: 0x00009908 File Offset: 0x00007B08
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
				this.m_Coders = new Decoder.LiteralDecoder.Decoder2[num];
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					this.m_Coders[(int)num2].Create();
				}
			}

			// Token: 0x06000103 RID: 259 RVA: 0x00009988 File Offset: 0x00007B88
			public void Init()
			{
				uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					this.m_Coders[(int)num2].Init();
				}
			}

			// Token: 0x06000104 RID: 260 RVA: 0x000099C5 File Offset: 0x00007BC5
			private uint GetState(uint pos, byte prevByte)
			{
				return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
			}

			// Token: 0x06000105 RID: 261 RVA: 0x000099E7 File Offset: 0x00007BE7
			public byte DecodeNormal(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte)
			{
				return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeNormal(rangeDecoder);
			}

			// Token: 0x06000106 RID: 262 RVA: 0x00009A02 File Offset: 0x00007C02
			public byte DecodeWithMatchByte(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
			{
				return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);
			}

			// Token: 0x04000129 RID: 297
			private Decoder.LiteralDecoder.Decoder2[] m_Coders;

			// Token: 0x0400012A RID: 298
			private int m_NumPrevBits;

			// Token: 0x0400012B RID: 299
			private int m_NumPosBits;

			// Token: 0x0400012C RID: 300
			private uint m_PosMask;

			// Token: 0x02000031 RID: 49
			private struct Decoder2
			{
				// Token: 0x0600011A RID: 282 RVA: 0x00009DFF File Offset: 0x00007FFF
				public void Create()
				{
					this.m_Decoders = new BitDecoder[768];
				}

				// Token: 0x0600011B RID: 283 RVA: 0x00009E14 File Offset: 0x00008014
				public void Init()
				{
					for (int i = 0; i < 768; i++)
					{
						this.m_Decoders[i].Init();
					}
				}

				// Token: 0x0600011C RID: 284 RVA: 0x00009E44 File Offset: 0x00008044
				public byte DecodeNormal(SevenZip.Compression.RangeCoder.Decoder rangeDecoder)
				{
					uint num = 1U;
					do
					{
						num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
					}
					while (num < 256U);
					return (byte)num;
				}

				// Token: 0x0600011D RID: 285 RVA: 0x00009E74 File Offset: 0x00008074
				public byte DecodeWithMatchByte(SevenZip.Compression.RangeCoder.Decoder rangeDecoder, byte matchByte)
				{
					uint num = 1U;
					for (;;)
					{
						uint num2 = (uint)(matchByte >> 7 & 1);
						matchByte = (byte)(matchByte << 1);
						uint num3 = this.m_Decoders[(int)((1U + num2 << 8) + num)].Decode(rangeDecoder);
						num = (num << 1 | num3);
						if (num2 != num3)
						{
							break;
						}
						if (num >= 256U)
						{
							goto IL_5C;
						}
					}
					while (num < 256U)
					{
						num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
					}
					IL_5C:
					return (byte)num;
				}

				// Token: 0x04000148 RID: 328
				private BitDecoder[] m_Decoders;
			}
		}
	}
}
