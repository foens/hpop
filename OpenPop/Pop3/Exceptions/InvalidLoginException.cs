using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the supplied username or password is not accepted by the POP3 server.
	/// </summary>
	[Serializable]
	public class InvalidLoginException : PopClientException
	{
		///<summary>
		/// Creates a InvalidLoginException with the given message and InnerException
		///</summary>
		///<param name="innerException">The exception that is the cause of this exception</param>
		public InvalidLoginException(Exception innerException)
			: base("Server did not accept user credentials", innerException)
		{ }

		/// <summary>
		/// Creates a new instance of the InvalidLoginException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected InvalidLoginException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}