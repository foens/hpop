using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Net.Mime;
using OpenPop.Mime.Decode;

namespace OpenPop.Mime.Header
{
	/// <summary>
	/// Class that holds all headers for a message
	/// </summary>
	/// <remarks>See <a href="http://www.rfc-editor.org/rfc/rfc4021.txt">RFC 4021</a> for a large list of headers</remarks>
	public sealed class MessageHeader
	{
		#region Properties
		/// <summary>
		/// All headers which were not recognized and explicitly dealt with.
		/// This should mostly be custom headers, which are marked as X-[name].
		/// This list will be empty if all headers were recognized and parsed.
		/// </summary>
		/// <remarks>
		/// If you as a user, feels that a header in this collection should
		/// be parsed, feel free to notify the developers.
		/// </remarks>
		public NameValueCollection UnknownHeaders { get; private set; }

		/// <summary>
		/// A human readable description of the body
		/// <see langword="null"/> if no Content-Description header was present in the message.
		/// </summary>
		public string ContentDescription { get; private set; }

		/// <summary>
		/// ID of the content part (like an attached image). Used with MultiPart messages.
		/// <see langword="null"/> if no Content-ID header field was present in the message.
		/// </summary>
		/// <see cref="MessageId">For an ID of the message</see>
		public string ContentId { get; private set; }

		/// <summary>
		/// Message keywords
		/// The list will be empty if no Keywords header was present in the message
		/// </summary>
		public List<string> Keywords { get; private set; }

		/// <summary>
		/// A List of emails to people who wishes to be notified when some event happens.
		/// These events could be email
		///  - deletion
		///  - printing
		///  - received
		///  - ...
		/// The list will be empty if no Disposition-Notification-To header was present in the message
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc3798">RFC 3798</a> for details</remarks>
		public List<RFCMailAddress> DispositionNotificationTo { get; private set; }

		/// <summary>
		/// This is the Received headers. This tells the path that the email went.
		/// The list will be empty if no Received header was present in the message
		/// </summary>
		public List<string> Received { get; private set; }

		/// <summary>
		/// Importance level type
		/// 
		/// The importance level is set to normal, if no Importance header field was mentioned or it contained
		/// unknown information. This is the expected behavior according to the RFC.
		/// </summary>
		public MailPriority Importance { get; private set; }

		/// <summary>
		/// This header describes the Content encoding during transfer.
		/// 
		/// If no Content-Transfer-Encoding header was present in the message, it is set
		/// to the default of SevenBit in accordance to the RFC.
		/// </summary>
		/// <remarks>See <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a> Part 6 for details</remarks>
		public ContentTransferEncoding ContentTransferEncoding { get; private set; }

		/// <summary>
		/// Carbon Copy. This specifies who got a copy of the message.
		/// The list will be empty if no Cc header was present in the message
		/// </summary>
		public List<RFCMailAddress> Cc { get; private set; }

		/// <summary>
		/// Blind Carbon Copy. This specifies who got a copy of the message, but others
		/// cannot see who these persons are.
		/// The list will be empty if no Received Bcc was present in the message
		/// </summary>
		public List<RFCMailAddress> Bcc { get; private set; }

		/// <summary>
		/// Specifies who this mail was for
		/// The list will be empty if no To header was present in the message
		/// </summary>
		public List<RFCMailAddress> To { get; private set; }

		/// <summary>
		/// Specifies who sent the email
		/// <see langword="null"/> if no From header field was present in the message
		/// </summary>
		public RFCMailAddress From { get; private set; }

		/// <summary>
		/// Specifies who a reply to the message should be sent to
		/// <see langword="null"/> if no Reply-To header field was present in the message
		/// </summary>
		public RFCMailAddress ReplyTo { get; private set; }

		/// <summary>
		/// This is the sender of the email address.
		/// The RFC states that this field can be used if a secretary
		/// is sending an email for someone she is working for.
		/// The email here will then be the secretary's email, and
		/// the Reply-To field would hold the address of the person she works for.
		/// RFC states that if the Sender is the same as the From field,
		/// sender should not be included in the message.
		/// <see langword="null"/> if no Sender header field was present in the message
		/// </summary>
		public RFCMailAddress Sender { get; private set; }

		/// <summary>
		/// If not set, the ContentType is created by the default string
		/// defined in <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a> Section 5.2
		/// which is "text/plain; charset=us-ascii"
		/// If set, the default is overridden.
		/// </summary>
		public ContentType ContentType { get; private set; }

		/// <summary>
		/// Used to describe if a <see cref="MessagePart"/> is to be displayed or to be though of as an attachment.
		/// Also contains information about filename if such was sent.
		/// <see langword="null"/> if no Content-Disposition header field was present in the message
		/// </summary>
		public ContentDisposition ContentDisposition { get; private set; }

		/// <summary>
		/// The Date when the email was sent.
		/// This is the raw value. <see cref="DateSent"/> for a parsed up <see cref="DateTime"/> value of this field
		/// <see langword="null"/> if no Date header field was present in the message
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">RFC 5322 section-3.6.1</a> for more details</remarks>
		public string Date { get; private set; }

		/// <summary>
		/// The Date when the email was sent.
		/// This is the parsed equivalent of <see cref="Date"/>.
		/// Notice that the <see cref="TimeZone"/> of the <see cref="DateTime"/> object is in UTC and has NOT been converted
		/// to local <see cref="TimeZone"/>.
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">RFC 5322 section-3.6.1</a> for more details</remarks>
		public DateTime DateSent { get; private set; }

		/// <summary>
		/// An ID of the message that is SUPPOSED to be in every message according to the RFC.
		/// The ID is unique
		/// <see langword="null"/> if no Message-ID header field was present in the message
		/// </summary>
		public string MessageId { get; private set; }

		/// <summary>
		/// The Mime Version.
		/// This field will almost always show 1.0
		/// <see langword="null"/> if no Mime-Version header field was present in the message
		/// </summary>
		public string MimeVersion { get; private set; }

		/// <summary>
		/// A single <see cref="MailAddress"/> with no username inside
		/// This is a trace header field, that should be in all messages
		/// <see langword="null"/> if no Return-Path header field was present in the message
		/// </summary>
		public RFCMailAddress ReturnPath { get; private set; }

		/// <summary>
		/// The subject line of the message in decoded, one line state.
		/// This should be in all messages.
		/// <see langword="null"/> if no Subject header field was present in the message
		/// </summary>
		public string Subject { get; private set; }
		#endregion

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a MessageHeader
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid MessageHeader object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headers"/> is <see langword="null"/></exception>
		internal MessageHeader(NameValueCollection headers)
		{
			if (headers == null)
				throw new ArgumentNullException("headers");

			// Create empty lists as defaults. We do not like null values
			To = new List<RFCMailAddress>();
			Cc = new List<RFCMailAddress>();
			Bcc = new List<RFCMailAddress>();
			Received = new List<string>();
			Keywords = new List<string>();
			DispositionNotificationTo = new List<RFCMailAddress>();
			UnknownHeaders = new NameValueCollection();

			// Default importancetype is Normal (assumed if not set)
			Importance = MailPriority.Normal;

			// 7BIT is the default ContentTransferEncoding (assumed if not set)
			ContentTransferEncoding = ContentTransferEncoding.SevenBit;

			// text/plain; charset=us-ascii is the default ContentType
			ContentType = new ContentType("text/plain; charset=us-ascii");

			// Now parse the actual headers
			ParseHeaders(headers);
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a <see cref="MessageHeader"/>
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid <see cref="MessageHeader"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headers"/> is <see langword="null"/></exception>
		private void ParseHeaders(NameValueCollection headers)
		{
			if (headers == null)
				throw new ArgumentNullException("headers");

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
		/// <exception cref="ArgumentNullException">If <paramref name="headerName"/> or <paramref name="headerValue"/> is <see langword="null"/></exception>
		private void ParseHeader(string headerName, string headerValue)
		{
			if(headerName == null)
				throw new ArgumentNullException("headerName");

			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			switch (headerName.ToUpperInvariant())
			{
				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "TO":
					To = RFCMailAddress.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "CC":
					Cc = RFCMailAddress.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "BCC":
					Bcc = RFCMailAddress.ParseMailAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.2
				case "FROM":
					// There is only one MailAddress in the from field
					From = RFCMailAddress.ParseMailAddress(headerValue);
					break;

				// http://tools.ietf.org/html/rfc5322#section-3.6.2
				// The implementation here might be wrong
				case "REPLY-TO":
					// This field may actually be a list of addresses, but no
					// such case has been encountered
					ReplyTo = RFCMailAddress.ParseMailAddress(headerValue);
					break;

				// http://tools.ietf.org/html/rfc5322#section-3.6.2
				case "SENDER":
					Sender = RFCMailAddress.ParseMailAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.5
				// RFC 5322:
				// The "Keywords:" field contains a comma-separated list of one or more
				// words or quoted-strings.
				// The field are intended to have only human-readable content
				// with information about the message
				case "KEYWORDS":
					string[] keywordsTemp = headerValue.Split(',');
					foreach (string keyword in keywordsTemp)
					{
						// Remove the quotes if there is any
						Keywords.Add(Utility.RemoveQuotesIfAny(keyword.Trim()));
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
					DispositionNotificationTo = RFCMailAddress.ParseMailAddresses(headerValue);
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
					ReturnPath = RFCMailAddress.ParseMailAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.4
				// Example Message-ID
				// <33cdd74d6b89ab2250ecd75b40a41405@nfs.eksperten.dk>
				case "MESSAGE-ID":
					MessageId = headerValue.Trim().TrimEnd('>').TrimStart('<');
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.1
				case "DATE":
					Date = headerValue.Trim();
					DateSent = RFC2822DateTime.StringToDate(headerValue);
					break;

				// See http://www.ietf.org/rfc/rfc2045.txt section 6
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

				// See http://www.ietf.org/rfc/rfc2183.txt
				case "CONTENT-DISPOSITION":
					ContentDisposition = HeaderFieldParser.ParseContentDisposition(headerValue);
					break;

				// See http://www.ietf.org/rfc/rfc2045.txt section 7
				// Example: <foo4*foo1@bar.net>
				case "CONTENT-ID":
					ContentId = headerValue.Trim().Trim('<').Trim('>');
					break;

				default:
					// This is an unknown header

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