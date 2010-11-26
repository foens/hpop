using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using OpenPop.Mime.Decode;
using OpenPop.Mime.Header;
using OpenPop.Common;

namespace OpenPop.Mime
{
	/// <summary>
	/// A MessagePart is a part of an email message used to describe the whole email parse tree.
	/// 
	/// Email messages are tree structures:
	/// Email messages may contain large tree structures, and the MessagePart are the nodes of the this structure.
	/// A MessagePart may either be a leaf in the structure or a internal node with links to other MessageParts.
	/// The root of the message tree is the <see cref="Message"/> class.
	/// 
	/// Leafs:
	/// If a MessagePart is a leaf, the part is not a <see cref="IsMultiPart">MultiPart</see> message.
	/// Leafs are where the contents of an email are placed.
	/// This includes, but is not limited to: attachments, text or images referenced from HTML.
	/// The content of an attachment can be fetched by using the <see cref="Body"/> property.
	/// If you want to have the text version of a MessagePart, use the <see cref="GetBodyAsText"/> method which will
	/// convert the <see cref="Body"/> into a string using the encoding the message was sent with.
	/// 
	/// Internal nodes:
	/// If a MessagePart is an internal node in the email tree structure, then the part is a <see cref="IsMultiPart">MultiPart</see> message.
	/// The <see cref="MessageParts"/> property will then contain links to the parts it contain.
	/// The <see cref="Body"/> property of the MessagePart will not be set.
	/// </summary>
	/// <example>
	/// This example illustrates how the message parse tree looks like given a specific message
	/// 
	/// The message source in this example is:
	/// MIME-Version: 1.0
	///	Content-Type: multipart/mixed; boundary="frontier"
	///	
	///	This is a message with multiple parts in MIME format.
	///	--frontier
	/// Content-Type: text/plain
	///	
	///	This is the body of the message.
	///	--frontier
	///	Content-Type: application/octet-stream
	///	Content-Transfer-Encoding: base64
	///	
	///	PGh0bWw+CiAgPGHLYWQ+CiAgPC9oZWFkPgogIDxib2R5PgogICAgPHA+VGhpcyBpcyB0aGUg
	///	Ym9keSBvZiB0aGUgbWVzc2FnZS48L3A+CiAgPC9ib2R5Pgo8L2h0bWw+Cg==
	///	--frontier--
	/// 
	/// The tree will look as follows, where the content-type media type of the message is listed
	/// - Message root
	///   - multipart/mixed MessagePart
	///     - text/plain MessagePart
	///     - application/octet-stream MessagePart
	/// 
	/// It is possible to have more complex message trees like the following:
	/// - Message root
	///   - multipart/mixed MessagePart
	///     - text/plain MessagePart
	///     - text/plain MessagePart
	///     - multipart/parallel
	///       - audio/basic
	///       - image/tiff
	///     - text/enriched
	///     - message/rfc822
	/// 
	/// But it is also possible to have very simple message trees like:
	/// - Message root
	///   - text/plain
	/// </example>
	public class MessagePart
	{
		#region Public properties
		/// <summary>
		/// The ContentType header field.
		/// If not set, the ContentType is created by the default string
		/// defined in <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a> Section 5.2
		/// which is "text/plain; charset=us-ascii"
		/// </summary>
		public ContentType ContentType { get; private set; }

		/// <summary>
		/// A human readable description of the body
		/// Null if not set
		/// </summary>
		public string ContentDescription { get; private set; }

		/// <summary>
		/// The CONTENT-TRANSFER-ENCODING header field
		/// 
		/// If the header was not found when this object was created, it is set
		/// to the default of 7BIT
		/// </summary>
		/// <remarks>See <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a> Part 6 for details</remarks>
		public ContentTransferEncoding ContentTransferEncoding { get; private set; }

		/// <summary>
		/// ID of the content part (like an attached image). Used with MultiPart messages.
		/// Null if not set
		/// </summary>
		public string ContentId { get; private set; }

		/// <summary>
		/// The ContentDisposition header field
		/// Null if not set
		/// </summary>
		public ContentDisposition ContentDisposition { get; private set; }

		/// <summary>
		/// This is the encoding used to parse the message body if the <see cref="MessagePart"/>
		/// is not a MultiPart message. It is derived from the <see cref="ContentType"/> character set property.
		/// </summary>
		public Encoding BodyEncoding { get; private set; }

		/// <summary>
		/// This is the parsed body of this <see cref="MessagePart"/>.
		/// It is parsed in that way, if the body was ContentTransferEncoded, it has been decoded to the
		/// correct bytes.
		/// It will be <see langword="null"/> if this <see cref="MessagePart"/> is a MultiPart message.
		/// Use <see cref="IsMultiPart"/> to check if this <see cref="MessagePart"/> is a MultiPart message.
		/// </summary>
		public byte[] Body { get; private set; }

		/// <summary>
		/// Describes if this <see cref="MessagePart"/> is a MultiPart message
		/// </summary>
		public bool IsMultiPart
		{
			get
			{
				return ContentType.MediaType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);
			}
		}

		/// <summary>
		/// A <see cref="MessagePart"/> is considered to be holding text in it's body if the MediaType
		/// starts either "text/" or is equal to "message/rfc822"
		/// </summary>
		public bool IsText
		{
			get
			{
				string mediaType = ContentType.MediaType;
				return mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) || mediaType.Equals("message/rfc822", StringComparison.OrdinalIgnoreCase);
			}
		}

		/// <summary>
		/// A <see cref="MessagePart"/> is considered to be an attachment, if
		///  - it is not holding <see cref="IsText">text</see> and is not a <see cref="IsMultiPart">MultiPart</see> message
		///  or
		///  - it has a Content-Disposition header that says it is an attachment
		/// </summary>
		public bool IsAttachment
		{
			get
			{
				// Inline is the opposite of attachment
				return (!IsText && !IsMultiPart) || (ContentDisposition != null && !ContentDisposition.Inline);
			}
		}

		/// <summary>
		/// This is a convenient-property for figuring out a FileName for this <see cref="MessagePart"/>.
		/// If the <see cref="MessagePart"/> is a MultiPart message, then it makes no sense to try to find a FileName.
		/// The FileName can be specified in the <see cref="ContentDisposition"/> or in the <see cref="ContentType"/> properties.
		/// If none of these places two places tells about the FileName, a default "(no name)" is returned.
		/// </summary>
		public string FindFileName
		{
			get
			{
				if (ContentDisposition != null && ContentDisposition.FileName != null)
					return ContentDisposition.FileName;

				if (ContentType.Name != null)
					return ContentType.Name;

				return "(no name)";
			}
		}

		/// <summary>
		/// If this <see cref="MessagePart"/> is a MultiPart message, then this property
		/// has a list of each of the Multiple parts that the message consists of
		/// It is <see langword="null"/> if it is not a MultiPart message.
		/// Use <see cref="IsMultiPart"/> to check if this <see cref="MessagePart"/> is a MultiPart message.
		/// </summary>
		public List<MessagePart> MessageParts { get; private set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Used to construct the topmost message part
		/// </summary>
		/// <param name="rawBody">The body that needs to be parsed</param>
		/// <param name="headers">The headers that should be used from the message</param>
		/// <exception cref="ArgumentNullException">If <paramref name="rawBody"/> or <paramref name="headers"/> is <see langword="null"/></exception>
		internal MessagePart(byte[] rawBody, MessageHeader headers)
		{
			if(rawBody == null)
				throw new ArgumentNullException("rawBody");
			
			if(headers == null)
				throw new ArgumentNullException("headers");

			ContentType = headers.ContentType;
			ContentDescription = headers.ContentDescription;
			ContentTransferEncoding = headers.ContentTransferEncoding;
			ContentId = headers.ContentId;
			ContentDisposition = headers.ContentDisposition;

			BodyEncoding = ParseBodyEncoding(ContentType.CharSet);

			ParseBody(rawBody);
		}
		#endregion

		#region Parsing
		/// <summary>
		/// Parses a character set into an encoding
		/// </summary>
		/// <param name="characterSet">The character set that needs to be parsed. <see langword="null"/> is allowed.</param>
		/// <returns>The encoding specified by the <paramref name="characterSet"/> parameter, or ASCII if the character set was <see langword="null"/> or empty</returns>
		private static Encoding ParseBodyEncoding(string characterSet)
		{
			// Default encoding in Mime messages is US-ASCII
			Encoding encoding = Encoding.ASCII;

			// If the character set was specified, find the encoding that the character
			// set describes, and use that one instead
			if (!string.IsNullOrEmpty(characterSet))
				encoding = HeaderFieldParser.ParseCharsetToEncoding(characterSet);

			return encoding;
		}

		/// <summary>
		/// Parses a byte array as a body of an email message.
		/// </summary>
		/// <param name="rawBody">The byte array to parse as body of an email message. This array may not contain headers.</param>
		private void ParseBody(byte[] rawBody)
		{
			if(IsMultiPart)
			{
				// Parses a MultiPart message
				ParseMultiPartBody(rawBody);
			} else
			{
				// Parses a non MultiPart message
				// Decode the body accodingly and set the Body property
				Body = DecodeBody(rawBody, ContentTransferEncoding);
			}
		}

		/// <summary>
		/// Parses the <paramref name="rawBody"/> byte array as a MultiPart message.
		/// It is not valid to call this method if <see cref="IsMultiPart"/> returned <see langword="false"/>.
		/// Fills the <see cref="MessageParts"/> property of this <see cref="MessagePart"/>.
		/// </summary>
		/// <param name="rawBody">The byte array which is to be parsed as a MultiPart message</param>
		private void ParseMultiPartBody(byte[] rawBody)
		{
			// Fetch out the boundary used to delimit the messages within the body
			string multipartBoundary = ContentType.Boundary;

			// Fetch the individual MultiPart message parts using the MultiPart boundary
			List<byte[]> bodyParts = GetMultiPartParts(rawBody, multipartBoundary);

			// Initialize the MessageParts property, with room to as many bodies as we have found
			MessageParts = new List<MessagePart>(bodyParts.Count);

			// Now parse each byte array as a message body and add it the the MessageParts property
			foreach (byte[] bodyPart in bodyParts)
			{
				MessagePart messagePart = GetMessagePart(bodyPart);
				MessageParts.Add(messagePart);
			}
		}

		/// <summary>
		/// Given a byte array describing a full message.
		/// Parses the byte array into a <see cref="MessagePart"/>.
		/// </summary>
		/// <param name="rawMessageContent">The byte array containing both headers and body of a message</param>
		/// <returns>A <see cref="MessagePart"/> which was described by the <paramref name="rawMessageContent"/> byte array</returns>
		private static MessagePart GetMessagePart(byte[] rawMessageContent)
		{
			// Find the headers and the body parts of the byte array
			MessageHeader headers;
			byte[] body;
			HeaderExtractor.ExtractHeadersAndBody(rawMessageContent, out headers, out body);

			// Create a new MessagePart from the headers and the body
			return new MessagePart(body, headers);
		}

		/// <summary>
		/// Gets a list of byte arrays where each entry in the list is a full message of a message part
		/// </summary>
		/// <param name="rawBody">The raw byte array describing the body of a message which is a MultiPart message</param>
		/// <param name="multipPartBoundary">The delimiter that splits the different MultiPart bodies from each other</param>
		/// <returns>A list of byte arrays, each a full message of a <see cref="MessagePart"/></returns>
		private static List<byte[]> GetMultiPartParts(byte[] rawBody, string multipPartBoundary)
		{
			// This is the list we want to return
			List<byte[]> messageBodies = new List<byte[]>();

			// Create a stream from which we can find MultiPart boundaries
			using (MemoryStream stream = new MemoryStream(rawBody))
			{
				// Find the start of the first message in this multipart
				// Since the method returns the first character on a the line containing the MultiPart boundary, we
				// need to add the MultiPart boundary with prepended "--" and appended CRLF pair to the position returned.
				int startLocation = FindPositionOfNextMultiPartBoundary(stream, multipPartBoundary) + ("--" + multipPartBoundary + "\r\n").Length;
				while (true)
				{
					// Find the end location of the current multipart
					// Since the method returns the first character on a the line containing the MultiPart boundary, we
					// need to go a CRLF pair back, so that we do not get that into the body of the message part
					int stopLocation = FindPositionOfNextMultiPartBoundary(stream, multipPartBoundary) - "\r\n".Length;

					// If no more MultiPart boundaries was found, there are no more message parts and we can stop
					if (stopLocation <= -1)
						break;

					// If we came this this place, it means we have found the start and end of a message part
					// Now we create a byte array with the correct length and put the message part's bytes into
					// it and add it to our list we want to return
					int length = stopLocation - startLocation;
					byte[] messageBody = new byte[length];
					Array.Copy(rawBody, startLocation, messageBody, 0, length);
					messageBodies.Add(messageBody);

					// We want to advance to the next message parts start.
					// We can find this by jumping forward the MultiPart boundary from the last
					// message parts end position
					startLocation = stopLocation + ("\r\n" + "--" + multipPartBoundary + "\r\n").Length;
				}
			}

			// We are done
			return messageBodies;
		}

		/// <summary>
		/// Method that is able to find a specific MultiPart boundary in a Stream.
		/// The Stream passed should not be used for anything else then for looking for MultiPart boundaries
		/// <param name="stream">The stream to find the next MultiPart boundary in. Do not use it for anything else then with this method.</param>
		/// <param name="multipPartBoundary">The MultiPart boundary to look for. This should be found in the <see cref="ContentType"/> header</param>
		/// </summary>
		/// <returns>The position of the first character of the line that contained MultiPartBoundary or -1 if no (more) MultiPart boundaries was found</returns>
		private static int FindPositionOfNextMultiPartBoundary(Stream stream, string multipPartBoundary)
		{
			while(true)
			{
				// Get the current position. This is the first position on the line - no characters of the line will
				// have been read yet
				int currentPos = (int) stream.Position;

				// Read the line
				string line = StreamUtility.ReadLineAsAscii(stream);

				// If we kept reading until there was no more lines, we did not meet
				// the MultiPart boundary. -1 is then returned to describe this.
				if (line == null)
					return -1;

				// The MultiPart boundary is the MultiPartBoundary with "--" in front of it
				// which is to be at the very start of a line
				if (line.StartsWith("--" + multipPartBoundary, StringComparison.Ordinal))
					return currentPos;
			}
		}

		/// <summary>
		/// Decodes a byte array into another byte array based upon the Content Transfer encoding
		/// </summary>
		/// <param name="messageBody">The byte array to decode into another byte array</param>
		/// <param name="contentTransferEncoding">The <see cref="ContentTransferEncoding"/> of the byte array</param>
		/// <returns>A byte array which comes from the <paramref name="contentTransferEncoding"/> being used on the <paramref name="messageBody"/></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="messageBody"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="contentTransferEncoding"/> is unsupported</exception>
		private static byte[] DecodeBody(byte[] messageBody, ContentTransferEncoding contentTransferEncoding)
		{
			if (messageBody == null)
				throw new ArgumentNullException("messageBody");

			switch (contentTransferEncoding)
			{
				case ContentTransferEncoding.QuotedPrintable:
					// If encoded in QuotedPrintable, everything in the body is in US-ASCII
					return QuotedPrintable.Decode(Encoding.ASCII.GetString(messageBody));

				case ContentTransferEncoding.Base64:
					// If encoded in Base64, everything in the body is in US-ASCII
					return Base64.Decode(Encoding.ASCII.GetString(messageBody));

				case ContentTransferEncoding.SevenBit:
				case ContentTransferEncoding.Binary:
				case ContentTransferEncoding.EightBit:
					// We do not have to do anything
					return messageBody;

				default:
					throw new ArgumentOutOfRangeException("contentTransferEncoding");
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Gets this MessagePart's <see cref="Body"/> as text.
		/// This is simply the <see cref="BodyEncoding"/> being used on the raw bytes of the <see cref="Body"/> property
		/// This method is only valid to call if it is not a MultiPart message and therefore contains a body
		/// </summary>
		/// <returns>The <see cref="Body"/> in bytes</returns>
		public string GetBodyAsText()
		{
			return BodyEncoding.GetString(Body);
		}

		/// <summary>
		/// Save this <see cref="MessagePart"/> to a file.
		/// There are no methods to reload the file.
		/// </summary>
		/// <param name="file">The File location to save the <see cref="MessagePart"/> to. Existent files will be overwritten.</param>
		/// <exception cref="ArgumentNullException">If <paramref name="file"/> is <see langword="null"/></exception>
		/// <exception>Other exceptions relevant to file saving might be thrown as well</exception>
		public void SaveToFile(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			File.WriteAllBytes(file.FullName, Body);
		}
		#endregion
	}
}