using System;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the user mailbox is in a locked state.<br/>
	/// </summary>
	/// <remarks>
	/// The mail boxes are locked when an existing session is open on the POP3 server.<br/>
	/// Lock conditions are also met in case of aborted sessions.
	/// </remarks>
	public class PopServerLockedException : PopClientException
	{
		///<summary>
		/// Creates a PopServerLockedException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the course of this exception</param>
		public PopServerLockedException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}