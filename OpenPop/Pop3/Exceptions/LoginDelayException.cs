using System;
using System.Runtime.Serialization;

namespace OpenPop.Pop3.Exceptions
{
	/// <summary>
	/// This exception indicates that the user has logged in recently and
	/// will not be allowed to login again until the login delay period has expired.
	/// Check the parameter to the LOGIN-DELAY capability, that the server responds with when
	/// <see cref="Pop3Client.Capabilities()"/> is called, to see what the delay is.
	/// </summary>
	[Serializable]
	public class LoginDelayException : PopClientException
	{
		///<summary>
		/// Creates a LoginDelayException with the given inner exception
		///</summary>
		///<param name="innerException">The exception that is the cause of this exception</param>
		public LoginDelayException(PopServerException innerException)
			: base("Login denied because of recent connection to this maildrop. Increase time between connections.", innerException)
		{ }

		/// <summary>
		/// Creates a new instance of the LoginDelayException class with serialized data.
		/// </summary>
		/// <param name="info">holds the serialized object data about the exception being thrown</param>
		/// <param name="context">contains contextual information about the source or destination</param>
		protected LoginDelayException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}