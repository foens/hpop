/*
*Name:			OpenPOP.Utility
*Function:		Utility
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:25 GMT-8
*Description:
*Changes:		
*/
using System;
using System.Text;
using System.IO;
using System.Threading;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Utility functions
	/// </summary>
	public class Utility
	{
		/// <summary>
		/// Weather auto loggin is on or off
		/// </summary>
		private static bool m_blnLog=false;

		/// <summary>
		/// The file name in which the logging will be done
		/// </summary>
		private static string m_strLogFile = "OpenPOP.log";

		/// <summary>
		/// Turns file logging on and off.<font color="red"><h1>Change Property Name</h1></font>
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

		/// <summary>
		/// Log an error to the log file
		/// </summary>
		/// <param name="strText">The error text to log</param>
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

	}
}
