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
using System.Text;

namespace OpenPOP.MIME.Decode
{
    public static class Base64
    {
        private static byte[] decodeToBytes(string strText)
        {
            try
            {
                return Convert.FromBase64String(strText);
            }
            catch (Exception e)
            {
                Utility.LogError("decodeToBytes:" + e.Message);
                
                return Encoding.Default.GetBytes("\0");
            }
        }

        /// <summary>
        /// Decoded a Base64 encoded string using the Default encoding of the system
        /// </summary>
        /// <param name="base64Encoded">Source string to decode</param>
        /// <returns>A decoded string</returns>
        public static string decode(string base64Encoded)
        {
            return Encoding.Default.GetString(decodeToBytes(base64Encoded));
        }

        /// <summary>
        /// Decoded a Base64 encoded string using a specified encoding
        /// </summary>
        /// <param name="base64Encoded">Source string to decode</param>
        /// <param name="nameOfEncoding">The name of the encoding to use</param>
        /// <returns>A decoded string</returns>
        public static string decode(string base64Encoded, string nameOfEncoding)
        {
            try
            {
                return Encoding.GetEncoding(nameOfEncoding).GetString(decodeToBytes(base64Encoded));
            }
            catch(Exception e)
            {
                Utility.LogError("decode: " + e.Message);
                return decode(base64Encoded);
            }
        }

        /// <summary>
        /// Checks if the Content-Type is base64
        /// </summary>
        /// <param name="contentType">The content type to be checked</param>
        /// <returns>True if it is base64 or false otherwise</returns>
        public static bool IsBase64(string contentType)
        {
            if (contentType != null)
                return contentType.ToLower().Equals("base64");

            return false;
        }
    }
}
