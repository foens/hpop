/*
*Name:			OpenPOP.Utility
*Function:		Utility
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:25 GMT+8
*Description:
*Changes:		
*				2004/3/29 12:25 GMT+8 by Unruled Boy
*					1.GetMimeType support for MONO
*					2.cleaning up the names of variants
*/
using System;
using System.Text;
using System.IO;
using System.Threading;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	public class Utility
	{
		private static bool m_blnLog=false;
		private static string m_strLogFile = "OpenPOP.log";

		public Utility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

//		public static string[] SplitText(string strText, string strSplitter)
//		{
//			string []segments=new string[0];
//			int indexOfstrSplitter=strText.IndexOf(strSplitter);
//			if(indexOfstrSplitter!=-1)
//			{
//
//			}
//			return segments;
//		}
//

		public static bool IsPictureFile(string strFile)
		{
			try
			{
				if(strFile!=null&&strFile!="")
				{
					strFile=strFile.ToLower();
					if(strFile.EndsWith(".jpg")||strFile.EndsWith(".bmp")||strFile.EndsWith(".ico")||strFile.EndsWith(".gif")||strFile.EndsWith(".png"))
						return true;
					else
						return false;
				}
				else
					return false;
			}
			catch
			{
				return false;
			}
		}

		public static string ParseEmailDate(string strDate)
		{
			string strRet=strDate.Trim();
			int indexOfTag=strRet.IndexOf(",");
			if(indexOfTag!=-1)
			{
				strRet=strRet.Substring(indexOfTag+1);
			}

			strRet=QuoteText(strRet,"+");
			strRet=QuoteText(strRet,"-");
			strRet=QuoteText(strRet,"GMT");
			return strRet.Trim();
		}

		public static string QuoteText(string strText, string strTag)
		{
			int indexOfTag=strText.IndexOf(strTag);
			if(indexOfTag!=-1)
				return strText.Substring(0,indexOfTag-1);
			else
				return strText;
		}

		public static bool ParseEmailAddress(string strEmailAddress,ref string strUser, ref string strAddress)
		{
			int indexOfAB=strEmailAddress.Trim().LastIndexOf("<");
			int indexOfEndAB=strEmailAddress.Trim().LastIndexOf(">");
			strUser=strEmailAddress;
			strAddress=strEmailAddress;
			if(indexOfAB>=0&&indexOfEndAB>=0)
			{
				if(indexOfAB>0)
				{
					strUser=strUser.Substring(0,indexOfAB-1);
//					strUser=strUser.Substring(0,indexOfAB-1).Trim('\"');
//					if(strUser.IndexOf("\"")>=0)
//					{
//						strUser=strUser.Substring(1,strUser.Length-1);
//					}
				}
				strUser=strUser.Trim();
				strUser=strUser.Trim('\"');
				strAddress=strAddress.Substring(indexOfAB+1,indexOfEndAB-(indexOfAB+1));
			}
			strUser=strUser.Trim();
			strUser=deCode(strUser);
			strAddress=strAddress.Trim();

			return true;
		}

		public static bool SaveByteContentToFile(byte[] content,string strFile)
		{
			try
			{
				if(File.Exists(strFile))
					File.Delete(strFile);
				FileStream fs=File.Create(strFile);
				fs.Write(content,0,content.Length);
				fs.Close();
				return true;
			}
			catch(Exception e)
			{
				Utility.LogError("SaveByteContentToFile():"+e.Message);
				return false;
			}
		}

		/// <summary>
		/// Sepearte header name and header value
		/// </summary>
		/// <param name="RawHeader"></param>
		/// <returns></returns>
		public static string[] getHeadersValue(string RawHeader)
		{
			if(RawHeader==null)
				throw new ArgumentNullException("RawHeader","Argument was null");

			string []array=new String[2]{"",""};
			int indexOfColon=RawHeader.IndexOf(":");			

			try
			{
				array[0]=RawHeader.Substring(0,indexOfColon).Trim();
				array[1]=RawHeader.Substring(indexOfColon+1).Trim();
			}
			catch(Exception){}

			return array;
		}

		public static string GetQuotedValue(string strText, string strSplitter, string strTag)
		{
			if(strText==null)
				throw new ArgumentNullException("strText","Argument was null");

			string []array=new String[2]{"",""};
			int indexOfstrSplitter=strText.IndexOf(strSplitter);			

			try
			{
				array[0]=strText.Substring(0,indexOfstrSplitter).Trim();
				array[1]=strText.Substring(indexOfstrSplitter+1).Trim();
				int pos=array[1].IndexOf("\"");
				if(pos!=-1)
				{
					int pos2=array[1].IndexOf("\"",pos+1);
					array[1]=array[1].Substring(pos+1,pos2-pos-1);
				}
			}
			catch(Exception){}

			//return array;
			if(array[0].ToLower()==strTag.ToLower())
				return array[1].Trim();
			else
				return null;
		}

		public static string Change(string strText,string charset)
		{
			if (charset==null || charset=="")
				return strText;
			byte[] b=Encoding.Default.GetBytes(strText);
			return new string(Encoding.GetEncoding(charset).GetChars(b));
		}

		public static string RemoveNonB64(string strBuffer)
		{
			return strBuffer.Replace("\0","");
		}

		public static string RemoveWhiteBlanks(string strBuffer)
		{
			return strBuffer.Replace("\0","").Replace("\r\n","");
		}

		public static string RemoveQuote(string strBuffer)			
		{
			string strRet=strBuffer;
			if(strRet.StartsWith("\""))
				strRet=strRet.Substring(1);
			if(strRet.EndsWith("\""))
				strRet=strRet.Substring(0,strRet.Length-1);
			return strRet;
		}

		public static string deCodeLine(string strSrc)
		{
			return deCode(RemoveWhiteBlanks(strSrc));
		}

		public static String deCode(String strSrc)
		{
			try
			{
				if(strSrc==null)
					return null;
				int start=strSrc.IndexOf("=?GB2312?");
				if (start==-1)
				{
					start=strSrc.IndexOf("=?gb2312?");
				}
				if(start>=0)
				{
					String strHead=strSrc.Substring(0,start);
					String strMethod=strSrc.Substring(start+9,1);
					strSrc=strSrc.Substring(start+11);
					int end=strSrc.IndexOf("?=");
					if (end==-1)
					{
						end=strSrc.Length;
					}
					String strFoot=strSrc.Substring(end+2,strSrc.Length-end-2);
					strSrc=strSrc.Substring(0,end);
					if(strMethod.ToUpper()=="B")
						strSrc=strHead+deCodeB64s(strSrc)+strFoot;
					else
					{
						if(strMethod.ToUpper()=="Q")
							strSrc=strHead+DecodeQP.ConvertHexContent(strSrc)+strFoot;
						else
							strSrc=strHead+strSrc+strFoot;
					}
					start=strSrc.IndexOf("=?GB2312?");
					if(start==-1)
					{
						start=strSrc.IndexOf("=?gb2312?");
					}
					if(start>=0)
						strSrc=deCode(strSrc);
				}
				else
				{
					strSrc=QuotedCoding.Decode(strSrc);
				}
			}
			catch
			{
			}

			return strSrc;
		}
		
		public static String deCodeB64s(String strSrc)
		{
			return Encoding.Default.GetString(deCodeB64(strSrc));
		}
		
		private static byte []deCodeB64(String strSrc)
		{
			byte[] by=null;
			try
			{ 
				if(strSrc!="")
				{
					by=Convert.FromBase64String(strSrc); 
					//strSrc=Encoding.Default.GetString(by);
				}
			} 
			catch(Exception e) 
			{
				by=Encoding.Default.GetBytes("\0");
				LogError("deCodeB64():"+e.Message);
			}
			return by;
		}

		/// <summary>
		/// Turns file logging on and off.
		/// </summary>
		/// <remarks>Comming soon.</remarks>
		public static bool Log
		{
			get
			{
				return m_blnLog;
			}
			set
			{
				m_blnLog = value;
			}
		}

		internal static void LogError(string strText) 
		{
			//Log=true;
			if(Log)
			{
				FileInfo file = null;
				FileStream fs = null;
				StreamWriter sw = null;
				try
				{
					file = new FileInfo(m_strLogFile);
					sw = file.AppendText();
					//fs = new FileStream(m_strLogFile, FileMode.OpenOrCreate, FileAccess.Write);
					//sw = new StreamWriter(fs);
					sw.WriteLine(DateTime.Now);
					sw.WriteLine(strText);
					sw.WriteLine("\r\n");
					sw.Flush();
				}
				finally
				{
					if(sw != null)
					{
						sw.Close();
						sw = null;
					}
					if(fs != null)
					{
						fs.Close();
						fs = null;
					}
					
				}
			}
		}

		public static bool IsQuotedPrintable(string strText)
		{
			if(strText!=null)
				return (strText.ToLower()=="quoted-printable".ToLower());
			else
				return false;
		}

		public static bool IsBase64(string strText)
		{
			if(strText!=null)
				return (strText.ToLower()=="base64".ToLower());
			else
				return false;
		}

		public static string[] splitOnSemiColon(string objString)
		{
			if(objString==null)
				throw new ArgumentNullException("RawHeader","Argument was null");

			string []array=null;
			int indexOfColon=objString.IndexOf(";");			

			if(indexOfColon<0)
			{
				array=new String[1];
				array[0]=objString;
				return array;
			}
			else
			{
				array=new String[2];
			}

			try
			{
				array[0]=objString.Substring(0,indexOfColon).Trim();
				array[1]=objString.Substring(indexOfColon+1).Trim();
			}
			catch(Exception){}

			return array;
		}

		public static bool IsNotNullText(string strText)
		{
			try
			{
				return (strText!=null&&strText!="");
			}
			catch
			{
				return false;
			}
		}

		public static bool IsNotNullTextEx(string strText)
		{
			try
			{
				return (strText!=null&&strText.Trim()!="");
			}
			catch
			{
				return false;
			}
		}

		public static bool IsOrNullTextEx(string strText)
		{
			try
			{
				return (strText==null||strText.Trim()=="");
			}
			catch
			{
				return false;
			}
		}

	}
}
