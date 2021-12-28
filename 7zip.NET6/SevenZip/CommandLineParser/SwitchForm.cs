using System;

namespace SevenZip.CommandLineParser
{
	// Token: 0x02000024 RID: 36
	public class SwitchForm
	{
		// Token: 0x060000EB RID: 235 RVA: 0x00009284 File Offset: 0x00007484
		public SwitchForm(string idString, SwitchType type, bool multi, int minLen, int maxLen, string postCharSet)
		{
			this.IDString = idString;
			this.Type = type;
			this.Multi = multi;
			this.MinLen = minLen;
			this.MaxLen = maxLen;
			this.PostCharSet = postCharSet;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000092B9 File Offset: 0x000074B9
		public SwitchForm(string idString, SwitchType type, bool multi, int minLen) : this(idString, type, multi, minLen, 0, "")
		{
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000092CC File Offset: 0x000074CC
		public SwitchForm(string idString, SwitchType type, bool multi) : this(idString, type, multi, 0)
		{
		}

		// Token: 0x0400010E RID: 270
		public string IDString;

		// Token: 0x0400010F RID: 271
		public SwitchType Type;

		// Token: 0x04000110 RID: 272
		public bool Multi;

		// Token: 0x04000111 RID: 273
		public int MinLen;

		// Token: 0x04000112 RID: 274
		public int MaxLen;

		// Token: 0x04000113 RID: 275
		public string PostCharSet;
	}
}
