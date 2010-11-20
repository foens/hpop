namespace OpenPop.Shared.Logging
{
	/// <summary>
	/// This is the log that all logging will go trough.
	/// </summary>
	public static class DefaultLogger
	{
		/// <summary>
		/// This is the logger used by all logging methods in the assembly. 
		/// You can override this if you want, to move logging to one of your own
		/// logging implementations.
		/// By default a <see cref="DiagnosticsLogger"/> is used.
		/// </summary>
		public static ILog Log = new DiagnosticsLogger();
	}
}