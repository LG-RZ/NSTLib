﻿using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
	// Token: 0x02000026 RID: 38
	public class Parser
	{
		// Token: 0x060000EF RID: 239 RVA: 0x000092F4 File Offset: 0x000074F4
		public Parser(int numSwitches)
		{
			this._switches = new SwitchResult[numSwitches];
			for (int i = 0; i < numSwitches; i++)
			{
				this._switches[i] = new SwitchResult();
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00009338 File Offset: 0x00007538
		private bool ParseString(string srcString, SwitchForm[] switchForms)
		{
			int length = srcString.Length;
			if (length == 0)
			{
				return false;
			}
			int i = 0;
			if (!Parser.IsItSwitchChar(srcString[i]))
			{
				return false;
			}
			while (i < length)
			{
				if (Parser.IsItSwitchChar(srcString[i]))
				{
					i++;
				}
				int num = 0;
				int num2 = -1;
				for (int j = 0; j < this._switches.Length; j++)
				{
					int length2 = switchForms[j].IDString.Length;
					if (length2 > num2 && i + length2 <= length && string.Compare(switchForms[j].IDString, 0, srcString, i, length2, true) == 0)
					{
						num = j;
						num2 = length2;
					}
				}
				if (num2 == -1)
				{
					throw new Exception("maxLen == kNoLen");
				}
				SwitchResult switchResult = this._switches[num];
				SwitchForm switchForm = switchForms[num];
				if (!switchForm.Multi && switchResult.ThereIs)
				{
					throw new Exception("switch must be single");
				}
				switchResult.ThereIs = true;
				i += num2;
				int num3 = length - i;
				SwitchType type = switchForm.Type;
				switch (type)
				{
				case SwitchType.PostMinus:
					if (num3 == 0)
					{
						switchResult.WithMinus = false;
					}
					else
					{
						switchResult.WithMinus = (srcString[i] == '-');
						if (switchResult.WithMinus)
						{
							i++;
						}
					}
					break;
				case SwitchType.LimitedPostString:
				case SwitchType.UnLimitedPostString:
				{
					int minLen = switchForm.MinLen;
					if (num3 < minLen)
					{
						throw new Exception("switch is not full");
					}
					if (type == SwitchType.UnLimitedPostString)
					{
						switchResult.PostStrings.Add(srcString.Substring(i));
						return true;
					}
					string text = srcString.Substring(i, minLen);
					i += minLen;
					int num4 = minLen;
					while (num4 < switchForm.MaxLen && i < length)
					{
						char c = srcString[i];
						if (Parser.IsItSwitchChar(c))
						{
							break;
						}
						text += c.ToString();
						num4++;
						i++;
					}
					switchResult.PostStrings.Add(text);
					break;
				}
				case SwitchType.PostChar:
				{
					if (num3 < switchForm.MinLen)
					{
						throw new Exception("switch is not full");
					}
					string postCharSet = switchForm.PostCharSet;
					if (num3 == 0)
					{
						switchResult.PostCharIndex = -1;
					}
					else
					{
						int num5 = postCharSet.IndexOf(srcString[i]);
						if (num5 < 0)
						{
							switchResult.PostCharIndex = -1;
						}
						else
						{
							switchResult.PostCharIndex = num5;
							i++;
						}
					}
					break;
				}
				}
			}
			return true;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00009578 File Offset: 0x00007778
		public void ParseStrings(SwitchForm[] switchForms, string[] commandStrings)
		{
			int num = commandStrings.Length;
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				string text = commandStrings[i];
				if (flag)
				{
					this.NonSwitchStrings.Add(text);
				}
				else if (text == "--")
				{
					flag = true;
				}
				else if (!this.ParseString(text, switchForms))
				{
					this.NonSwitchStrings.Add(text);
				}
			}
		}

		// Token: 0x17000004 RID: 4
		public SwitchResult this[int index]
		{
			get
			{
				return this._switches[index];
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000095E0 File Offset: 0x000077E0
		public static int ParseCommand(CommandForm[] commandForms, string commandString, out string postString)
		{
			for (int i = 0; i < commandForms.Length; i++)
			{
				string idstring = commandForms[i].IDString;
				if (commandForms[i].PostStringMode)
				{
					if (commandString.IndexOf(idstring) == 0)
					{
						postString = commandString.Substring(idstring.Length);
						return i;
					}
				}
				else if (commandString == idstring)
				{
					postString = "";
					return i;
				}
			}
			postString = "";
			return -1;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009644 File Offset: 0x00007844
		private static bool ParseSubCharsCommand(int numForms, CommandSubCharsSet[] forms, string commandString, ArrayList indices)
		{
			indices.Clear();
			int num = 0;
			for (int i = 0; i < numForms; i++)
			{
				CommandSubCharsSet commandSubCharsSet = forms[i];
				int num2 = -1;
				int length = commandSubCharsSet.Chars.Length;
				for (int j = 0; j < length; j++)
				{
					char value = commandSubCharsSet.Chars[j];
					int num3 = commandString.IndexOf(value);
					if (num3 >= 0)
					{
						if (num2 >= 0)
						{
							return false;
						}
						if (commandString.IndexOf(value, num3 + 1) >= 0)
						{
							return false;
						}
						num2 = j;
						num++;
					}
				}
				if (num2 == -1 && !commandSubCharsSet.EmptyAllowed)
				{
					return false;
				}
				indices.Add(num2);
			}
			return num == commandString.Length;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000096EC File Offset: 0x000078EC
		private static bool IsItSwitchChar(char c)
		{
			return c == '-' || c == '/';
		}

		// Token: 0x04000118 RID: 280
		public ArrayList NonSwitchStrings = new ArrayList();

		// Token: 0x04000119 RID: 281
		private SwitchResult[] _switches;

		// Token: 0x0400011A RID: 282
		private const char kSwitchID1 = '-';

		// Token: 0x0400011B RID: 283
		private const char kSwitchID2 = '/';

		// Token: 0x0400011C RID: 284
		private const char kSwitchMinus = '-';

		// Token: 0x0400011D RID: 285
		private const string kStopSwitchParsing = "--";
	}
}
