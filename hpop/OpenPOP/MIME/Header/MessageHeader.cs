using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using OpenPOP.MIME.Decode;

namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// Class that holds all headers for a message
	/// </summary>
	/// <remarks>See <a href="http://www.rfc-editor.org/rfc/rfc4021.txt">http://www.rfc-editor.org/rfc/rfc4021.txt</a> for a large list of headers</remarks>
	public sealed class MessageHeader
	{
		#region Properties
		/// <summary>
		/// All headers which were not recognized and explicitly dealt with.
		/// This should mostly be custom headers, which are marked as X-[name].
		/// Empty list if no unknown headers
		/// </summary>
		/// <remarks>
		/// If you as a user, feels that a header in this collection should
		/// be parsed, feel free to notify the developers.
		/// </remarks>
		public NameValueCollection UnknownHeaders { get; private set; }

		/// <summary>
		/// A human readable description of the body
		/// Null if not set
		/// </summary>
		public string ContentDescription { get; private set; }

		/// <summary>
		/// ID of the content part (like an attached image). Used with multipart messages.
		/// Null if not set
		/// </summary>
		/// <see cref="MessageID">For an ID of the message</see>
		public string ContentID { get; private set; }

		/// <summary>
		/// Message keywords
		/// Empty list if no keywords
		/// </summary>
		public List<string> Keywords { get; private set; }

		/// <summary>
		/// Tells to where a Disposition Notification should be sent to.
		/// These notifications can be deletion, printing, ...
		/// Empty list of not set
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc3798">http://tools.ietf.org/html/rfc3798</a> for details</remarks>
		public List<MailAddress> DispositionNotificationTo { get; private set; }

		/// <summary>
		/// This is the Received headers. This tells the path that the email went.
		/// Empty list of not used
		/// </summary>
		public List<string> Received { get; private set; }

		/// <summary>
		/// Importance level type
		/// 
		/// The importance level is set to normal, if no such header field was mentioned or it contained
		/// unknown information. This is the expected behavior according to the RFC.
		/// </summary>
		public MessageImportance Importance { get; private set; }

		/// <summary>
		/// The CONTENT-TRANSFER-ENCODING header field
		/// 
		/// If the header was not found when this object was created, it is set
		/// to the default of 7BIT
		/// </summary>
		/// <remarks>See <a href="http://www.ietf.org/rfc/rfc2045.txt">http://www.ietf.org/rfc/rfc2045.txt</a> Part 6 for details</remarks>
		public ContentTransferEncoding ContentTransferEncoding { get; private set; }

		/// <summary>
		/// Carbon Copy. This specifies who got a copy of the message.
		/// Empty list of not set
		/// </summary>
		public List<MailAddress> CC { get; private set; }

		/// <summary>
		/// Blind Carbon Copy. This specifies who got a copy of the message, but others
		/// cannot see who these persons are.
		/// Empty list of not set
		/// </summary>
		public List<MailAddress> BCC { get; private set; }

		/// <summary>
		/// Specifies to who this mail was for.
		/// Empty list if not used
		/// </summary>
		public List<MailAddress> To { get; private set; }

		/// <summary>
		/// Specifies who sent the email
		/// Null if not set
		/// </summary>
		public MailAddress From { get; private set; }

		/// <summary>
		/// Specifies to who a reply to the message should be sent
		/// Null if not set
		/// </summary>
		public MailAddress ReplyTo { get; private set; }

		/// <summary>
		/// The ContentType header field.
		/// If not set, the ContentType is created by the default string
		/// defined in <a href="http://www.ietf.org/rfc/rfc2045.txt">http://www.ietf.org/rfc/rfc2045.txt</a> Section 5.2
		/// which is "text/plain; charset=us-ascii"
		/// </summary>
		public ContentType ContentType { get; private set; }

		/// <summary>
		/// The ContentDisposition header field
		/// Null if not set
		/// </summary>
		public ContentDisposition ContentDisposition { get; private set; }

		/// <summary>
		/// The Date when the email was sent.
		/// This is the raw value. <see cref="DateSent"/> for a parsed up <see cref="DateTime"/> value of this field
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">http://tools.ietf.org/html/rfc5322#section-3.6.1</a> for more details</remarks>
		public string Date { get; private set; }

		/// <summary>
		/// The Date when the email was sent.
		/// This is the parsed equivalent of <see cref="Date"/>.
		/// Notice that the TimeZone of the DateTime object is in UTC and has NOT been converted
		/// to local TimeZone.
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">http://tools.ietf.org/html/rfc5322#section-3.6.1</a> for more details</remarks>
		public DateTime DateSent { get; private set; }

		/// <summary>
		/// An ID of the message that is SUPPOSED to be in every message according to the RFC.
		/// The ID is unique
		/// Null if not set (which should be rare)
		/// </summary>
		public string MessageID { get; private set; }

		/// <summary>
		/// The Mime Version.
		/// This field will almost always show 1.0
		/// Null if not set
		/// </summary>
		public string MimeVersion { get; private set; }

		/// <summary>
		/// A single <see cref="MailAddress"/> with no username inside
		/// This is a trace header field, that should be in all messages
		/// Null if not set
		/// </summary>
		public MailAddress ReturnPath { get; private set; }

		/// <summary>
		/// The subject line of the message in decoded, one line state.
		/// This should be in all messages.
		/// Null if not set
		/// </summary>
		public string Subject { get; private set; }
		#endregion

		/// <summary>
		/// Used to set up default values
		/// </summary>
		private MessageHeader()
		{
			// Create empty lists as defaults. We do not like null values
			To = new List<MailAddress>();
			CC = new List<MailAddress>();
			BCC = new List<MailAddress>();
			Received = new List<string>();
			Keywords = new List<string>();
			DispositionNotificationTo = new List<MailAddress>();
			UnknownHeaders = new NameValueCollection();

			// Default importancetype is Normal (assumed if not set)
			Importance = MessageImportance.Normal;

			// 7BIT is the default ContentTransferEncoding (assumed if not set)
			ContentTransferEncoding = ContentTransferEncoding.SevenBit;

			// text/plain; charset=us-ascii is the default ContentType
			ContentType = new ContentType("text/plain; charset=us-ascii");
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a MessageHeader
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid MessageHeader object</returns>
		/// <exception cref="ArgumentNullException">If headers is <see langword="null"/></exception>
		public MessageHeader(NameValueCollection headers)
			: this()
		{
			ParseHeaders(headers);
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a MessageHeader, but with some other default values
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <param name="contentType">A <see cref="ContentType"/> to use as default, which might get overwritten</param>
		/// <param name="contentTransferEncoding">A <see cref="ContentTransferEncoding"/> to use as default, which might get overwritten</param>
		/// <returns>A valid MessageHeader object</returns>
		/// <exception cref="ArgumentNullException">If headers is <see langword="null"/></exception>
		public MessageHeader(NameValueCollection headers, ContentType contentType, ContentTransferEncoding contentTransferEncoding)
			: this()
		{
			ContentType = contentType;
			ContentTransferEncoding = contentTransferEncoding;

			ParseHeaders(headers);
		}

		/// <summary>
		/// Very simply header object.
		/// It uses all defaults but sets the <paramref name="contentType"/>
		/// </summary>
		/// <param name="contentType">The <see cref="ContentType"/> to use</param>
		/// <exception cref="ArgumentNullException">If <paramref name="contentType"/> was <see langword="null"/></exception>
		public MessageHeader(ContentType contentType)
			: this()
		{
			if (contentType == null)
				throw new ArgumentNullException();

			ContentType = contentType;
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a <see cref="MessageHeader"/>
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid <see cref="MessageHeader"/> object</returns>
		/// <exception cref="ArgumentNullException">If headers is <see langword="null"/></exception>
		private void ParseHeaders(NameValueCollection headers)
		{
			if (headers == null)
				throw new ArgumentNullException();

			// Now begin to parse the header values
			foreach (string headerName in headers.Keys)
			{
				string[] headerValues = headers.GetValues(headerName);
				if (headerValues != null)
				{
					foreach (string headerValue in headerValues)
					{
						ParseHeader(headerName, headerValue);
					}
				}
			}
		}

		#region Header fields parsing
		/// <summary>
		/// Parses a single header and sets member variables according to it.
		/// </summary>
		/// <param name="headerName">The name of the header</param>
		/// <param name="headerValue">The value of the header in unfolded state (only one line)</param>
		private void ParseHeader(string headerName, string headerValue)
		{
			switch (headerName.ToUpper())
			{
				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "TO":
					To = HeaderFieldParser.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "CC":
					CC = HeaderFieldParser.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "BCC":
					BCC = HeaderFieldParser.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.2
				case "FROM":
					// There is only one MailAddress in the from field
					From = HeaderFieldParser.ParseMailAddress(headerValue);
					break;

				// http://tools.ietf.org/html/rfc5322#section-3.6.2
				// The implementation here might be wrong
				case "REPLY-TO":
					// I am unsure if there is more than one email address here
					ReplyTo = HeaderFieldParser.ParseMailAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.5
				// RFC 5322:
				// The "Keywords:" field contains a comma-separated list of one or more
				// words or quoted-strings.
				// The field are intended to have only human-readable content
				// with information about the message
				case "KEYWORDS":
					string[] KeywordsTemp = headerValue.Split(',');
					foreach (string keyword in KeywordsTemp)
					{
						// Remove the quotes if there is any
						Keywords.Add(Utility.RemoveQuotes(keyword.Trim()));
					}
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.7
				case "RECEIVED":
					// Simply add the value to the list
					Received.Add(headerValue.Trim());
					break;

				case "IMPORTANCE":
					Importance = HeaderFieldParser.ParseImportance(headerValue.Trim());
					break;


				// See http://tools.ietf.org/html/rfc3798#section-2.1
				case "DISPOSITION-NOTIFICATION-TO":
					DispositionNotificationTo = HeaderFieldParser.ParseMailAddresses(headerValue);
					break;

				case "MIME-VERSION":
					MimeVersion = headerValue.Trim();
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.5
				case "SUBJECT":
				case "THREAD-TOPIC":
					Subject = EncodedWord.Decode(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.7
				case "RETURN-PATH":
					// Return-paths does not include a username, but we 
					// may still use the address parser 
					ReturnPath = HeaderFieldParser.ParseMailAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.4
				// Example Message-ID
				// <33cdd74d6b89ab2250ecd75b40a41405@nfs.eksperten.dk>
				case "MESSAGE-ID":
					MessageID = headerValue.Trim().TrimEnd('>').TrimStart('<');
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.1
				case "DATE":
					Date = headerValue.Trim();
					DateSent = RFC2822DateTime.StringToDate(headerValue);
					break;

				// See ContentTransferEncoding class for more details
				case "CONTENT-TRANSFER-ENCODING":
					ContentTransferEncoding = HeaderFieldParser.ParseContentTransferEncoding(headerValue.Trim());
					break;

				// See http://www.ietf.org/rfc/rfc2045.txt section 8.
				case "CONTENT-DESCRIPTION":
					// Human description of for example a file. Can be encoded
					ContentDescription = EncodedWord.Decode(headerValue.Trim());
					break;

				// See http://www.ietf.org/rfc/rfc2045.txt section 5.1.
				// Example: Content-type: text/plain; charset="us-ascii"
				case "CONTENT-TYPE":
					ContentType = HeaderFieldParser.ParseContentType(headerValue);
					break;

				case "CONTENT-DISPOSITION":
					ContentDisposition = HeaderFieldParser.ParseContentDisposition(headerValue);
					break;

				// Example: <foo4*foo1@bar.net>
				case "CONTENT-ID":
					ContentID = headerValue.Trim().Trim('<').Trim('>');
					break;

				default:
					// This is a unknown header

					// Custom headers are allowed. That means headers
					// that are not mentionen in the RFC.
					// Such headers start with the letter "X"
					// We do not have any special parsing of such

					// Add it to unknown headers
					UnknownHeaders.Add(headerName, headerValue);
					break;
			}
		}
		#endregion
	}
}