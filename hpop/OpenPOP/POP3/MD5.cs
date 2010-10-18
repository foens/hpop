using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// This is a simple class which encapsulates MD5 algorithms
	/// </summary>
	internal static class MD5
	{
		/// <summary>
		/// Computes the MD5 hash function on a string
		/// </summary>
		/// <param name="input">The input string to be hashed</param>
		/// <returns>The MD5 hash of the input string</returns>
		public static string ComputeHashHex(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider();

			// Give the md5 function the bytes of the string, and get an hashed byte[] as output
			// TODO Is Encoding.Default a good choice?
			byte[] result = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);

			StringBuilder returnThis = new StringBuilder();

			// Convert the hashed value back into a string
			foreach (byte aByte in result)
				returnThis.Append(Uri.HexEscape((char)aByte));

			return returnThis.ToString().Replace("%", "").ToLower();
		}
	}
}