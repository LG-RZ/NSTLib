using System;

namespace SevenZip.CommandLineParser
{
	public class CommandForm
	{
		public CommandForm(string idString, bool postStringMode)
		{
			this.IDString = idString;
			this.PostStringMode = postStringMode;
		}

		public string IDString = "";

		public bool PostStringMode;
	}
}
