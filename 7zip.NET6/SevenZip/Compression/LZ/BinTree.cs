using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x0200001B RID: 27
	public class BinTree : InWindow, IMatchFinder, IInWindowStream
	{
		// Token: 0x0600008C RID: 140 RVA: 0x00005690 File Offset: 0x00003890
		public void SetType(int numHashBytes)
		{
			this.HASH_ARRAY = (numHashBytes > 2);
			if (this.HASH_ARRAY)
			{
				this.kNumHashDirectBytes = 0U;
				this.kMinMatchCheck = 4U;
				this.kFixHashSize = 66560U;
				return;
			}
			this.kNumHashDirectBytes = 2U;
			this.kMinMatchCheck = 3U;
			this.kFixHashSize = 0U;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000056DE File Offset: 0x000038DE
		public new void SetStream(Stream stream)
		{
			base.SetStream(stream);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000056E7 File Offset: 0x000038E7
		public new void ReleaseStream()
		{
			base.ReleaseStream();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000056F0 File Offset: 0x000038F0
		public new void Init()
		{
			base.Init();
			for (uint num = 0U; num < this._hashSizeSum; num += 1U)
			{
				this._hash[(int)num] = 0U;
			}
			this._cyclicBufferPos = 0U;
			base.ReduceOffsets(-1);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000572C File Offset: 0x0000392C
		public new void MovePos()
		{
			uint num = this._cyclicBufferPos + 1U;
			this._cyclicBufferPos = num;
			if (num >= this._cyclicBufferSize)
			{
				this._cyclicBufferPos = 0U;
			}
			base.MovePos();
			if (this._pos == 2147483647U)
			{
				this.Normalize();
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005772 File Offset: 0x00003972
		public new byte GetIndexByte(int index)
		{
			return base.GetIndexByte(index);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000577B File Offset: 0x0000397B
		public new uint GetMatchLen(int index, uint distance, uint limit)
		{
			return base.GetMatchLen(index, distance, limit);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00005786 File Offset: 0x00003986
		public new uint GetNumAvailableBytes()
		{
			return base.GetNumAvailableBytes();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005790 File Offset: 0x00003990
		public void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter)
		{
			if (historySize > 2147483391U)
			{
				throw new Exception();
			}
			this._cutValue = 16U + (matchMaxLen >> 1);
			uint keepSizeReserv = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2U + 256U;
			base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, keepSizeReserv);
			this._matchMaxLen = matchMaxLen;
			uint num = historySize + 1U;
			if (this._cyclicBufferSize != num)
			{
				this._son = new uint[(this._cyclicBufferSize = num) * 2U];
			}
			uint num2 = 65536U;
			if (this.HASH_ARRAY)
			{
				num2 = historySize - 1U;
				num2 |= num2 >> 1;
				num2 |= num2 >> 2;
				num2 |= num2 >> 4;
				num2 |= num2 >> 8;
				num2 >>= 1;
				num2 |= 65535U;
				if (num2 > 16777216U)
				{
					num2 >>= 1;
				}
				this._hashMask = num2;
				num2 += 1U;
				num2 += this.kFixHashSize;
			}
			if (num2 != this._hashSizeSum)
			{
				this._hash = new uint[this._hashSizeSum = num2];
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005878 File Offset: 0x00003A78
		public uint GetMatches(uint[] distances)
		{
			uint num;
			if (this._pos + this._matchMaxLen <= this._streamPos)
			{
				num = this._matchMaxLen;
			}
			else
			{
				num = this._streamPos - this._pos;
				if (num < this.kMinMatchCheck)
				{
					this.MovePos();
					return 0U;
				}
			}
			uint num2 = 0U;
			uint num3 = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0U;
			uint num4 = this._bufferOffset + this._pos;
			uint num5 = 1U;
			uint num6 = 0U;
			uint num7 = 0U;
			uint num10;
			if (this.HASH_ARRAY)
			{
				uint num8 = CRC.Table[(int)this._bufferBase[(int)num4]] ^ (uint)this._bufferBase[(int)(num4 + 1U)];
				num6 = (num8 & 1023U);
				uint num9 = num8 ^ (uint)((uint)this._bufferBase[(int)(num4 + 2U)] << 8);
				num7 = (num9 & 65535U);
				num10 = ((num9 ^ CRC.Table[(int)this._bufferBase[(int)(num4 + 3U)]] << 5) & this._hashMask);
			}
			else
			{
				num10 = (uint)((int)this._bufferBase[(int)num4] ^ (int)this._bufferBase[(int)(num4 + 1U)] << 8);
			}
			uint num11 = this._hash[(int)(this.kFixHashSize + num10)];
			if (this.HASH_ARRAY)
			{
				uint num12 = this._hash[(int)num6];
				uint num13 = this._hash[(int)(1024U + num7)];
				this._hash[(int)num6] = this._pos;
				this._hash[(int)(1024U + num7)] = this._pos;
				if (num12 > num3 && this._bufferBase[(int)(this._bufferOffset + num12)] == this._bufferBase[(int)num4])
				{
					num5 = (distances[(int)num2++] = 2U);
					distances[(int)num2++] = this._pos - num12 - 1U;
				}
				if (num13 > num3 && this._bufferBase[(int)(this._bufferOffset + num13)] == this._bufferBase[(int)num4])
				{
					if (num13 == num12)
					{
						num2 -= 2U;
					}
					num5 = (distances[(int)num2++] = 3U);
					distances[(int)num2++] = this._pos - num13 - 1U;
					num12 = num13;
				}
				if (num2 != 0U && num12 == num11)
				{
					num2 -= 2U;
					num5 = 1U;
				}
			}
			this._hash[(int)(this.kFixHashSize + num10)] = this._pos;
			uint num14 = (this._cyclicBufferPos << 1) + 1U;
			uint num15 = this._cyclicBufferPos << 1;
			uint val2;
			uint val = val2 = this.kNumHashDirectBytes;
			if (this.kNumHashDirectBytes != 0U && num11 > num3 && this._bufferBase[(int)(this._bufferOffset + num11 + this.kNumHashDirectBytes)] != this._bufferBase[(int)(num4 + this.kNumHashDirectBytes)])
			{
				num5 = (distances[(int)num2++] = this.kNumHashDirectBytes);
				distances[(int)num2++] = this._pos - num11 - 1U;
			}
			uint cutValue = this._cutValue;
			while (num11 > num3 && cutValue-- != 0U)
			{
				uint num16 = this._pos - num11;
				uint num17 = ((num16 <= this._cyclicBufferPos) ? (this._cyclicBufferPos - num16) : (this._cyclicBufferPos - num16 + this._cyclicBufferSize)) << 1;
				uint num18 = this._bufferOffset + num11;
				uint num19 = Math.Min(val2, val);
				if (this._bufferBase[(int)(num18 + num19)] == this._bufferBase[(int)(num4 + num19)])
				{
					while ((num19 += 1U) != num && this._bufferBase[(int)(num18 + num19)] == this._bufferBase[(int)(num4 + num19)])
					{
					}
					if (num5 < num19)
					{
						num5 = (distances[(int)num2++] = num19);
						distances[(int)num2++] = num16 - 1U;
						if (num19 == num)
						{
							this._son[(int)num15] = this._son[(int)num17];
							this._son[(int)num14] = this._son[(int)(num17 + 1U)];
							IL_3D1:
							this.MovePos();
							return num2;
						}
					}
				}
				if (this._bufferBase[(int)(num18 + num19)] < this._bufferBase[(int)(num4 + num19)])
				{
					this._son[(int)num15] = num11;
					num15 = num17 + 1U;
					num11 = this._son[(int)num15];
					val = num19;
				}
				else
				{
					this._son[(int)num14] = num11;
					num14 = num17;
					num11 = this._son[(int)num14];
					val2 = num19;
				}
			}
			this._son[(int)num14] = (this._son[(int)num15] = 0U);
			this.MovePos();
			return num2;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005C60 File Offset: 0x00003E60
		public void Skip(uint num)
		{
			for (;;)
			{
				uint num2;
				if (this._pos + this._matchMaxLen <= this._streamPos)
				{
					num2 = this._matchMaxLen;
					goto IL_40;
				}
				num2 = this._streamPos - this._pos;
				if (num2 >= this.kMinMatchCheck)
				{
					goto IL_40;
				}
				this.MovePos();
				IL_29A:
				if ((num -= 1U) == 0U)
				{
					break;
				}
				continue;
				IL_40:
				uint num3 = (this._pos > this._cyclicBufferSize) ? (this._pos - this._cyclicBufferSize) : 0U;
				uint num4 = this._bufferOffset + this._pos;
				uint num9;
				if (this.HASH_ARRAY)
				{
					uint num5 = CRC.Table[(int)this._bufferBase[(int)num4]] ^ (uint)this._bufferBase[(int)(num4 + 1U)];
					uint num6 = num5 & 1023U;
					this._hash[(int)num6] = this._pos;
					uint num7 = num5 ^ (uint)((uint)this._bufferBase[(int)(num4 + 2U)] << 8);
					uint num8 = num7 & 65535U;
					this._hash[(int)(1024U + num8)] = this._pos;
					num9 = ((num7 ^ CRC.Table[(int)this._bufferBase[(int)(num4 + 3U)]] << 5) & this._hashMask);
				}
				else
				{
					num9 = (uint)((int)this._bufferBase[(int)num4] ^ (int)this._bufferBase[(int)(num4 + 1U)] << 8);
				}
				uint num10 = this._hash[(int)(this.kFixHashSize + num9)];
				this._hash[(int)(this.kFixHashSize + num9)] = this._pos;
				uint num11 = (this._cyclicBufferPos << 1) + 1U;
				uint num12 = this._cyclicBufferPos << 1;
				uint val2;
				uint val = val2 = this.kNumHashDirectBytes;
				uint cutValue = this._cutValue;
				while (num10 > num3 && cutValue-- != 0U)
				{
					uint num13 = this._pos - num10;
					uint num14 = ((num13 <= this._cyclicBufferPos) ? (this._cyclicBufferPos - num13) : (this._cyclicBufferPos - num13 + this._cyclicBufferSize)) << 1;
					uint num15 = this._bufferOffset + num10;
					uint num16 = Math.Min(val2, val);
					if (this._bufferBase[(int)(num15 + num16)] == this._bufferBase[(int)(num4 + num16)])
					{
						while ((num16 += 1U) != num2 && this._bufferBase[(int)(num15 + num16)] == this._bufferBase[(int)(num4 + num16)])
						{
						}
						if (num16 == num2)
						{
							this._son[(int)num12] = this._son[(int)num14];
							this._son[(int)num11] = this._son[(int)(num14 + 1U)];
							IL_294:
							this.MovePos();
							goto IL_29A;
						}
					}
					if (this._bufferBase[(int)(num15 + num16)] < this._bufferBase[(int)(num4 + num16)])
					{
						this._son[(int)num12] = num10;
						num12 = num14 + 1U;
						num10 = this._son[(int)num12];
						val = num16;
					}
					else
					{
						this._son[(int)num11] = num10;
						num11 = num14;
						num10 = this._son[(int)num11];
						val2 = num16;
					}
				}
				this._son[(int)num11] = (this._son[(int)num12] = 0U);
				this.MovePos();
				goto IL_29A;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005F14 File Offset: 0x00004114
		private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
		{
			for (uint num = 0U; num < numItems; num += 1U)
			{
				uint num2 = items[(int)num];
				if (num2 <= subValue)
				{
					num2 = 0U;
				}
				else
				{
					num2 -= subValue;
				}
				items[(int)num] = num2;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005F44 File Offset: 0x00004144
		private void Normalize()
		{
			uint subValue = this._pos - this._cyclicBufferSize;
			this.NormalizeLinks(this._son, this._cyclicBufferSize * 2U, subValue);
			this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
			base.ReduceOffsets((int)subValue);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005F8E File Offset: 0x0000418E
		public void SetCutValue(uint cutValue)
		{
			this._cutValue = cutValue;
		}

		// Token: 0x04000071 RID: 113
		private uint _cyclicBufferPos;

		// Token: 0x04000072 RID: 114
		private uint _cyclicBufferSize;

		// Token: 0x04000073 RID: 115
		private uint _matchMaxLen;

		// Token: 0x04000074 RID: 116
		private uint[] _son;

		// Token: 0x04000075 RID: 117
		private uint[] _hash;

		// Token: 0x04000076 RID: 118
		private uint _cutValue = 255U;

		// Token: 0x04000077 RID: 119
		private uint _hashMask;

		// Token: 0x04000078 RID: 120
		private uint _hashSizeSum;

		// Token: 0x04000079 RID: 121
		private bool HASH_ARRAY = true;

		// Token: 0x0400007A RID: 122
		private const uint kHash2Size = 1024U;

		// Token: 0x0400007B RID: 123
		private const uint kHash3Size = 65536U;

		// Token: 0x0400007C RID: 124
		private const uint kBT2HashSize = 65536U;

		// Token: 0x0400007D RID: 125
		private const uint kStartMaxLen = 1U;

		// Token: 0x0400007E RID: 126
		private const uint kHash3Offset = 1024U;

		// Token: 0x0400007F RID: 127
		private const uint kEmptyHashValue = 0U;

		// Token: 0x04000080 RID: 128
		private const uint kMaxValForNormalize = 2147483647U;

		// Token: 0x04000081 RID: 129
		private uint kNumHashDirectBytes;

		// Token: 0x04000082 RID: 130
		private uint kMinMatchCheck = 4U;

		// Token: 0x04000083 RID: 131
		private uint kFixHashSize = 66560U;
	}
}
