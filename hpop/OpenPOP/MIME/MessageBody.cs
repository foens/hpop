using System;

namespace OpenPOP.MIME
{
	/// <summary>
	/// Represents the body of an email message
	/// </summary>
	public class MessageBody
	{
		/// <summary>
		/// The body of this <see cref="MessageBody"/>
		/// </summary>
		public string Body { get; private set; }

		/// <summary>
		/// The type of this <see cref="MessageBody"/>
		/// This can be text/plain, text/html or similar
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		/// Constructs a new MessageBody from to simple strings
		/// </summary>
		/// <param name="body">The body part of the MessageBody</param>
		/// <param name="type">The type of the MessageBody</param>
		/// <exception cref="ArgumentNullException">If body or type is <see langword="null"/></exception>
		public MessageBody(string body, string type)
		{
			if (body == null)
				throw new ArgumentNullException("body");

			if (type == null)
				throw new ArgumentNullException("type");

			Body = body;
			Type = type;
		}
	}
}