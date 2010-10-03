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