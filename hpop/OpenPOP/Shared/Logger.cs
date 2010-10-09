using System;
using System.IO;

namespace OpenPOP.Shared
{
	/// <summary>
	/// Logger which can be used for debugging purposes
	/// </summary>
	[Obsolete("This class has been replace by the FileLogger object")]
	public static class Logger
	{
		/// <summary>
		/// Turns the internal file logging on and off.
		/// </summary>
		public static bool Log
		{
			get { return FileLogger.Enabled; }
			set { FileLogger.Enabled = value; }
		}

		/// <summary>
		/// The file to which log messages will be written
		/// </summary>
		/// <remarks>This property defaults to OpenPOP.log.</remarks>
		public static string LogFile
		{
			get { return FileLogger.LogFile.FullName; }
			set { FileLogger.LogFile = new FileInfo(value); }
		}
	}
}