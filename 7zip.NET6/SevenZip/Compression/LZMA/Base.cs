using System;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x0200001E RID: 30
	internal abstract class Base
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x0000651A File Offset: 0x0000471A
		public static uint GetLenToPosState(uint len)
		{
			len -= 2U;
			if (len < 4U)
			{
				return len;
			}
			return 3U;
		}

		// Token: 0x04000095 RID: 149
		public const uint kNumRepDistances = 4U;

		// Token: 0x04000096 RID: 150
		public const uint kNumStates = 12U;

		// Token: 0x04000097 RID: 151
		public const int kNumPosSlotBits = 6;

		// Token: 0x04000098 RID: 152
		public const int kDicLogSizeMin = 0;

		// Token: 0x04000099 RID: 153
		public const int kNumLenToPosStatesBits = 2;

		// Token: 0x0400009A RID: 154
		public const uint kNumLenToPosStates = 4U;

		// Token: 0x0400009B RID: 155
		public const uint kMatchMinLen = 2U;

		// Token: 0x0400009C RID: 156
		public const int kNumAlignBits = 4;

		// Token: 0x0400009D RID: 157
		public const uint kAlignTableSize = 16U;

		// Token: 0x0400009E RID: 158
		public const uint kAlignMask = 15U;

		// Token: 0x0400009F RID: 159
		public const uint kStartPosModelIndex = 4U;

		// Token: 0x040000A0 RID: 160
		public const uint kEndPosModelIndex = 14U;

		// Token: 0x040000A1 RID: 161
		public const uint kNumPosModels = 10U;

		// Token: 0x040000A2 RID: 162
		public const uint kNumFullDistances = 128U;

		// Token: 0x040000A3 RID: 163
		public const uint kNumLitPosStatesBitsEncodingMax = 4U;

		// Token: 0x040000A4 RID: 164
		public const uint kNumLitContextBitsMax = 8U;

		// Token: 0x040000A5 RID: 165
		public const int kNumPosStatesBitsMax = 4;

		// Token: 0x040000A6 RID: 166
		public const uint kNumPosStatesMax = 16U;

		// Token: 0x040000A7 RID: 167
		public const int kNumPosStatesBitsEncodingMax = 4;

		// Token: 0x040000A8 RID: 168
		public const uint kNumPosStatesEncodingMax = 16U;

		// Token: 0x040000A9 RID: 169
		public const int kNumLowLenBits = 3;

		// Token: 0x040000AA RID: 170
		public const int kNumMidLenBits = 3;

		// Token: 0x040000AB RID: 171
		public const int kNumHighLenBits = 8;

		// Token: 0x040000AC RID: 172
		public const uint kNumLowLenSymbols = 8U;

		// Token: 0x040000AD RID: 173
		public const uint kNumMidLenSymbols = 8U;

		// Token: 0x040000AE RID: 174
		public const uint kNumLenSymbols = 272U;

		// Token: 0x040000AF RID: 175
		public const uint kMatchMaxLen = 273U;

		// Token: 0x02000029 RID: 41
		public struct State
		{
			// Token: 0x060000F8 RID: 248 RVA: 0x0000972E File Offset: 0x0000792E
			public void Init()
			{
				this.Index = 0U;
			}

			// Token: 0x060000F9 RID: 249 RVA: 0x00009737 File Offset: 0x00007937
			public void UpdateChar()
			{
				if (this.Index < 4U)
				{
					this.Index = 0U;
					return;
				}
				if (this.Index < 10U)
				{
					this.Index -= 3U;
					return;
				}
				this.Index -= 6U;
			}

			// Token: 0x060000FA RID: 250 RVA: 0x00009771 File Offset: 0x00007971
			public void UpdateMatch()
			{
				this.Index = ((this.Index < 7U) ? 7U : 10U);
			}

			// Token: 0x060000FB RID: 251 RVA: 0x00009787 File Offset: 0x00007987
			public void UpdateRep()
			{
				this.Index = ((this.Index < 7U) ? 8U : 11U);
			}

			// Token: 0x060000FC RID: 252 RVA: 0x0000979D File Offset: 0x0000799D
			public void UpdateShortRep()
			{
				this.Index = ((this.Index < 7U) ? 9U : 11U);
			}

			// Token: 0x060000FD RID: 253 RVA: 0x000097B4 File Offset: 0x000079B4
			public bool IsCharState()
			{
				return this.Index < 7U;
			}

			// Token: 0x04000122 RID: 290
			public uint Index;
		}
	}
}
