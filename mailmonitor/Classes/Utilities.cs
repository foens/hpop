using System;
using System.Runtime.InteropServices;

namespace MailMonitor
{
	public class Utilities
	{
		[DllImport("kernel32")]
		private static extern int Beep(int dwFreq, int dwDuration);

		private struct HTMLTag
		{
			internal string strPrefix;
			internal string strSuffix;
		}
		private static HTMLTag[] _htmlTags=new HTMLTag[4];
		private static string[] _htmlTagText={"<html", "</html>", "<body", "</body>", "<p", "</p>", "<div", "</div>"};


		public Utilities()
		{
		}

		public static string ToFormattedHTML(string strHTML)
		{
			for(int i=0; i<_htmlTagText.Length;i+=2)
			{
				_htmlTags[(int)((i + 1) / 2)].strPrefix = _htmlTagText[i];
				_htmlTags[(int)((i + 1) / 2)].strSuffix = _htmlTagText[i + 1];
			}

			string strRet= strHTML;

			if(!IsHTML(strRet))
				strRet = strRet.Replace("\r\n", "<br>\r\n");

			return strRet;
		}

		public static bool IsHTML(string strHTML)
		{
			string strRet = strHTML.ToLower();
			bool blnRet= false;

			for(int i = 0; i<_htmlTags.Length; i++)
			{            
				if(strRet.IndexOf(_htmlTags[i].strPrefix)!=-1 && strRet.IndexOf(_htmlTags[i].strSuffix)!=-1)
				{
					blnRet = true;
					break;
				}
			}
			return blnRet;
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
