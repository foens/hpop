using System;

namespace OpenPOP.POP3.Exceptions
{
	/// <summary>
	/// Thrown when either the login or the password is invalid on the POP3 Server
	/// </summary>
	/// <remarks>Should be used only when using APOP Authentication Method</remarks>
	public class InvalidLoginOrPasswordException : PopClientException
	{
		///<summary>
		/// Creates a InvalidLoginOrPasswordException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the course of this exception</param>
		public InvalidLoginOrPasswordException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}