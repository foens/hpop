using System;
using System.IO;

namespace OpenPOP.Shared
{
	/// <summary>
	/// Class used for handling the instance creation of the default ILog logging interface
	/// </summary>
	/// <remarks>If the application wishes to override the default logger used by the system, then a factory
	/// delegate is passed to the class using the <see cref="LoggerFactory"/> method.
	/// This class, by default, returns an instance of a <see cref="DiagnosticsLogger"/> object.</remarks>
	public static class DefaultLogger
	{
		/// <summary>
		/// Delegate used to create the 
		/// </summary>
		/// <returns></returns>
		public delegate ILog DefaultLoggerFactory( );

		/// <summary>
		/// The factory object to use to create the default logger for the system
		/// </summary>
		public static DefaultLoggerFactory LoggerFactory;

		static DefaultLogger()
		{
			LoggerFactory = CreateDiagnosticsLogger;
		}

		// This would be much nicer under C# 3.0 using an anonymous delegate
		// instead of a defined delegate method
		private static ILog CreateDiagnosticsLogger()
		{
			return new DiagnosticsLogger( );
		}

		/// <summary>
		/// Creates an instance object of the default logging interface
		/// </summary>
		/// <returns>An <see cref="ILog"/> interface object</returns>
		public static ILog CreateLogger( )
		{
			return LoggerFactory();
		}
	}

	/// <summary>
	/// This logging object writes aplication error and debug output using the
	/// <see cref="System.Diagnostics.Trace"/> facilities.
	/// </summary>
	public class DiagnosticsLogger : ILog
	{
		/// <summary>
		/// Logs an error to the System Trace facility
		/// </summary>
		/// <param name="message">This is the string to log</param>
		public void LogError(string message)
		{
			System.Diagnostics.Trace.WriteLine("OpenPOP: " + message);
		}

		/// <summary>
		/// Logs a debug message to the system Trace Facility
		/// </summary>
		/// <param name="message">This is the debug message to log</param>
		public void LogDebug(string message)
		{
			System.Diagnostics.Trace.WriteLine("OpenPOP: (DEBUG) " + message);
		}
	}

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
		private static readonly object LogLock = new object(  );


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
		internal static void LogToFile(string text)
		{
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