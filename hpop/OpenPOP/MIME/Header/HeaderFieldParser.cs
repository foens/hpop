using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using OpenPOP.MIME.Decode;

namespace OpenPOP.MIME.Header
{
    /// <summary>
    /// Class that can parse different fields in the header sections of a MIME message
    /// </summary>
    public static class HeaderFieldParser
    {
        /// <summary>
        /// Parse email address from a MIME header
        /// http://tools.ietf.org/html/rfc5322#section-3.4
        /// 
        /// Examples of input:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// "Eksperten mailrobot" <noreply@mail.eksperten.dk>
        /// <noreply@mail.eksperten.dk>
        /// noreply@mail.eksperten.dk
        /// Some name (will return null on this)
        /// 
        /// 
        /// It might also contain encoded text.
        /// <see cref="EncodedWord.Decode">For more information about encoded text</see>
        /// </summary>
        /// <param name="input">The value to parse out and email and/or a username</param>
        /// <returns>A valid MailAddress where the input has been parsed into or null if the input is not valid</returns>
        public static MailAddress ParseMailAddress(string input)
        {
            // Remove exesive whitespace
            input = input.Trim();

            // Decode the value, if it was encoded
            input = EncodedWord.Decode(input);

            // Find the location of the email address
            int indexStartEmail = input.LastIndexOf("<");
            int indexEndEmail = input.LastIndexOf(">");

            if (indexStartEmail >= 0 && indexEndEmail >= 0)
            {
                string username;
                // Check if there is a username in front of the email address
                if (indexStartEmail > 0)
                {
                    // Parse out the user
                    username = input.Substring(0, indexStartEmail).Trim();
                }
                else
                {
                    // There was no user
                    username = "";
                }

                // Parse out the email address without the "<"  and ">"
                indexStartEmail = indexStartEmail + 1;
                int emailLength = indexEndEmail - indexStartEmail;
                string emailAddress = input.Substring(indexStartEmail, emailLength);

                // There has been cases where there was no emailaddress between the < and >
                if (emailAddress.Equals(""))
                    return null;

                // If the username is quoted, MailAddress' constructor will remove them for us
                return new MailAddress(emailAddress, username);
            }

            // This might be on the form noreply@mail.eksperten.dk
            // Sometimes invalid emails are sent, like sqlmap-user@sourceforge.net. (last period is illigal)
            try
            {
                if (input.Contains("@"))
                    return new MailAddress(input);
            }
            catch (FormatException)
            { }

            // This is not a MailAddress
            // It could be that the format used was simply a name
            // which is indeed valid according to the RFC
            // Example:
            // Eksperten mailrobot
            return null;
        }

        /// <summary>
        /// Parses input of the form
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>, ...
        /// to a list of MailAddresses
        /// </summary>
        /// <param name="input">The input that is a comma-seperated list of EmailAddresses to parse</param>
        /// <returns>A List of MailAddresses, or an empty list if there was no valid EmailAddresses to parse</returns>
        public static List<MailAddress> ParseMailAddresses(string input)
        {
            List<MailAddress> returner = new List<MailAddress>();

            // MailAddresses are split by commas
            string[] mailAddresses = input.Split(',');

            // Parse each of these
            foreach (string mailAddress in mailAddresses)
            {
                MailAddress address = ParseMailAddress(mailAddress);

                // Silently drop invalid mailaddresses
                if(address != null)
                    returner.Add(address);
            }

            return returner;
        }

        /// <summary>
        /// Parses the Content-Transfer-Encoding header
        /// </summary>
        /// <param name="headerValue">The value for the header to be parsed</param>
        /// <returns>A ContentTransferEncoding</returns>
        public static ContentTransferEncoding ParseContentTransferEncoding(string headerValue)
        {
            if (headerValue == null)
                return ContentTransferEncoding.SevenBit; // This is the default value

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
        /// <returns>A valid importancetype. If the headerValue is not recognized, Normal is returned.</returns>
        public static MessageImportance ParseImportance(string headerValue)
        {
            switch (headerValue.ToUpper())
            {
                case "5":
                case "HIGH":
                    return MessageImportance.High;

                case "3":
                case "NORMAL":
                    return MessageImportance.Normal;

                case "1":
                case "LOW":
                    return MessageImportance.Low;

                default:
                    return MessageImportance.Normal;
            }
        }

        /// <summary>
        /// Parses a the value for the header Content-Type to 
        /// a ContentType object
        /// </summary>
        /// <param name="headerValue">The value to be parsed</param>
        /// <returns>A valid ContentType object</returns>
        public static ContentType ParseContentType(string headerValue)
        {
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
        /// a ContentDisposition object
        /// </summary>
        /// <param name="headerValue">The value to be parsed</param>
        /// <returns>A valid ContentDisposition object</returns>
        public static ContentDisposition ParseContentDisposition(string headerValue)
        {
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
                { }

                // Then lets try to full 00 replacement
                return new ContentDisposition(headerValue.Replace("00", "01").Replace("GMT", "+0001"));
            }
        }
    }
}