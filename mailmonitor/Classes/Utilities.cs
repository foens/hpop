using System;
using System.Runtime.InteropServices;

namespace MailMonitor
{
	public class Utilities
	{
		[DllImport("kernel32")]
		private static extern int Beep(int dwFreq, int dwDuration);

		public Utilities()
		{
		}

		public static string ToNormalFileName(string strFile)
		{
			try
			{
				char[] chrItems=new char[9];
				chrItems[0]='/';
				chrItems[1]='\\';
				chrItems[2]=':';
				chrItems[3]='*';
				chrItems[4]='?';
				chrItems[5]='\"';
				chrItems[6]='<';
				chrItems[7]='>';
				chrItems[8]='|';
				strFile=ReplaceChars(strFile,chrItems);
			}
			catch
			{
			}
			return strFile;
		}

		public static string ReplaceChars(string strText, char[] chrItems)
		{
			for(int i=0;i<chrItems.Length;i++)
			{
				strText=strText.Replace(chrItems[i],' ');
			}
			return strText;
		}

		public static void BeepIt()
		{
			Beep(500,50);
		}
	}
}
