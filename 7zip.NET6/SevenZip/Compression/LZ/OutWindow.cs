using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x0200001D RID: 29
	public class OutWindow
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x000062B6 File Offset: 0x000044B6
		public void Create(uint windowSize)
		{
			if (this._windowSize != windowSize)
			{
				this._buffer = new byte[windowSize];
			}
			this._windowSize = windowSize;
			this._pos = 0U;
			this._streamPos = 0U;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000062E2 File Offset: 0x000044E2
		public void Init(Stream stream, bool solid)
		{
			this.ReleaseStream();
			this._stream = stream;
			if (!solid)
			{
				this._streamPos = 0U;
				this._pos = 0U;
				this.TrainSize = 0U;
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000630C File Offset: 0x0000450C
		public bool Train(Stream stream)
		{
			long length = stream.Length;
			uint num = (length < (long)((ulong)this._windowSize)) ? ((uint)length) : this._windowSize;
			this.TrainSize = num;
			stream.Position = length - (long)((ulong)num);
			this._streamPos = (this._pos = 0U);
			while (num > 0U)
			{
				uint num2 = this._windowSize - this._pos;
				if (num < num2)
				{
					num2 = num;
				}
				int num3 = stream.Read(this._buffer, (int)this._pos, (int)num2);
				if (num3 == 0)
				{
					return false;
				}
				num -= (uint)num3;
				this._pos += (uint)num3;
				this._streamPos += (uint)num3;
				if (this._pos == this._windowSize)
				{
					this._streamPos = (this._pos = 0U);
				}
			}
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000063CD File Offset: 0x000045CD
		public void ReleaseStream()
		{
			this.Flush();
			this._stream = null;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000063DC File Offset: 0x000045DC
		public void Flush()
		{
			uint num = this._pos - this._streamPos;
			if (num == 0U)
			{
				return;
			}
			this._stream.Write(this._buffer, (int)this._streamPos, (int)num);
			if (this._pos >= this._windowSize)
			{
				this._pos = 0U;
			}
			this._streamPos = this._pos;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006434 File Offset: 0x00004634
		public void CopyBlock(uint distance, uint len)
		{
			uint num = this._pos - distance - 1U;
			if (num >= this._windowSize)
			{
				num += this._windowSize;
			}
			while (len > 0U)
			{
				if (num >= this._windowSize)
				{
					num = 0U;
				}
				byte[] buffer = this._buffer;
				uint pos = this._pos;
				this._pos = pos + 1U;
				buffer[(int)pos] = this._buffer[(int)num++];
				if (this._pos >= this._windowSize)
				{
					this.Flush();
				}
				len -= 1U;
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000064AC File Offset: 0x000046AC
		public void PutByte(byte b)
		{
			byte[] buffer = this._buffer;
			uint pos = this._pos;
			this._pos = pos + 1U;
			buffer[(int)pos] = b;
			if (this._pos >= this._windowSize)
			{
				this.Flush();
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000064E8 File Offset: 0x000046E8
		public byte GetByte(uint distance)
		{
			uint num = this._pos - distance - 1U;
			if (num >= this._windowSize)
			{
				num += this._windowSize;
			}
			return this._buffer[(int)num];
		}

		// Token: 0x0400008F RID: 143
		private byte[] _buffer;

		// Token: 0x04000090 RID: 144
		private uint _pos;

		// Token: 0x04000091 RID: 145
		private uint _windowSize;

		// Token: 0x04000092 RID: 146
		private uint _streamPos;

		// Token: 0x04000093 RID: 147
		private Stream _stream;

		// Token: 0x04000094 RID: 148
		public uint TrainSize;
	}
}
