using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x02000021 RID: 33
	public class InBuffer
	{
		// Token: 0x060000DB RID: 219 RVA: 0x00009073 File Offset: 0x00007273
		public InBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000908E File Offset: 0x0000728E
		public void Init(Stream stream)
		{
			this.m_Stream = stream;
			this.m_ProcessedSize = 0UL;
			this.m_Limit = 0U;
			this.m_Pos = 0U;
			this.m_StreamWasExhausted = false;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000090B4 File Offset: 0x000072B4
		public bool ReadBlock()
		{
			if (this.m_StreamWasExhausted)
			{
				return false;
			}
			this.m_ProcessedSize += (ulong)this.m_Pos;
			int num = this.m_Stream.Read(this.m_Buffer, 0, (int)this.m_BufferSize);
			this.m_Pos = 0U;
			this.m_Limit = (uint)num;
			this.m_StreamWasExhausted = (num == 0);
			return !this.m_StreamWasExhausted;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00009119 File Offset: 0x00007319
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009124 File Offset: 0x00007324
		public bool ReadByte(byte b)
		{
			if (this.m_Pos >= this.m_Limit && !this.ReadBlock())
			{
				return false;
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			b = buffer[(int)pos];
			return true;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00009164 File Offset: 0x00007364
		public byte ReadByte()
		{
			if (this.m_Pos >= this.m_Limit && !this.ReadBlock())
			{
				return byte.MaxValue;
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			return buffer[(int)pos];
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000091A5 File Offset: 0x000073A5
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x040000FC RID: 252
		private byte[] m_Buffer;

		// Token: 0x040000FD RID: 253
		private uint m_Pos;

		// Token: 0x040000FE RID: 254
		private uint m_Limit;

		// Token: 0x040000FF RID: 255
		private uint m_BufferSize;

		// Token: 0x04000100 RID: 256
		private Stream m_Stream;

		// Token: 0x04000101 RID: 257
		private bool m_StreamWasExhausted;

		// Token: 0x04000102 RID: 258
		private ulong m_ProcessedSize;
	}
}
