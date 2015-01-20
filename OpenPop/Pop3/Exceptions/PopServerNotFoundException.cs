using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the specified POP3 server can not be found or connected to.
	/// </summary>	
	[Serializable]
	public class PopServerNotFoundException : PopClientException
	{
		///<summary>
		/// Creates a PopServerNotFoundException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the cause of this exception</param>
		public PopServerNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Creates a new instance of the PopServerNotFoundException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected PopServerNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}