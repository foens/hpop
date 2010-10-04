using System;
using System.IO;

namespace OpenPOP.Shared
{
	/// <summary>
	/// Logger which can be used for debugging purposes
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// The default file to which logging will be done
		/// </summary>
		private const string DefaultLogFile = "OpenPOP.log";

		/// <summary>
		/// Static constructor
		/// </summary>
		static Logger()
		{
			LogFile = DefaultLogFile;
		}

		/// <summary>
		/// Turns the internal file logging on and off.
		/// </summary>
		public static bool Log { get; set; }

		/// <summary>
		/// The file to which log messages will be written
		/// </summary>
		/// <remarks>This property defaults to OpenPOP.log.</remarks>
		public static string LogFile { get; set; }

		/// <summary>
		/// Log an error to the log file
		/// </summary>
		/// <param name="toLog">The error text to log</param>
		internal static void LogError(string toLog)
		{
			// Should this be logged?
			if (Log)
			{
				// We want to open the file and append some text to it
				FileInfo file = new FileInfo(LogFile);
				using (StreamWriter sw = file.AppendText())
				{
					sw.WriteLine(DateTime.Now + toLog);
					sw.Flush();
				}
			}
		}
	}
}