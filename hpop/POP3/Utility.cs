/******************************************************************************
	Copyright 2003-2004 Hamid Qureshi and Unruled Boy 
	OpenPOP.Net is free software; you can redistribute it and/or modify
	it under the terms of the Lesser GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	OpenPOP.Net is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	Lesser GNU General Public License for more details.

	You should have received a copy of the Lesser GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/

/*
*Name:			OpenPOP.Utility
*Function:		Utility
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		3 May 2004 0200 GMT+5 by Hamid Qureshi
*Description:
*Changes:		3rd May 1600 GMT+5 by Hamid Qureshi
*					1.Adding NDoc Comments
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

