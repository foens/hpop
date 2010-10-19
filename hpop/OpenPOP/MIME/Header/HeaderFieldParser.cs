using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using OpenPOP.MIME.Decode;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// Class that can parse different fields in the header sections of a MIME message
	/// </summary>
	internal static class HeaderFieldParser
	{
		/// <summary>
		/// Parses the Content-Transfer-Encoding header
		/// </summary>
		/// <param name="headerValue">The value for the header to be parsed</param>
		/// <returns>A <see cref="ContentTransferEncoding"/></returns>
		public static ContentTransferEncoding ParseContentTransferEncoding(string headerValue)
		{
			if(headerValue == null)
				throw new ArgumentNullException("headerValue");

			switch (headerValue.Trim().ToUpper())
			{
				case "7BIT":
					return ContentTransferEncoding.SevenBit;

				case "8BIT":
					return ContentTransferEncoding.EightBit;

				case "QUOTED-PRINTABLE":
					return ContentTransferEncoding.QuotedPrintable;

				case "BASE64":
					return ContentTransferEncoding.Base64;

				case "BINARY":
					return ContentTransferEncoding.Binary;

				default:
					throw new ArgumentException("Unknown ContentTransferEncoding: " + headerValue);
			}
		}

		/// <summary>
		/// Parses an ImportanceType from a given Importance header value
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="MailPriority"/>. If the <paramref name="headerValue"/> is not recognized, Normal is returned.</returns>
		public static MailPriority ParseImportance(string headerValue)
		{
			if(headerValue == null)
				throw new ArgumentNullException("headerValue");

			switch (headerValue.ToUpper())
			{
				case "5":
				case "HIGH":
					return MailPriority.High;

				case "3":
				case "NORMAL":
					return MailPriority.Normal;

				case "1":
				case "LOW":
					return MailPriority.Low;

				default:
					return MailPriority.Normal;
			}
		}

		/// <summary>
		/// Parses a the value for the header Content-Type to 
		/// a <see cref="ContentType"/> object
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="ContentType"/> object</returns>
		public static ContentType ParseContentType(string headerValue)
		{
			if(headerValue == null)
				throw new ArgumentNullException("headerValue");

			try
			{
				return new ContentType(headerValue);
			}
			catch (Exception)
			{
				// Sometimes the header values are not accepted by the ContentType parser, so lets try different things

				// There have been instances of headerValues that looks like:
				// text/plain; charset = "UTF-8"
				// therefore, lets try to remove any whitespace
				return new ContentType(headerValue.Replace(" ", ""));
			}
		}

		/// <summary>
		/// Parses a the value for the header Content-Disposition to 
		/// a <see cref="ContentDisposition"/> object
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="ContentDisposition"/> object</returns>
		public static ContentDisposition ParseContentDisposition(string headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			try
			{
				return new ContentDisposition(headerValue);
			}
			catch (Exception)
			{
				// Sometimes the header values are not accepted by the ContentDisposition parser, so lets try different things

				// There have been instances of headerValues that looks like:
				// inline; filename="image001.png"; size=566;creation-date="Wed, 02 Jun 2010 15:39:38 GMT";modification-date="Wed, 02 Jun 2010 15:39:38 GMT"
				// The problem here is the GMT part. According to the RFC http://www.ietf.org/rfc/rfc2183.txt section 2.
				// quoted-date-time contents MUST be an RFC 822 `date-time'. Numeric timezones (+HHMM or -HHMM) MUST be used.
				// But according to http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/a6547f67-8e68-467c-b062-3dee20a03218
				// this is not enough, we have to change GMT to +0001, because of some obscure failure in the framework.
				// which seems that the number 00 is not supported
				// an input that confirms this is
				// inline; filename="image001.jpg"; size=71596;creation-date="Thu, 18 Mar 2010 00:17:09 GMT";modification-date="Thu, 18 Mar 2010 00:17:09 GMT"
				// if this is changed to 
				// inline; filename="image001.jpg"; size=71596;creation-date="Thu, 18 Mar 2010 01:17:09 GMT";modification-date="Thu, 18 Mar 2010 01:17:09 GMT"
				// it works, but if this is given:
				// inline; filename="image001.jpg"; size=71596;creation-date="Thu, 18 Mar 2010 01:00:09 GMT";modification-date="Thu, 18 Mar 2010 01:00:09 GMT"
				// then it fails!
				// Since on
				// https://connect.microsoft.com/VisualStudio/feedback/details/339010/contentdisposition-doesnt-respect-rfc-822-section-5
				// the team says that GMT is translated to 0000, GMT fails. Therefore we replace that first
				// and we also replace all 00 to 01, which WILL give WRONG times, but will now throw an exception.
				// Also, this wrong time should not happen on platforms that has a correct implementation of ContentDisposition, since we try that first

				// Lets first try with only GMT replaced
				try
				{
					return new ContentDisposition(headerValue.Replace("GMT", "+0001"));
				}
				catch (FormatException)
				{
					// Then lets try to full 00 replacement
					return new ContentDisposition(headerValue.Replace("00", "01").Replace("GMT", "+0001"));	
				}
			}
		}

		/// <summary>
		/// Parse a character set into an encoding
		/// </summary>
		/// <param name="charset">The character set to parse</param>
		/// <returns>An encoding which corresponds to the character set</returns>
		public static Encoding ParseCharsetToEncoding(string charset)
		{
			if (charset == null)
				throw new ArgumentNullException("charset");

			string charSetLower = charset.ToLower();
			if (charSetLower.Contains("windows") || charSetLower.Contains("cp"))
			{
				// It seems the character set contains an codepage value, which we should use to parse the encoding
				charSetLower = charSetLower.Replace("cp", ""); // Remove cp
				charSetLower = charSetLower.Replace("windows", ""); // Remove windows
				charSetLower = charSetLower.Replace("-", ""); // Remove - which could be used as cp-1554

				// Now we hope the only thing left in the charset is numbers.
				int codepageNumber = int.Parse(charSetLower);

				return Encoding.GetEncoding(codepageNumber);
			}

			// It seems there is no codepage value in the charset. It must be a named encoding
			return Encoding.GetEncoding(charset);
		}
	}
}