namespace OpenPOP.POP3.Exceptions
{
	/// <summary>
	/// Thrown when the <see cref="POPClient"/> is being used in an invalid way
	/// </summary>
	public class InvalidUseException : PopClientException
	{
		///<summary>
		/// Creates a InvalidUseException with the given message
		///</summary>
		///<param name="message">The message to include in the exception</param>
		public InvalidUseException(string message)
			: base(message)
		{ }
	}
}