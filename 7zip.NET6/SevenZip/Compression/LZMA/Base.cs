using System;

namespace SevenZip.Compression.LZMA
{
	internal abstract class Base
	{
		public static uint GetLenToPosState(uint len)
		{
			len -= 2U;
			if (len < 4U)
			{
				return len;
			}
			return 3U;
		}

		public const uint kNumRepDistances = 4U;

		public const uint kNumStates = 12U;

		public const int kNumPosSlotBits = 6;

		public const int kDicLogSizeMin = 0;

		public const int kNumLenToPosStatesBits = 2;

		public const uint kNumLenToPosStates = 4U;

		public const uint kMatchMinLen = 2U;

		public const int kNumAlignBits = 4;

		public const uint kAlignTableSize = 16U;

		public const uint kAlignMask = 15U;

		public const uint kStartPosModelIndex = 4U;

		public const uint kEndPosModelIndex = 14U;

		public const uint kNumPosModels = 10U;

		public const uint kNumFullDistances = 128U;

		public const uint kNumLitPosStatesBitsEncodingMax = 4U;

		public const uint kNumLitContextBitsMax = 8U;

		public const int kNumPosStatesBitsMax = 4;

		public const uint kNumPosStatesMax = 16U;

		public const int kNumPosStatesBitsEncodingMax = 4;

		public const uint kNumPosStatesEncodingMax = 16U;

		public const int kNumLowLenBits = 3;

		public const int kNumMidLenBits = 3;

		public const int kNumHighLenBits = 8;

		public const uint kNumLowLenSymbols = 8U;

		public const uint kNumMidLenSymbols = 8U;

		public const uint kNumLenSymbols = 272U;

		public const uint kMatchMaxLen = 273U;

		public struct State
		{
			public void Init()
			{
				this.Index = 0U;
			}

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

			public void UpdateMatch()
			{
				this.Index = ((this.Index < 7U) ? 7U : 10U);
			}

			public void UpdateRep()
			{
				this.Index = ((this.Index < 7U) ? 8U : 11U);
			}

			public void UpdateShortRep()
			{
				this.Index = ((this.Index < 7U) ? 9U : 11U);
			}

			public bool IsCharState()
			{
				return this.Index < 7U;
			}

			public uint Index;
		}
	}
}
