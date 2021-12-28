using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x02000022 RID: 34
	public class OutBuffer
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x000091B5 File Offset: 0x000073B5
		public OutBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000091D0 File Offset: 0x000073D0
		public void SetStream(Stream stream)
		{
			this.m_Stream = stream;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000091D9 File Offset: 0x000073D9
		public void FlushStream()
		{
			this.m_Stream.Flush();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000091E6 File Offset: 0x000073E6
		public void CloseStream()
		{
			this.m_Stream.Close();
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000091F3 File Offset: 0x000073F3
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000091FC File Offset: 0x000073FC
		public void Init()
		{
			this.m_ProcessedSize = 0UL;
			this.m_Pos = 0U;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00009210 File Offset: 0x00007410
		public void WriteByte(byte b)
		{
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			buffer[(int)pos] = b;
			if (this.m_Pos >= this.m_BufferSize)
			{
				this.FlushData();
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000924A File Offset: 0x0000744A
		public void FlushData()
		{
			if (this.m_Pos == 0U)
			{
				return;
			}
			this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
			this.m_Pos = 0U;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00009274 File Offset: 0x00007474
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x04000103 RID: 259
		private byte[] m_Buffer;

		// Token: 0x04000104 RID: 260
		private uint m_Pos;

		// Token: 0x04000105 RID: 261
		private uint m_BufferSize;

		// Token: 0x04000106 RID: 262
		private Stream m_Stream;

		// Token: 0x04000107 RID: 263
		private ulong m_ProcessedSize;
	}
}
