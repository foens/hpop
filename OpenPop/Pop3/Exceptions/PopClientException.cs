using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// This is the base exception for all <see cref="Pop3Client"/> exceptions.
	/// </summary>
	[Serializable]
	public abstract class PopClientException : Exception
	{
		/// <summary>
		///  Creates a new instance of the PopClientException class
		/// </summary>
		protected PopClientException()
		{
		}

		///<summary>
		/// Creates a PopClientException with the given message and InnerException
		///</summary>
		///<param name="message">The message to include in the exception</param>
		///<param name="innerException">The exception that is the cause of this exception</param>
		protected PopClientException(string message, Exception innerException)
			: base(message, innerException)
		{
			if(message == null)
				throw new ArgumentNullException("message");

			if(innerException == null)
				throw new ArgumentNullException("innerException");
		}

		///<summary>
		/// Creates a PopClientException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		protected PopClientException(string message)
			: base(message)
		{
			if (message == null)
				throw new ArgumentNullException("message");
		}

		/// <summary>
		/// Creates a new instance of the PopClientException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected PopClientException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}