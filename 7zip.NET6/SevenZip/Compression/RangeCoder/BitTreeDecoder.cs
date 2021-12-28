using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000018 RID: 24
	internal struct BitTreeDecoder
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00005570 File Offset: 0x00003770
		public BitTreeDecoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitDecoder[1 << numBitLevels];
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000558C File Offset: 0x0000378C
		public void Init()
		{
			uint num = 1U;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)num].Init();
				num += 1U;
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000055C4 File Offset: 0x000037C4
		public uint Decode(Decoder rangeDecoder)
		{
			uint num = 1U;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				num = (num << 1) + this.Models[(int)num].Decode(rangeDecoder);
			}
			return num - (1U << this.NumBitLevels);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005608 File Offset: 0x00003808
		public uint ReverseDecode(Decoder rangeDecoder)
		{
			uint num = 1U;
			uint num2 = 0U;
			for (int i = 0; i < this.NumBitLevels; i++)
			{
				uint num3 = this.Models[(int)num].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00005650 File Offset: 0x00003850
		public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
		{
			uint num = 1U;
			uint num2 = 0U;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num3 = Models[(int)(startIndex + num)].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		// Token: 0x0400006F RID: 111
		private BitDecoder[] Models;

		// Token: 0x04000070 RID: 112
		private int NumBitLevels;
	}
}
