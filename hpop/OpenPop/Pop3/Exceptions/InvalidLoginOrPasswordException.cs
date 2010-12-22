using System;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when either the username or the password is not accepted by the POP3 server.
	/// </summary>
	/// <remarks>Should be used only when using <see cref="AuthenticationMethod.APOP"/>.</remarks>
	public class InvalidLoginOrPasswordException : PopClientException
	{
		///<summary>
		/// Creates a InvalidLoginOrPasswordException with the given message and inner exception
		///</summary>
		///<param name="innerException">The exception that is the course of this exception</param>
		public InvalidLoginOrPasswordException(Exception innerException)
			: base("The supplied username or password are wrong", innerException)
		{ }
	}
}