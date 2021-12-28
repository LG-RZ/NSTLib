using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000013 RID: 19
	internal class Encoder
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00004C84 File Offset: 0x00002E84
		public void SetStream(Stream stream)
		{
			this.Stream = stream;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00004C8D File Offset: 0x00002E8D
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004C96 File Offset: 0x00002E96
		public void Init()
		{
			this.StartPosition = this.Stream.Position;
			this.Low = 0UL;
			this.Range = uint.MaxValue;
			this._cacheSize = 1U;
			this._cache = 0;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004CC8 File Offset: 0x00002EC8
		public void FlushData()
		{
			for (int i = 0; i < 5; i++)
			{
				this.ShiftLow();
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004CE7 File Offset: 0x00002EE7
		public void FlushStream()
		{
			this.Stream.Flush();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004CF4 File Offset: 0x00002EF4
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004D04 File Offset: 0x00002F04
		public void Encode(uint start, uint size, uint total)
		{
			this.Low += (ulong)(start * (this.Range /= total));
			this.Range *= size;
			while (this.Range < 16777216U)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004D64 File Offset: 0x00002F64
		public void ShiftLow()
		{
			if ((uint)this.Low < 4278190080U || (uint)(this.Low >> 32) == 1U)
			{
				byte b = this._cache;
				uint num;
				do
				{
					this.Stream.WriteByte((byte)((ulong)b + (this.Low >> 32)));
					b = byte.MaxValue;
					num = this._cacheSize - 1U;
					this._cacheSize = num;
				}
				while (num != 0U);
				this._cache = (byte)((uint)this.Low >> 24);
			}
			this._cacheSize += 1U;
			this.Low = (ulong)((ulong)((uint)this.Low) << 8);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004DF4 File Offset: 0x00002FF4
		public void EncodeDirectBits(uint v, int numTotalBits)
		{
			for (int i = numTotalBits - 1; i >= 0; i--)
			{
				this.Range >>= 1;
				if ((v >> i & 1U) == 1U)
				{
					this.Low += (ulong)this.Range;
				}
				if (this.Range < 16777216U)
				{
					this.Range <<= 8;
					this.ShiftLow();
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004E60 File Offset: 0x00003060
		public void EncodeBit(uint size0, int numTotalBits, uint symbol)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			if (symbol == 0U)
			{
				this.Range = num;
			}
			else
			{
				this.Low += (ulong)num;
				this.Range -= num;
			}
			while (this.Range < 16777216U)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004EC7 File Offset: 0x000030C7
		public long GetProcessedSizeAdd()
		{
			return (long)((ulong)this._cacheSize + (ulong)this.Stream.Position - (ulong)this.StartPosition + 4UL);
		}

		// Token: 0x04000057 RID: 87
		public const uint kTopValue = 16777216U;

		// Token: 0x04000058 RID: 88
		private Stream Stream;

		// Token: 0x04000059 RID: 89
		public ulong Low;

		// Token: 0x0400005A RID: 90
		public uint Range;

		// Token: 0x0400005B RID: 91
		private uint _cacheSize;

		// Token: 0x0400005C RID: 92
		private byte _cache;

		// Token: 0x0400005D RID: 93
		private long StartPosition;
	}
}
