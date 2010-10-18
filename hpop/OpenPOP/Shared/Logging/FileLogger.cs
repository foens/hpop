using System;
using System.IO;

namespace OpenPOP.Shared.Logging
{
	/// <summary>
	/// This logging object writes application error and debug output to a text file.
	/// </summary>
	public class FileLogger : ILog
	{
		#region File Logging
		/// <summary>
		/// The default file to which logging will be done
		/// </summary>
		private const string DefaultLogFile = "OpenPOP.log";

		/// <summary>
		/// Lock object to prevent thread interactions
		/// </summary>
		private static readonly object LogLock = new object();


		/// <summary>
		/// Static constructor
		/// </summary>
		static FileLogger()
		{
			LogFile = new FileInfo(DefaultLogFile);
			Enabled = true;
			Verbose = false;
		}

		/// <summary>
		/// Turns the logging on and off.
		/// </summary>
		public static bool Enabled { get; set; }

		/// <summary>
		/// Enables or disables the output of Debug level log messages
		/// </summary>
		public static bool Verbose { get; set; }

		/// <summary>
		/// The file to which log messages will be written
		/// </summary>
		/// <remarks>This property defaults to OpenPOP.log.</remarks>
		public static FileInfo LogFile { get; set; }

		/// <summary>
		/// Write a message to the log file
		/// </summary>
		/// <param name="text">The error text to log</param>
		private static void LogToFile(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			// We want to open the file and append some text to it
			lock (LogLock)
			{
				using (StreamWriter sw = LogFile.AppendText())
				{
					sw.WriteLine(DateTime.Now + " " + text);
					sw.Flush();
				}
			}
		}
		#endregion

		#region ILog Implementation
		/// <summary>
		/// Logs an error to the logs
		/// </summary>
		/// <param name="message">This is the string to log</param>
		public void LogError(string message)
		{
			if (Enabled)
				LogToFile(message);
		}

		/// <summary>
		/// Logs a debug message to the logs
		/// </summary>
		/// <param name="message">This is the debug message to log</param>
		public void LogDebug(string message)
		{
			if (Enabled && Verbose)
				LogToFile("DEBUG: " + message);
		}
		#endregion
	}
}