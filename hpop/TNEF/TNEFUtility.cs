using System;
using System.IO;

namespace COM.NET.Mail.TNEF
{
	/// <summary>
	/// Log 的摘要说明。
	/// </summary>
	public class TNEFUtility
	{
		private static bool m_bLog=true;
		internal static string m_strLogFile = @"c:\temp\COM.NET.Mail.TNEF.log";

		public TNEFUtility()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
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
					sw.WriteLine(DateTime.Now+":"+text);//+"\r\n");
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

	}
}
