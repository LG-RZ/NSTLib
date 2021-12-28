using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000015 RID: 21
	internal struct BitEncoder
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000050D4 File Offset: 0x000032D4
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000050E1 File Offset: 0x000032E1
		public void UpdateModel(uint symbol)
		{
			if (symbol == 0U)
			{
				this.Prob += 2048U - this.Prob >> 5;
				return;
			}
			this.Prob -= this.Prob >> 5;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00005118 File Offset: 0x00003318
		public void Encode(Encoder encoder, uint symbol)
		{
			uint num = (encoder.Range >> 11) * this.Prob;
			if (symbol == 0U)
			{
				encoder.Range = num;
				this.Prob += 2048U - this.Prob >> 5;
			}
			else
			{
				encoder.Low += (ulong)num;
				encoder.Range -= num;
				this.Prob -= this.Prob >> 5;
			}
			if (encoder.Range < 16777216U)
			{
				encoder.Range <<= 8;
				encoder.ShiftLow();
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000051B0 File Offset: 0x000033B0
		static BitEncoder()
		{
			for (int i = 8; i >= 0; i--)
			{
				uint num = 1U << 9 - i - 1;
				uint num2 = 1U << 9 - i;
				for (uint num3 = num; num3 < num2; num3 += 1U)
				{
					BitEncoder.ProbPrices[(int)num3] = (uint)((i << 6) + (int)(num2 - num3 << 6 >> 9 - i - 1));
				}
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005212 File Offset: 0x00003412
		public uint GetPrice(uint symbol)
		{
			return BitEncoder.ProbPrices[(int)(checked((IntPtr)((unchecked((ulong)(this.Prob - symbol) ^ (ulong)((long)(-(long)symbol))) & 2047UL) >> 2)))];
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00005231 File Offset: 0x00003431
		public uint GetPrice0()
		{
			return BitEncoder.ProbPrices[(int)(this.Prob >> 2)];
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00005241 File Offset: 0x00003441
		public uint GetPrice1()
		{
			return BitEncoder.ProbPrices[(int)(2048U - this.Prob >> 2)];
		}

		// Token: 0x04000062 RID: 98
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x04000063 RID: 99
		public const uint kBitModelTotal = 2048U;

		// Token: 0x04000064 RID: 100
		private const int kNumMoveBits = 5;

		// Token: 0x04000065 RID: 101
		private const int kNumMoveReducingBits = 2;

		// Token: 0x04000066 RID: 102
		public const int kNumBitPriceShiftBits = 6;

		// Token: 0x04000067 RID: 103
		private uint Prob;

		// Token: 0x04000068 RID: 104
		private static uint[] ProbPrices = new uint[512];
	}
}
