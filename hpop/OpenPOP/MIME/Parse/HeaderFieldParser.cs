/******************************************************************************
	Copyright 2003-2004 Hamid Qureshi and Unruled Boy 
	OpenPOP.Net is free software; you can redistribute it and/or modify
	it under the terms of the Lesser GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	OpenPOP.Net is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	Lesser GNU General Public License for more details.

	You should have received a copy of the Lesser GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Net.Mail;
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
        /// Some name
        /// 
        /// 
        /// It might also contain encoded text.
        /// <see cref="EncodedWord.decode">For more information about encoded text</see>
        /// </summary>
        /// <param name="input">The value to parse out and email and/or a username</param>
        /// <returns>A valid MailAddress where the input has been parsed into or null if the input is not valid</returns>
        public static MailAddress ParseMailAddress(string input)
        {
            // Remove exesive whitespace
            input = input.Trim();

            // Decode the value, if it was encoded
            input = EncodedWord.decode(input);

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
            if(input.Contains("@"))
                return new MailAddress(input);

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
        public static MessageImportanceType ParseImportance(string headerValue)
        {
            switch (headerValue.ToUpper())
            {
                case "5":
                case "HIGH":
                    return MessageImportanceType.High;

                case "3":
                case "NORMAL":
                    return MessageImportanceType.Normal;

                case "1":
                case "LOW":
                    return MessageImportanceType.Low;

                default:
                    return MessageImportanceType.Normal;
            }
        }
    }
}