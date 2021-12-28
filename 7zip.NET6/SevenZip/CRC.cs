using System;

namespace SevenZip
{
	// Token: 0x0200000A RID: 10
	internal class CRC
	{
		// Token: 0x06000047 RID: 71 RVA: 0x00004B74 File Offset: 0x00002D74
		static CRC()
		{
			for (uint num = 0U; num < 256U; num += 1U)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					if ((num2 & 1U) != 0U)
					{
						num2 = (num2 >> 1 ^ 3988292384U);
					}
					else
					{
						num2 >>= 1;
					}
				}
				CRC.Table[(int)num] = num2;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004BCB File Offset: 0x00002DCB
		public void Init()
		{
			this._value = uint.MaxValue;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004BD4 File Offset: 0x00002DD4
		public void UpdateByte(byte b)
		{
			this._value = (CRC.Table[(int)((byte)this._value ^ b)] ^ this._value >> 8);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004BF4 File Offset: 0x00002DF4
		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint num = 0U; num < size; num += 1U)
			{
				this._value = (CRC.Table[(int)((byte)this._value ^ data[(int)(offset + num)])] ^ this._value >> 8);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004C2F File Offset: 0x00002E2F
		public uint GetDigest()
		{
			return this._value ^ uint.MaxValue;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004C39 File Offset: 0x00002E39
		private static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC crc = new CRC();
			crc.Update(data, offset, size);
			return crc.GetDigest();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004C4E File Offset: 0x00002E4E
		private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return CRC.CalculateDigest(data, offset, size) == digest;
		}

		// Token: 0x04000045 RID: 69
		public static readonly uint[] Table = new uint[256];

		// Token: 0x04000046 RID: 70
		private uint _value = uint.MaxValue;
	}
}
