using System;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the password supplied for the username is not accepted by the POP3 server.
	/// </summary>	
	/// <remarks>Should be used only when using <see cref="AuthenticationMethod.UsernameAndPassword"/>.</remarks>
	public class InvalidPasswordException : PopClientException
	{
		///<summary>
		/// Creates a InvalidPasswordException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the course of this exception</param>
		public InvalidPasswordException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}