using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// Thrown when the server does not return "+" to a command.<br/>
	/// The server response is then placed inside.
	/// </summary>
	[Serializable]
	public class PopServerException : PopClientException
	{
		///<summary>
		/// Creates a PopServerException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		public PopServerException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Creates a new instance of the PopServerException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected PopServerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}