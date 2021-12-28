using System;

namespace SevenZip.Compression.LZ
{
	// Token: 0x0200001A RID: 26
	internal interface IMatchFinder : IInWindowStream
	{
		// Token: 0x06000089 RID: 137
		void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter);

		// Token: 0x0600008A RID: 138
		uint GetMatches(uint[] distances);

		// Token: 0x0600008B RID: 139
		void Skip(uint num);
	}
}
