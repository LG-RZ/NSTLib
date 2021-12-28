using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x02000019 RID: 25
	internal interface IInWindowStream
	{
		// Token: 0x06000083 RID: 131
		void SetStream(Stream inStream);

		// Token: 0x06000084 RID: 132
		void Init();

		// Token: 0x06000085 RID: 133
		void ReleaseStream();

		// Token: 0x06000086 RID: 134
		byte GetIndexByte(int index);

		// Token: 0x06000087 RID: 135
		uint GetMatchLen(int index, uint distance, uint limit);

		// Token: 0x06000088 RID: 136
		uint GetNumAvailableBytes();
	}
}
