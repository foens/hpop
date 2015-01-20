using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the POP3 server sends an error "-ERR" during initial handshake "HELO".
	/// </summary>	
	[Serializable]
	public class PopServerNotAvailableException : PopClientException
	{
		///<summary>
		/// Creates a PopServerNotAvailableException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the cause of this exception</param>
		public PopServerNotAvailableException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Creates a new instance of the PopServerNotAvailableException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected PopServerNotAvailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}