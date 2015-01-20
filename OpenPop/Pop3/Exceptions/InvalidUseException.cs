using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the <see cref="Pop3Client"/> is being used in an invalid way.<br/>
	/// This could for example happen if a someone tries to fetch a message without authenticating.
	/// </summary>
	[Serializable]
	public class InvalidUseException : PopClientException
	{
		///<summary>
		/// Creates a InvalidUseException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		public InvalidUseException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Creates a new instance of the InvalidUseException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected InvalidUseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}