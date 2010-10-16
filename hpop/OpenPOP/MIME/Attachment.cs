using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME
{
	///<summary>
	/// This class represents an Attachment to an email message.
	///</summary>
	public class Attachment : IComparable<Attachment>
	{
		#region Member Variables
		private const string DefaultMIMEFileName = "body.eml";
		private const string DefaultReportFileName = "report.htm";
		private const string DefaultFileName = "body.htm";
		#endregion

		#region Properties
		/// <summary>
		/// Headers for this Attachment
		/// </summary>
		public MessageHeader Headers { get; private set; }

		/// <summary>
		/// Content File Name
		/// </summary>
		public string ContentFileName { get; private set; }

		/// <summary>
		/// Raw Content
		/// Full Attachment, with headers and everything.
		/// The raw string used to create this attachment
		/// </summary>
		public string RawContent { get; private set; }

		/// <summary>
		/// Raw Attachment Content (headers removed if was specified at creation)
		/// </summary>
		public string RawAttachment { get; private set; }

		/// <summary>
		/// The logging interface used by the object
		/// </summary>
		private ILog Log { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Used to create a new attachment internally to avoid any
		/// duplicate code for setting up an attachment
		/// </summary>
		/// <param name="fileName">Sets the attachment file name to the supplied argument</param>
		/// <param name="logger">The logging interface to be used by the object</param>
		private Attachment(string fileName, ILog logger)
		{
			Log = logger ?? DefaultLogger.CreateLogger();

			// Setup defaults
			RawAttachment = null;
			RawContent = null;

			// Setup parameters
			ContentFileName = fileName;
		}

		/// <summary>
		/// Create an Attachment from byte contents. These are NOT parsed in any way, but assumed to be correct.
		/// This is used for MS-TNEF attachments
		/// </summary>
		/// <param name="attachmentContent">The contents of the Attachment</param>
		/// <param name="fileName">Sets the attachment file name to the supplied argument</param>
		/// <param name="contentType">The content type of the Attachment</param>
		/// <param name="logger">The logging interface to be used by the object</param>
		public Attachment(byte[] attachmentContent, string fileName, string contentType, ILog logger)
			: this(fileName, logger)
		{
			string bytesInAString = Encoding.Default.GetString(attachmentContent);
			RawContent = bytesInAString;
			RawAttachment = bytesInAString;
			Headers = new MessageHeader(HeaderFieldParser.ParseContentType(contentType));
		}

		/// <summary>
		/// Create an Attachment from byte contents. These are NOT parsed in any way, but assumed to be correct.
		/// This is used for MS-TNEF attachments
		/// </summary>
		/// <param name="attachmentContent">attachment bytes content</param>
		/// <param name="fileName">Sets the attachment file name to the supplied argument</param>
		/// <param name="contentType">The content type of the Attachment</param>
		public Attachment(byte[] attachmentContent, string fileName, string contentType)
			: this(attachmentContent, fileName, contentType, null)
		{
		}

		/// <summary>
		/// Create an attachment from a string, with some headers use from the message it is inside
		/// </summary>
		/// <param name="attachmentContent">The content of the Attachment</param>
		/// <param name="headersFromMessage">The attachments headers defaults to some of the message headers, this is the headers from the message</param>
		/// <param name="logger">The logging interface to be used by the object</param>
		public Attachment(string attachmentContent, MessageHeader headersFromMessage, ILog logger)
			: this(string.Empty, logger)
		{
			if (attachmentContent == null)
				throw new ArgumentNullException("attachmentContent");

			RawContent = attachmentContent;

			string rawHeaders;
			NameValueCollection headers;
			HeaderExtractor.ExtractHeaders(attachmentContent, out rawHeaders, out headers);

			Headers = new MessageHeader(headers, headersFromMessage.ContentType, headersFromMessage.ContentTransferEncoding);

			// If we parsed headers, as we just did, the RawAttachment is found by removing the headers
			// We also want to remove the line just after the headers, that tells the headers ended
			RawAttachment = Utility.ReplaceFirstOccurrence(attachmentContent, rawHeaders + "\r\n\r\n", "");

			// Set the filename
			ContentFileName = FigureOutFilename(Headers);
		}

		/// <summary>
		/// Create an attachment from a string, with some headers use from the message it is inside
		/// </summary>
		/// <param name="attachmentContent">The content of the Attachment</param>
		/// <param name="headersFromMessage">The attachments headers defaults to some of the message headers, this is the headers from the message</param>
		public Attachment(string attachmentContent, MessageHeader headersFromMessage)
			: this(attachmentContent, headersFromMessage, null)
		{
		}
		#endregion

		/// <summary>
		/// This method is responsible for picking a good name for an Attachment
		/// based on the headers of it
		/// </summary>
		/// <param name="headers">The headers that can be used to give a reasonable name</param>
		/// <returns>A name to use for an Attachment with the headers given</returns>
		private static string FigureOutFilename(MessageHeader headers)
		{
			// There is a name field in the ContentType
			if (!string.IsNullOrEmpty(headers.ContentType.Name))
				return headers.ContentType.Name;

			// There is a FileName in the ContentDisposition
			if (headers.ContentDisposition != null)
				return headers.ContentDisposition.FileName;

			// We could not find any given name. Instead we will try
			// to give a name based on the MediaType
			if (headers.ContentType.MediaType != null)
			{
				string type = headers.ContentType.MediaType.ToLower();
				if (type.Contains("report"))
					return DefaultReportFileName;

				if (type.Contains("multipart/"))
					return DefaultMIMEFileName;

				if (type.Contains("message/rfc822"))
					return DefaultMIMEFileName;
			}

			// If it was not possible with the MediaType, use the ContentID as a name
			if (headers.ContentID != null)
				return headers.ContentID;

			// If everything else fails, just use the default name
			return DefaultFileName;
		}

		/// <summary>
		/// Decode the attachment to text
		/// </summary>
		/// <returns>Decoded attachment text</returns>
		public string DecodeAsText()
		{
			if (!string.IsNullOrEmpty(Headers.ContentType.MediaType) && Headers.ContentType.MediaType.Equals("message/rfc822", StringComparison.InvariantCultureIgnoreCase))
				return EncodedWord.Decode(RawAttachment);

			return Utility.DoDecode(RawAttachment, Headers.ContentTransferEncoding, Headers.ContentType.CharSet);
		}

		/// <summary>
		/// Decode attachment to be a message object
		/// </summary>
		/// <param name="removeHeaderBlankLine"></param>
		/// <param name="useRawContent"></param>
		/// <returns>new message object</returns>
		public Message DecodeAsMessage(bool removeHeaderBlankLine, bool useRawContent)
		{
			string contentToDecode = useRawContent ? RawContent : RawAttachment;

			if (removeHeaderBlankLine && contentToDecode.StartsWith("\r\n"))
				contentToDecode = contentToDecode.Substring(2, contentToDecode.Length - 2);
			return new Message(false, contentToDecode, false, Log);
		}

		/// <summary>
		/// Decode the attachment to bytes
		/// </summary>
		/// <returns>Decoded attachment bytes</returns>
		public byte[] DecodedAsBytes()
		{
			// TODO Is Encoding.Default good enough?
			return Encoding.Default.GetBytes(DecodeAsText());
		}

		/// <summary>
		/// Save this Attachment to a file
		/// </summary>
		/// <param name="file">File to write Attachment to</param>
		/// <returns><see langword="true"/> if save was successful, <see langword="false"/> if save failed</returns>
		public bool SaveToFile(FileInfo file)
		{
			return Utility.SaveByteContentToFile(file, DecodedAsBytes());
		}

		/// <summary>
		/// Compares this instance with the specified <see cref="Attachment"/> and indicates whether this
		/// instance precedes, follows, or appears in the same position
		/// </summary>
		/// <param name="attachment">The attachment to compare against</param>
		/// <returns>
		/// A 32-bit signed integer indicating the lexical relationship between the two comparands.
		/// </returns>
		public int CompareTo(Attachment attachment)
		{
			return RawAttachment.CompareTo(attachment.RawAttachment);
		}

		/// <summary>
		/// Verify if the attachment is an RFC822 message.
		/// </summary>
		/// <returns><see langword="true"/> if Attachment is a RFC822 message, <see langword="false"/> otherwise</returns>
		public bool IsMIMEMailFile()
		{
			return (Headers.ContentType.MediaType != null &&
			        Headers.ContentType.MediaType.ToLower().Contains("message/rfc822")) ||
			       ContentFileName.EndsWith(".eml", true, CultureInfo.InvariantCulture);
		}

		///<summary>
		/// Checks if this Attachment is a multi part attachment
		///</summary>
		///<returns><see langword="true"/> if attachment is a multi part attachment, <see langword="false"/> otherwise</returns>
		public bool isMultipartAttachment()
		{
			return Headers.ContentType.MediaType != null && Headers.ContentType.MediaType.ToLower().Contains("multipart/");
		}
	}
}