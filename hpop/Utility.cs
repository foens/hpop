using System;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace OpenPOP
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	public class Utility
	{
		private const string tag_Content_Transfer_Encoding="Content-Transfer-Encoding";
		private static bool m_bLog=false;
		internal static string m_strLogFile = "OpenPOP.log";

		public Utility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

//		public static string[] SplitText(string text, string splitter)
//		{
//			string []segments=new string[0];
//			int indexOfSplitter=text.IndexOf(splitter);
//			if(indexOfSplitter!=-1)
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

		public static string GetQuotedValue(string text, string splitter, string tag)
		{
			if(text==null)
				throw new ArgumentNullException("text","Argument was null");

			string []array=new String[2]{"",""};
			int indexOfSplitter=text.IndexOf(splitter);			

			try
			{
				array[0]=text.Substring(0,indexOfSplitter).Trim();
				array[1]=text.Substring(indexOfSplitter+1).Trim();
				int pos=array[1].IndexOf("\"");
				if(pos!=-1)
				{
					int pos2=array[1].IndexOf("\"",pos+1);
					array[1]=array[1].Substring(pos+1,pos2-pos-1);
				}
			}
			catch(Exception){}

			//return array;
			if(array[0].ToLower()==tag.ToLower())
				return array[1].Trim();
			else
				return null;
		}

		public static string Change(string text,string charset)
		{
			if (charset==null || charset=="")
				return text;
			byte[] b=Encoding.Default.GetBytes(text);
			return new string(Encoding.GetEncoding(charset).GetChars(b));
		}

		public static string GetContentTransferEncoding(string buffer, int pos)
		{
			int begin=0,end=0;
			begin=buffer.ToLower().IndexOf(tag_Content_Transfer_Encoding.ToLower(),pos);
			if(begin!=-1)
			{
				end=buffer.ToLower().IndexOf("\r\n".ToLower(),begin+1);
				return buffer.Substring(begin+tag_Content_Transfer_Encoding.Length+1,end-begin-tag_Content_Transfer_Encoding.Length).Trim();
			}
			else
				return "";
		}

		public static string RemoveNonB64(string buffer)
		{
			return buffer.Replace("\0","");
		}

		public static string RemoveWhiteBlanks(string buffer)
		{
			return buffer.Replace("\0","").Replace("\r\n","");
		}

		public static string RemoveQuote(string buffer)			
		{
			string strRet=buffer;
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
							strSrc=strHead+POP3.DecodeQP.ConvertHexContent(strSrc)+strFoot;
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
					strSrc=POP3.QuotedCoding.Decode(strSrc);
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
				return m_bLog;
			}
			set
			{
				m_bLog = value;
			}
		}

		internal static void LogError(string text) 
		{
			Log=false;
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
					sw.WriteLine(text);
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

		/// <summary>Returns the MIME content-type for the supplied file extension</summary>
		/// <returns>String MIME type (Example: \"text/plain\")</returns>
		public static string getMimeType(string fileName)
		{
			try
			{
				string fileExtension=new FileInfo(fileName).Extension;
				RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(fileExtension);
				string contentType = (string)extKey.GetValue("Content Type");

				if (contentType.ToString() != null)
				{	
					return contentType.ToString(); 
				}
				else
				{ return "application/octet-stream"; }
			}
			catch(System.Exception)
			{ return "application/octet-stream"; }
		}

	}
}
