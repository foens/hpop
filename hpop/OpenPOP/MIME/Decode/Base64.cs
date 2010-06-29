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
        private static byte[] DecodeToBytes(string strText)
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
        public static string Decode(string base64Encoded)
        {
            return Encoding.Default.GetString(DecodeToBytes(base64Encoded));
        }

        /// <summary>
        /// Decoded a Base64 encoded string using a specified encoding
        /// </summary>
        /// <param name="base64Encoded">Source string to decode</param>
        /// <param name="nameOfEncoding">The name of the encoding to use</param>
        /// <returns>A decoded string</returns>
        public static string Decode(string base64Encoded, string nameOfEncoding)
        {
            try
            {
                return Encoding.GetEncoding(nameOfEncoding).GetString(DecodeToBytes(base64Encoded));
            }
            catch(Exception e)
            {
                Utility.LogError("decode: " + e.Message);
                return Decode(base64Encoded);
            }
        }
    }
}