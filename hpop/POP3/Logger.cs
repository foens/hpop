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

using System;
using System.IO;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Logger which can be used for debugging purposes
	/// </summary>
	public static class Logger
	{
	    /// <summary>
		/// The file to which logging will be done
		/// </summary>
		private const string logFile = "OpenPOP.log";

	    /// <summary>
	    /// Turns file logging on and off.
	    /// </summary>
	    public static bool Log { get; set; }

	    /// <summary>
		/// Log an error to the log file
		/// </summary>
		/// <param name="toLog">The error text to log</param>
		internal static void LogError(string toLog) 
		{
			// Should this be logged?
			if(Log)
			{
			    StreamWriter sw = null;
				try
				{
                    // We want to open the file and append some text to it
					FileInfo file = new FileInfo(logFile);
					sw = file.AppendText();
					sw.WriteLine(DateTime.Now);
					sw.WriteLine(toLog);
					sw.WriteLine();
					sw.Flush();
				}
				finally
				{
					if(sw != null)
					{
						sw.Close();
					}
				}
			}
		}
	}
}