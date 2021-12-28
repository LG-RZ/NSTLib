using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000017 RID: 23
	internal struct BitTreeEncoder
	{
		// Token: 0x06000076 RID: 118 RVA: 0x00005389 File Offset: 0x00003589
		public BitTreeEncoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitEncoder[1 << numBitLevels];
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000053A4 File Offset: 0x000035A4
		public void Init()
		{
			uint num = 1U;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)num].Init();
				num += 1U;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000053DC File Offset: 0x000035DC
		public void Encode(Encoder rangeEncoder, uint symbol)
		{
			uint num = 1U;
			int i = this.NumBitLevels;
			while (i > 0)
			{
				i--;
				uint num2 = symbol >> i & 1U;
				this.Models[(int)num].Encode(rangeEncoder, num2);
				num = (num << 1 | num2);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005420 File Offset: 0x00003620
		public void ReverseEncode(Encoder rangeEncoder, uint symbol)
		{
			uint num = 1U;
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)this.NumBitLevels))
			{
				uint num3 = symbol & 1U;
				this.Models[(int)num].Encode(rangeEncoder, num3);
				num = (num << 1 | num3);
				symbol >>= 1;
				num2 += 1U;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005464 File Offset: 0x00003664
		public uint GetPrice(uint symbol)
		{
			uint num = 0U;
			uint num2 = 1U;
			int i = this.NumBitLevels;
			while (i > 0)
			{
				i--;
				uint num3 = symbol >> i & 1U;
				num += this.Models[(int)num2].GetPrice(num3);
				num2 = (num2 << 1) + num3;
			}
			return num;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000054AC File Offset: 0x000036AC
		public uint ReverseGetPrice(uint symbol)
		{
			uint num = 0U;
			uint num2 = 1U;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				uint num3 = symbol & 1U;
				symbol >>= 1;
				num += this.Models[(int)num2].GetPrice(num3);
				num2 = (num2 << 1 | num3);
			}
			return num;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000054F4 File Offset: 0x000036F4
		public static uint ReverseGetPrice(BitEncoder[] Models, uint startIndex, int NumBitLevels, uint symbol)
		{
			uint num = 0U;
			uint num2 = 1U;
			for (int i = NumBitLevels; i > 0; i--)
			{
				uint num3 = symbol & 1U;
				symbol >>= 1;
				num += Models[(int)(startIndex + num2)].GetPrice(num3);
				num2 = (num2 << 1 | num3);
			}
			return num;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005534 File Offset: 0x00003734
		public static void ReverseEncode(BitEncoder[] Models, uint startIndex, Encoder rangeEncoder, int NumBitLevels, uint symbol)
		{
			uint num = 1U;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num2 = symbol & 1U;
				Models[(int)(startIndex + num)].Encode(rangeEncoder, num2);
				num = (num << 1 | num2);
				symbol >>= 1;
			}
		}

		// Token: 0x0400006D RID: 109
		private BitEncoder[] Models;

		// Token: 0x0400006E RID: 110
		private int NumBitLevels;
	}
}
