using OpenPOP.POP3;

namespace OpenPOP.Shared
{
	/// <summary>
	/// Default logger used when no logger is specified.
	/// The logger simply logs to a static logger, which can be turned on or off
	/// without knowing to this object
	/// </summary>
	public class DefaultLogger : ILog
	{
		public void LogError(string message)
		{
			Logger.LogError(message);
		}

		public void LogDebug(string message)
		{
			Logger.LogError("DEBUG: " + message);
		}
	}
}