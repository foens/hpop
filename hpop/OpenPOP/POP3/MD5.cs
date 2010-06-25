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
using System.Security.Cryptography;
using System.Text;

namespace OpenPOP.POP3
{
	public static class MD5
	{
        /// <summary>
        /// Computes the MD5 hash function on a string
        /// </summary>
        /// <param name="input">The input string to be hashed</param>
        /// <returns>The MD5 hash of the input string</returns>
	    public static string ComputeHashHex(String input)
		{
	        System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();

			// Give the md5 function the bytes of the string, and get an hashed byte[] as output
	        byte[] res = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);

            StringBuilder returnThis = new StringBuilder();

            // Convert the hashed value back into a string
	        foreach (byte re in res)
	            returnThis.Append(Uri.HexEscape((char) re));

	        return returnThis.ToString().Replace("%", "").ToLower();
		}
	}
}