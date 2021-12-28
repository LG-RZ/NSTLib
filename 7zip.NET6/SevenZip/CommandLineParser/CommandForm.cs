using System;

namespace SevenZip.CommandLineParser
{
	// Token: 0x02000027 RID: 39
	public class CommandForm
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x000096FA File Offset: 0x000078FA
		public CommandForm(string idString, bool postStringMode)
		{
			this.IDString = idString;
			this.PostStringMode = postStringMode;
		}

		// Token: 0x0400011E RID: 286
		public string IDString = "";

		// Token: 0x0400011F RID: 287
		public bool PostStringMode;
	}
}
