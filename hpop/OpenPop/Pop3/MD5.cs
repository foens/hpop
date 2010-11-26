using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenPop.Pop3
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
		/// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
		public static string ComputeHashHex(byte[] input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			using (System.Security.Cryptography.MD5 md5 = new MD5CryptoServiceProvider())
			{
				// Give the md5 function the bytes of the string, and get an hashed byte[] as output
				byte[] result = md5.ComputeHash(input, 0, input.Length);

				StringBuilder returnThis = new StringBuilder();

				// Convert the hashed value back into a string
				foreach (byte aByte in result)
					returnThis.Append(Uri.HexEscape((char)aByte));

				return returnThis.ToString().Replace("%", "").ToLowerInvariant();
			}
		}
	}
}