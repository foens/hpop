namespace OpenPOP.Shared.Logging
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
		/// Delegate used to create a logger
		/// </summary>
		/// <returns>A new Logger to use for logging</returns>
		public delegate ILog DefaultLoggerFactory();

		/// <summary>
		/// The factory object to use to create the default logger for the system
		/// </summary>
		public static DefaultLoggerFactory LoggerFactory = CreateDiagnosticsLogger;

		// This would be much nicer under C# 3.0 using an anonymous delegate
		// instead of a defined delegate method
		// TODO Is delegates not supported in C# 2.0? : http://msdn.microsoft.com/en-us/library/orm-9780596516109-03-09.aspx
		private static ILog CreateDiagnosticsLogger()
		{
			return new DiagnosticsLogger();
		}

		/// <summary>
		/// Creates an instance object of the default logging interface
		/// </summary>
		/// <returns>An <see cref="ILog"/> interface object</returns>
		public static ILog CreateLogger()
		{
			return LoggerFactory();
		}
	}
}