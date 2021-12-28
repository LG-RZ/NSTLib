using System;
using System.IO;

namespace SevenZip
{
	// Token: 0x0200000E RID: 14
	public interface ICoder
	{
		// Token: 0x06000052 RID: 82
		void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
	}
}
