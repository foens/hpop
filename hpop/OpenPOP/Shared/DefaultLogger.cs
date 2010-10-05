namespace OpenPOP.Shared
{
	/// <summary>
	/// Default logger used when no logger is specified.
	/// The logger simply logs to a static logger, which can be turned on or off
	/// without knowing to this object
	/// </summary>
	public class DefaultLogger : ILog
	{
		/// <summary>
		/// Logs an error to the logs
		/// </summary>
		/// <param name="message">This is the string to log</param>
		public void LogError(string message)
		{
			Logger.LogError(message);
		}

		/// <summary>
		/// Logs a debug message to the logs
		/// </summary>
		/// <param name="message">This is the debug message to log</param>
		public void LogDebug(string message)
		{
			Logger.LogError("DEBUG: " + message);
		}
	}
}