using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
	public class SwitchResult
	{
		public SwitchResult()
		{
			this.ThereIs = false;
		}

		public bool ThereIs;

		public bool WithMinus;

		public ArrayList PostStrings = new ArrayList();

		public int PostCharIndex;
	}
}
