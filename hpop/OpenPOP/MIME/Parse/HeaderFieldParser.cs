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

using OpenPOP.MIME.Decode;

namespace OpenPOP.MIME.Parse
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
        /// Example of input:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// 
        /// It might also contain encoded text.
        /// A username in front of the emailaddress is not required.
        /// <see cref="EncodedWord.decode">For more information about encoded text</see>
        /// </summary>
        /// <param name="input">The value to parse out and email and/or a username</param>
        /// <param name="username">
        /// The decoded username in front.
        /// From the example this would be "Eksperten mailrobot"
        /// If there is no username, returned value will be empty.
        /// </param>
        /// <param name="emailAddress">
        /// The decoded email address.
        /// From the example this would be noreply@mail.eksperten.dk
        /// </param>
        public static void ParseEmailAddress(string input, out string username, out string emailAddress)
        {
            // Remove exesive whitespace
            input = input.Trim();

            // Find the location of the email address
            int indexStartEmail = input.LastIndexOf("<");
            int indexEndEmail = input.LastIndexOf(">");

            if (indexStartEmail >= 0 && indexEndEmail >= 0)
            {
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
                emailAddress = input.Substring(indexStartEmail, emailLength);

                // Decode both values
                username = EncodedWord.decode(username);
                emailAddress = EncodedWord.decode(emailAddress);
            }
            else
            {
                // Check first to see, as a last resort, if it contains an email only
                if (input.Contains("@"))
                {
                    username = "";
                    emailAddress = input;
                }
                else
                {
                    // This must be a group name only then
                    username = input;
                    emailAddress = "";
                }
            }
        }

        /// <summary>
        /// Parse date time info from MIME Date header
        /// </summary>
        /// <param name="strDate">Encoded MIME date time</param>
        /// <returns>Decoded date time info</returns>
        public static string ParseEmailDate(string strDate)
        {
            string strRet = strDate.Trim();
            int indexOfTag = strRet.IndexOf(",");
            if (indexOfTag != -1)
            {
                strRet = strRet.Substring(indexOfTag + 1);
            }

            strRet = Utility.Substring(strRet, "+");
            strRet = Utility.Substring(strRet, "-");
            strRet = Utility.Substring(strRet, "GMT");
            strRet = Utility.Substring(strRet, "CST");
            return strRet.Trim();
        }
    }
}
