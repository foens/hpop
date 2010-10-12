using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.IO;
using System.Net.Mail;
using System.Text;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME
{
	/// <summary>
	/// The class represents a MIME Message
	/// </summary>
	public class Message
	{
		#region Properties
		/// <summary>
		/// Whether to auto decode MS-TNEF attachment files
		/// </summary>
		private bool AutoDecodeMSTNEF { get; set; }

		/// <summary>
		/// Headers of the Message.
		/// </summary>
		public MessageHeader Headers { get; private set; }

		/// <summary>
		/// These are the message bodies that could be found in the message.
		/// The last message should be the message most faithful to what the user sent
		/// Commonly the second message is HTML and the first is plain text
		/// </summary>
		public List<MessageBody> MessageBody { get; private set; }

		/// <summary>
		/// Attachments for the Message
		/// </summary>
		public List<Attachment> Attachments { get; private set; }

		/// <summary>
		/// The raw message body part of the <see cref="RawMessage"/> that this message was constructed with.
		/// The Raw message is simply the message body part of the message, but the message body has NOT
		/// been decoded or converted in any way.
		/// You properly want to <see cref="MessageBody"/> instead.
		/// </summary>
		public string RawMessageBody { get; private set; }

		/// <summary>
		/// The header part from the <see cref="RawMessage"/> that this message was constructed with
		/// </summary>
		public string RawHeader { get; private set; }

		/// <summary>
		/// The raw content from which this message has been constructed
		/// </summary>
		public string RawMessage { get; private set; }

		/// <summary>
		/// The logging interface used by the object
		/// </summary>
		private ILog Log { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Sets up a default new message
		/// </summary>
		/// <param name="log">The logging interface to use</param>
		private Message(ILog log)
		{
			Log = log ?? DefaultLogger.CreateLogger();
			RawMessage = null;
			RawHeader = null;
			RawMessageBody = null;
			Attachments = new List<Attachment>();
			MessageBody = new List<MessageBody>();
			AutoDecodeMSTNEF = false;
		}

		/// <summary>
		/// Initializes a message from a .eml file
		/// </summary>
		/// <param name="autoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="onlyParseHeader">whether only decode the header without body</param>
		/// <param name="emlFile">File with email content to load from</param>
		/// <param name="logger">The logging interface to use</param>
		public Message(bool autoDecodeMSTNEF, bool onlyParseHeader, FileInfo emlFile, ILog logger)
			: this(logger)
		{
			string messageContent = null;
			if (Utility.ReadPlainTextFromFile(emlFile, ref messageContent))
			{
				AutoDecodeMSTNEF = autoDecodeMSTNEF;
				InitializeMessage(messageContent, onlyParseHeader);
			} else
			{
				throw new FileNotFoundException("Could not find file " + emlFile);
			}
		}

		/// <summary>
		/// Initializes a message from a .eml file
		/// </summary>
		/// <param name="autoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="onlyParseHeader">whether only decode the header without body</param>
		/// <param name="emlFile">File with email content to load from</param>
		public Message(bool autoDecodeMSTNEF, bool onlyParseHeader, FileInfo emlFile)
			: this(autoDecodeMSTNEF, onlyParseHeader, emlFile, null)
		{
		}

		/// <summary>
		/// Creates a new message from a string
		/// </summary>
		/// <param name="autoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="rawMessageContent">raw message content</param>
		/// <param name="onlyParseHeader">whether only decode the header without body</param>
		/// <param name="logger">The logging interface to use</param>
		public Message(bool autoDecodeMSTNEF, string rawMessageContent, bool onlyParseHeader, ILog logger)
			: this(logger)
		{
			AutoDecodeMSTNEF = autoDecodeMSTNEF;
			InitializeMessage(rawMessageContent, onlyParseHeader);
		}

		/// <summary>
		/// Creates a new message from a string
		/// </summary>
		/// <param name="autoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="rawMessageContent">raw message content</param>
		/// <param name="onlyParseHeader">whether only decode the header without body</param>
		public Message(bool autoDecodeMSTNEF, string rawMessageContent, bool onlyParseHeader)
			: this(autoDecodeMSTNEF, rawMessageContent, onlyParseHeader, null)
		{
		}
		#endregion

		#region Public functions
		/// <summary>
		/// Verify if the message is a report
		/// </summary>
		/// <returns><see langword="true"/> if message is a report message, <see langword="false"/> otherwise</returns>
		public bool IsReport()
		{
			if (string.IsNullOrEmpty(Headers.ContentType.MediaType))
				return false;
			return (Headers.ContentType.MediaType.IndexOf("report", StringComparison.InvariantCultureIgnoreCase) != -1);
		}

		/// <summary>
		/// translate pictures url within the body
		/// </summary>
		/// <param name="body">message body</param>
		/// <param name="hsbFiles">pictures collection</param>
		/// <returns>translated message body</returns>
		public string TranslateHTMLPictureFiles(string body, Hashtable hsbFiles)
		{
			foreach (Attachment attachment in Attachments)
			{
				if (Utility.IsPictureFile(attachment.ContentFileName))
				{
					if (!string.IsNullOrEmpty(attachment.Headers.ContentID))
						body = body.Replace("cid:" + attachment.Headers.ContentID, hsbFiles[attachment.ContentFileName].ToString());
					else
						body = body.Replace(attachment.ContentFileName, hsbFiles[attachment.ContentFileName].ToString());
				}
			}

			return body;
		}

		/// <summary>
		/// WARNING: This is work in progress / experimental. This might not work at all.
		/// If you find any bugs using this method, please report to the developers
		/// 
		/// Converts this message to a System.Net.Mail.MailMessage
		/// making the message easy to send using the inbuilt SMTP client of .NET
		/// </summary>
		/// <returns>A MailMessage object which corrosponds to this message</returns>
		public MailMessage ToMailMessage()
		{
			MailMessage mailMessage = new MailMessage();

			// It is not always that the From header is
			// in a message
			if(Headers.From != null)
				mailMessage.From = Headers.From;
			
			// TODO: This might not work!
			Encoding encodingUsed = Encoding.Default;
				if (Headers.ContentType.CharSet != null)
					encodingUsed = HeaderFieldParser.ParseCharsetToEncoding(Headers.ContentType.CharSet);

			// Add to
			foreach (MailAddress to in Headers.To)
				mailMessage.To.Add(to);

			mailMessage.Subject = Headers.Subject;
			mailMessage.SubjectEncoding = encodingUsed;

			// Take the most faithfull message
			mailMessage.Body = MessageBody[MessageBody.Count - 1].Body;
			mailMessage.BodyEncoding = encodingUsed;

			// If there are more than one body, add them as alternatives
			foreach (MessageBody body in MessageBody)
			{
				// Skip the one we added as the body of the mailMessage
				if (body.Body.Equals(mailMessage.Body))
					continue;
				
				// We need to give a stream to the AlternateView, so converting to stream
				byte[] byteArray = encodingUsed.GetBytes(body.Body);
				MemoryStream stream = new MemoryStream(byteArray);

				// Create the alternative view and add it to the mailMessage
				mailMessage.AlternateViews.Add(new AlternateView(stream, body.Type));
			}

			foreach (Attachment attachment in Attachments)
			{
				// We need to give a stream to the Attachment, so converting to stream
				MemoryStream stream = new MemoryStream(attachment.DecodedAsBytes());

				// Create the attachment and add it the to mailMessage
				mailMessage.Attachments.Add(new System.Net.Mail.Attachment(stream, attachment.Headers.ContentType));
			}

			mailMessage.ReplyTo = Headers.ReplyTo;

			foreach (MailAddress cc in Headers.CC)
				mailMessage.CC.Add(cc);

			mailMessage.Priority = Headers.Importance;

			// TODO What about sender property?

			return mailMessage;
		}

		/// <summary>
		/// Translate inline pictures within the body to a path where the images are saved
		/// under their ContentFileName.
		/// </summary>
		/// <param name="body">The body to be changed</param>
		/// <param name="path">Path to the location of the pictures</param>
		/// <returns>A Translated message body</returns>
		public string TranslateHTMLPictureFiles(string body, DirectoryInfo path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			if (!path.Exists)
				throw new ArgumentException("The path" + path.FullName + " does not exist", "path");

			foreach (Attachment attachment in Attachments)
			{
				if (Utility.IsPictureFile(attachment.ContentFileName))
				{
					if (!string.IsNullOrEmpty(attachment.Headers.ContentID))
						body = body.Replace("cid:" + attachment.Headers.ContentID, Path.Combine(path.FullName, attachment.ContentFileName));
					else
						body = body.Replace(attachment.ContentFileName, Path.Combine(path.FullName, attachment.ContentFileName));
				}
			}

			return body;
		}

		/// <summary>
		/// Save all Attachments included in this message to a defined path.
		/// The attachments name will be appended to the path, and saved under that name.
		/// </summary>
		/// <param name="pathToSaveTo">Path to place the attachments. Cannot be null. Path must exist.</param>
		/// <returns><see langword="true"/> if all attachments was saved successfully, <see langword="false"/> if just one failed</returns>
		public bool SaveAttachments(DirectoryInfo pathToSaveTo)
		{
			if (pathToSaveTo == null)
				throw new ArgumentNullException("pathToSaveTo");

			if (!pathToSaveTo.Exists)
				throw new ArgumentException("The path" + pathToSaveTo.FullName + " does not exist", "pathToSaveTo");

			try
			{
				bool result = true;

				foreach (Attachment attachment in Attachments)
				{
					result = attachment.SaveToFile(new FileInfo(Path.Combine(pathToSaveTo.FullName, attachment.ContentFileName)));
					if (result == false)
						break;
				}
				return result;
			} catch (Exception e)
			{
				Log.LogError("SaveAttachments()" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Save message content to an eml file
		/// </summary>
		/// <param name="file">The File location to save the message to</param>
		/// <param name="replaceFileIfExists">Should the file be replaced if it exists?</param>
		/// <returns><see langword="true"/> on success, <see langword="false"/> otherwise</returns>
		public bool SaveToMIMEEmailFile(FileInfo file, bool replaceFileIfExists)
		{
			return Utility.SavePlainTextToFile(file, RawMessage, replaceFileIfExists);
		}
		#endregion

		#region Main parser function
		/// <summary>
		/// Initializes a new message from raw MIME content.
		/// This method parses headers, message body and attachments.
		/// </summary>
		/// <param name="input">Raw message content from which parsing will begin</param>
		/// <param name="onlyParseHeaders">Whether only to parse and decode headers</param>
		private void InitializeMessage(string input, bool onlyParseHeaders)
		{
			// Keep the raw message for later usage
			RawMessage = input;

			// Genericly parse out header names and values
			// Also include the rawHeader text for later use
			string rawHeadersTemp;
			NameValueCollection headersUnparsedCollection;
			HeaderExtractor.ExtractHeaders(input, out rawHeadersTemp, out headersUnparsedCollection);
			RawHeader = rawHeadersTemp;

			// Parse the headers
			Headers = new MessageHeader(headersUnparsedCollection);

			if (onlyParseHeaders == false)
			{
				// The message body must be the full raw message, with headers removed.
				// The headers does not contain the last \r\n of the last line
				// The headers does not contain the delimiter \r\n to denote the end of the header section
				// Therefore we add the \r\n\r\n to the RawHeader
				RawMessageBody = Utility.ReplaceFirstOccurrence(RawMessage, RawHeader + "\r\n\r\n", "");

				// Check if the message is a multipart message (which means, has multiple message bodies)
				if (Headers.ContentType.MediaType.ToLower().Contains("multipart"))
				{
					// Set up attachments
					ParseMultipartMessageBody();

					// Some of the attachments can be text and html, these we want in our MessageBody instead
					if (Attachments.Count > 0)
					{
						List<Attachment> toRemoveFromAttachments = new List<Attachment>();

						// Check if the first attachment is the message
						foreach (Attachment attachment in Attachments)
						{
							if (
							    // The content type must be plain or html
							    (
							    	attachment.Headers.ContentType.MediaType.Contains("text/plain") ||
							    	attachment.Headers.ContentType.MediaType.Contains("text/html")
							    )
							    &&
							    // But if the attachment is a message, it cannot have a filename/name
							    !(
							     	(attachment.Headers.ContentType.Parameters != null && attachment.Headers.ContentType.Parameters.ContainsKey("name")) ||
							     	(attachment.Headers.ContentDisposition != null && attachment.Headers.ContentDisposition.Parameters.ContainsKey("filename"))
							     )
								)
							{
								MessageBody.Add(new MessageBody(attachment.DecodeAsText(), attachment.Headers.ContentType.MediaType));
								toRemoveFromAttachments.Add(attachment);
							}
						}

						foreach (Attachment removeFromAttachment in toRemoveFromAttachments)
						{
							Attachments.Remove(removeFromAttachment);
						}
					}
				} else
				{
					// This is not a multipart message.
					// This means that the whole message body is the actual message
					// Parse this according to encoding and such
					GetMessageBody(RawMessageBody);
				}
			}
		}
		#endregion

		#region Body parser functions
		/// <summary>
		/// Parses the <see cref="MessageBody"/> as a Multipart message.
		/// This method will add these parts as Attachments
		/// </summary>
		private void ParseMultipartMessageBody()
		{
			string multipartBoundary = Headers.ContentType.Boundary;

			if (string.IsNullOrEmpty(multipartBoundary))
				throw new ArgumentException("The body is a multipart message, but there is no multipart boundary");

			int indexOfAttachmentStart = 0;
			bool moreParts = true;

			// Keep working until we have parsed every message part in this message
			while (moreParts)
			{
				// Find the start of the message parts multipartBoundary
				indexOfAttachmentStart = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart);

				if (indexOfAttachmentStart == -1)
					throw new ArgumentException("The start of the attachment could not be found");

				// Find the start of this message part - which does not include the multipartBoundary or the trailing CRLF
				indexOfAttachmentStart = indexOfAttachmentStart + multipartBoundary.Length + "\r\n".Length;

				// Find the end of the attachment, were we do not want the last line
				int indexOfAttachmentEnd = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart);

				if (indexOfAttachmentEnd == -1)
					throw new ArgumentException("The end of the attachment could not be found");

				// Check if this is the last part, which ends with the multipartBoundary followed by "--"
				if (RawMessageBody.Substring(indexOfAttachmentEnd).StartsWith(multipartBoundary + "--"))
					moreParts = false;

				// Calculate the length. We do not want to include the last "\r\n" in the attachment
				int attachmentLength = indexOfAttachmentEnd - indexOfAttachmentStart - "\r\n".Length;

				string messagePart = RawMessageBody.Substring(indexOfAttachmentStart, attachmentLength);
				Attachment att = new Attachment(messagePart, Headers, Log);

				// Check if this is the MS-TNEF attachment type
				// which has ContentType application/ms-tnef
				// and also if we should decode it
				if (MIMETypes.IsMSTNEF(att.Headers.ContentType.MediaType) && AutoDecodeMSTNEF)
				{
					// It was a MS-TNEF attachment. Now we should parse it.
					using (TNEFParser tnef = new TNEFParser(att.DecodedAsBytes(), Log))
					{
						if (tnef.Parse())
						{
							// ms-tnef attachment might contain multiple attachments inside it
							foreach (TNEFAttachment tatt in tnef.Attachments())
							{
								Attachment attNew = new Attachment(tatt.Content, tatt.FileName, MIMETypes.GetMimeType(tatt.FileName), Log);
								Attachments.Add(attNew);
							}
						} else
							throw new ArgumentException("Could not parse TNEF attachment");
					}
				} else if (att.isMultipartAttachment())
				{
					// The attachment itself is a multipart message
					// Parse it as such, and take the attachments from it
					// and add it to our message
					// This will in reality flatten the structure
					Message m = att.DecodeAsMessage(true, true);
					foreach (MessageBody body in m.MessageBody)
					{
						MessageBody.Add(body);
					}

					foreach (Attachment attachment in m.Attachments)
						Attachments.Add(attachment);
				} else
				{
					// This must be an attachment
					Attachments.Add(att);
				}
			}
		}

		/// <summary>
		/// Parses message body of a MIME message
		/// </summary>
		/// <param name="buffer">Raw message body</param>
		private void GetMessageBody(string buffer)
		{
			// TODO foens: I do not like that this function is named Get
			//             but it actually clears the MessageBody list!

			MessageBody.Clear();

			try
			{
				if (Utility.IsOrNullTextEx(buffer))
					return;

				if (Utility.IsOrNullTextEx(Headers.ContentType.MediaType) && Headers.ContentTransferEncoding == ContentTransferEncoding.EightBit)
				{
					// Assume text/plain
					MessageBody.Add(new MessageBody(buffer, "text/plain"));
					return;
				}

				if (Headers.ContentType.MediaType != null && Headers.ContentType.MediaType.ToLower().Contains("digest"))
				{
					MessageBody.Add(new MessageBody(buffer, Headers.ContentType.MediaType));
					return;
				}

				string body;
				if (Headers.ContentType.MediaType != null && !Headers.ContentType.MediaType.ToLower().Contains("multipart"))
				{
					// This is not a multipart message.
					// It only contains some text
					// Now we only need to decode the text according to encoding
					body = Utility.DoDecode(buffer, Headers.ContentTransferEncoding, Headers.ContentType.CharSet);

					MessageBody.Add(new MessageBody(body, Headers.ContentType.MediaType));
					return;
				}

				// This is a multipart message with multiple message bodies or attachments
				int begin = 0;

				// Foreach part
				while (begin != -1)
				{
					string multipartBoundary = Headers.ContentType.Boundary;

					// The start of a part of the message body is indicated by a "--" and the MutlipartBoundary
					// Find this start, which should not be included in the message
					begin = buffer.IndexOf("--" + multipartBoundary, begin);
					if (begin != -1)
					{
						// Genericly parse out header names and values
						string rawHeadersTemp;
						NameValueCollection headersUnparsedCollection;
						HeaderExtractor.ExtractHeaders(buffer.Substring(begin), out rawHeadersTemp, out headersUnparsedCollection);

						// Parse the header name and values into strong types
						MessageHeader multipartHeaders = new MessageHeader(headersUnparsedCollection);

						// The message itself is located after the MultipartBoundary. It may contain headers, which is ended
						// by a empty line, which corrosponds to "\r\n\r\n". We don't want to include the "\r\n", so skip them.
						begin = buffer.IndexOf("\r\n\r\n", begin) + "\r\n\r\n".Length;

						// Find end of text
						// This is again ended by the "--" and the MultipartBoundary, where we don't want the last line delimter in the message
						int end = buffer.IndexOf("--" + multipartBoundary, begin) - "\r\n".Length;

						// Calculate the message length
						int messageLength = end - begin;

						// Now get the body out of the full message
						body = buffer.Substring(begin, messageLength);

						string charSet = Headers.ContentType.CharSet;
						if (multipartHeaders.ContentType.CharSet != null)
							charSet = multipartHeaders.ContentType.CharSet;

						// Decode the body
						body = Utility.DoDecode(body, multipartHeaders.ContentTransferEncoding, charSet);

						MessageBody.Add(new MessageBody(body, multipartHeaders.ContentType.MediaType));
					} else
					{
						// If we did not find any parts in the multipart message
						// We just add everything as a message
						if (MessageBody.Count == 0)
						{
							// Assume text/plain
							MessageBody.Add(new MessageBody(buffer, "text/plain"));
						}
						break;
					}
				}
			} catch (Exception e)
			{
				Log.LogError("GetMessageBody():" + e.Message);
				string body;
				try
				{
					// TODO foens: Why do we try to base64 decode here?
					//             Can we just assume it is base64?!
					body = Base64.Decode(buffer);
				} catch (Exception)
				{
					Log.LogError("GetMessageBody():" + e.Message);
					body = buffer;
				}
				MessageBody.Add(new MessageBody(body, "text/plain")); // Assume text/plain
			}
		}
		#endregion
	}
}