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