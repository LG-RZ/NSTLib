using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000016 RID: 22
	internal struct BitDecoder
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00005257 File Offset: 0x00003457
		public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0U)
			{
				this.Prob += 2048U - this.Prob >> numMoveBits;
				return;
			}
			this.Prob -= this.Prob >> numMoveBits;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00005293 File Offset: 0x00003493
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000052A0 File Offset: 0x000034A0
		public uint Decode(Decoder rangeDecoder)
		{
			uint num = (rangeDecoder.Range >> 11) * this.Prob;
			if (rangeDecoder.Code < num)
			{
				rangeDecoder.Range = num;
				this.Prob += 2048U - this.Prob >> 5;
				if (rangeDecoder.Range < 16777216U)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
					rangeDecoder.Range <<= 8;
				}
				return 0U;
			}
			rangeDecoder.Range -= num;
			rangeDecoder.Code -= num;
			this.Prob -= this.Prob >> 5;
			if (rangeDecoder.Range < 16777216U)
			{
				rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
				rangeDecoder.Range <<= 8;
			}
			return 1U;
		}

		// Token: 0x04000069 RID: 105
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x0400006A RID: 106
		public const uint kBitModelTotal = 2048U;

		// Token: 0x0400006B RID: 107
		private const int kNumMoveBits = 5;

		// Token: 0x0400006C RID: 108
		private uint Prob;
	}
}
