using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x0200001C RID: 28
	public class InWindow
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00005FC4 File Offset: 0x000041C4
		public void MoveBlock()
		{
			uint num = this._bufferOffset + this._pos - this._keepSizeBefore;
			if (num > 0U)
			{
				num -= 1U;
			}
			uint num2 = this._bufferOffset + this._streamPos - num;
			for (uint num3 = 0U; num3 < num2; num3 += 1U)
			{
				this._bufferBase[(int)num3] = this._bufferBase[(int)(num + num3)];
			}
			this._bufferOffset -= num;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000602C File Offset: 0x0000422C
		public virtual void ReadBlock()
		{
			if (this._streamEndWasReached)
			{
				return;
			}
			for (;;)
			{
				int num = (int)(0U - this._bufferOffset + this._blockSize - this._streamPos);
				if (num == 0)
				{
					break;
				}
				int num2 = this._stream.Read(this._bufferBase, (int)(this._bufferOffset + this._streamPos), num);
				if (num2 == 0)
				{
					goto Block_3;
				}
				this._streamPos += (uint)num2;
				if (this._streamPos >= this._pos + this._keepSizeAfter)
				{
					this._posLimit = this._streamPos - this._keepSizeAfter;
				}
			}
			return;
			Block_3:
			this._posLimit = this._streamPos;
			if (this._bufferOffset + this._posLimit > this._pointerToLastSafePosition)
			{
				this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
			}
			this._streamEndWasReached = true;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000060F9 File Offset: 0x000042F9
		private void Free()
		{
			this._bufferBase = null;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006104 File Offset: 0x00004304
		public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
		{
			this._keepSizeBefore = keepSizeBefore;
			this._keepSizeAfter = keepSizeAfter;
			uint num = keepSizeBefore + keepSizeAfter + keepSizeReserv;
			if (this._bufferBase == null || this._blockSize != num)
			{
				this.Free();
				this._blockSize = num;
				this._bufferBase = new byte[this._blockSize];
			}
			this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006162 File Offset: 0x00004362
		public void SetStream(Stream stream)
		{
			this._stream = stream;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000616B File Offset: 0x0000436B
		public void ReleaseStream()
		{
			this._stream = null;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006174 File Offset: 0x00004374
		public void Init()
		{
			this._bufferOffset = 0U;
			this._pos = 0U;
			this._streamPos = 0U;
			this._streamEndWasReached = false;
			this.ReadBlock();
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006198 File Offset: 0x00004398
		public void MovePos()
		{
			this._pos += 1U;
			if (this._pos > this._posLimit)
			{
				if (this._bufferOffset + this._pos > this._pointerToLastSafePosition)
				{
					this.MoveBlock();
				}
				this.ReadBlock();
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000061D7 File Offset: 0x000043D7
		public byte GetIndexByte(int index)
		{
			return this._bufferBase[(int)(checked((IntPtr)(unchecked((ulong)(this._bufferOffset + this._pos) + (ulong)((long)index)))))];
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000061F4 File Offset: 0x000043F4
		public uint GetMatchLen(int index, uint distance, uint limit)
		{
			if (this._streamEndWasReached && (ulong)this._pos + (ulong)((long)index) + (ulong)limit > (ulong)this._streamPos)
			{
				limit = this._streamPos - (uint)((ulong)this._pos + (ulong)((long)index));
			}
			distance += 1U;
			uint num = this._bufferOffset + this._pos + (uint)index;
			uint num2 = 0U;
			while (num2 < limit && this._bufferBase[(int)(num + num2)] == this._bufferBase[(int)(num + num2 - distance)])
			{
				num2 += 1U;
			}
			return num2;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000626D File Offset: 0x0000446D
		public uint GetNumAvailableBytes()
		{
			return this._streamPos - this._pos;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000627C File Offset: 0x0000447C
		public void ReduceOffsets(int subValue)
		{
			this._bufferOffset += (uint)subValue;
			this._posLimit -= (uint)subValue;
			this._pos -= (uint)subValue;
			this._streamPos -= (uint)subValue;
		}

		// Token: 0x04000084 RID: 132
		public byte[] _bufferBase;

		// Token: 0x04000085 RID: 133
		private Stream _stream;

		// Token: 0x04000086 RID: 134
		private uint _posLimit;

		// Token: 0x04000087 RID: 135
		private bool _streamEndWasReached;

		// Token: 0x04000088 RID: 136
		private uint _pointerToLastSafePosition;

		// Token: 0x04000089 RID: 137
		public uint _bufferOffset;

		// Token: 0x0400008A RID: 138
		public uint _blockSize;

		// Token: 0x0400008B RID: 139
		public uint _pos;

		// Token: 0x0400008C RID: 140
		private uint _keepSizeBefore;

		// Token: 0x0400008D RID: 141
		private uint _keepSizeAfter;

		// Token: 0x0400008E RID: 142
		public uint _streamPos;
	}
}
