using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000014 RID: 20
	internal class Decoder
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00004EE8 File Offset: 0x000030E8
		public void Init(Stream stream)
		{
			this.Stream = stream;
			this.Code = 0U;
			this.Range = uint.MaxValue;
			for (int i = 0; i < 5; i++)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004F31 File Offset: 0x00003131
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004F3A File Offset: 0x0000313A
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004F47 File Offset: 0x00003147
		public void Normalize()
		{
			while (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004F81 File Offset: 0x00003181
		public void Normalize2()
		{
			if (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004FBC File Offset: 0x000031BC
		public uint GetThreshold(uint total)
		{
			return this.Code / (this.Range /= total);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004FE1 File Offset: 0x000031E1
		public void Decode(uint start, uint size, uint total)
		{
			this.Code -= start * this.Range;
			this.Range *= size;
			this.Normalize();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000500C File Offset: 0x0000320C
		public uint DecodeDirectBits(int numTotalBits)
		{
			uint num = this.Range;
			uint num2 = this.Code;
			uint num3 = 0U;
			for (int i = numTotalBits; i > 0; i--)
			{
				num >>= 1;
				uint num4 = num2 - num >> 31;
				num2 -= (num & num4 - 1U);
				num3 = (num3 << 1 | 1U - num4);
				if (num < 16777216U)
				{
					num2 = (num2 << 8 | (uint)((byte)this.Stream.ReadByte()));
					num <<= 8;
				}
			}
			this.Range = num;
			this.Code = num2;
			return num3;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005080 File Offset: 0x00003280
		public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			uint result;
			if (this.Code < num)
			{
				result = 0U;
				this.Range = num;
			}
			else
			{
				result = 1U;
				this.Code -= num;
				this.Range -= num;
			}
			this.Normalize();
			return result;
		}

		// Token: 0x0400005E RID: 94
		public const uint kTopValue = 16777216U;

		// Token: 0x0400005F RID: 95
		public uint Range;

		// Token: 0x04000060 RID: 96
		public uint Code;

		// Token: 0x04000061 RID: 97
		public Stream Stream;
	}
}
