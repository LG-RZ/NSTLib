using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
	// Token: 0x02000025 RID: 37
	public class SwitchResult
	{
		// Token: 0x060000EE RID: 238 RVA: 0x000092D8 File Offset: 0x000074D8
		public SwitchResult()
		{
			this.ThereIs = false;
		}

		// Token: 0x04000114 RID: 276
		public bool ThereIs;

		// Token: 0x04000115 RID: 277
		public bool WithMinus;

		// Token: 0x04000116 RID: 278
		public ArrayList PostStrings = new ArrayList();

		// Token: 0x04000117 RID: 279
		public int PostCharIndex;
	}
}
