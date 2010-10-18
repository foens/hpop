using System;

namespace OpenPOP.POP3.Exceptions
{
	/// <summary>
	/// Thrown when the specified POP3 Server can not be found or connected with
	/// </summary>	
	public class PopServerNotFoundException : PopClientException
	{
		///<summary>
		/// Creates a PopServerNotFoundException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the course of this exception</param>
		public PopServerNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	}
}