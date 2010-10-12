using System;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenPOP.MIME.Decode
{
	/// <summary>
	/// Used for decoding Quoted-Printable text
	/// This is a robust implementation of a Quoted-Printable decoder defined in RFC 2047.
	/// Every measurement has been taken to conform to the RFC.
	/// </summary>
	/// <remarks>
	/// <a href="http://tools.ietf.org/html/rfc2047">RFC 2047</a> This is the RFC which the decoder conforms to.
	/// The RFC above overrides <a href="http://tools.ietf.org/html/rfc2045#section-6.7">RFC 2045</a> which originally
	/// defined what a Quoted-Printable string was.
	/// </remarks>
	internal static class QuotedPrintable
	{
		/// <summary>
		/// Decodes a Quoted-Printable string
		/// </summary>
		/// <param name="toDecode">Quoted-Printable encoded string</param>
		/// <param name="encoding">Specifies which encoding the returned string will be in</param>
		/// <returns>A decoded string in the correct encoding</returns>
		public static string Decode(string toDecode, Encoding encoding)
		{
			// Decode the QuotedPrintable string and return it
			return RFC2047QuotedPrintableDecode(toDecode, encoding);
		}

		/// <remarks>See <a href="http://tools.ietf.org/html/rfc2047">http://tools.ietf.org/html/rfc2047</a> for RFC details</remarks>
		/// <summary>
		/// This is the actual decoder
		/// </summary>
		/// <param name="toDecode">The string to be decoded from Quoted-Printable</param>
		/// <param name="encoding">The encoding to use when decoding</param>
		/// <returns>A decoded string</returns>
		private static string RFC2047QuotedPrintableDecode(string toDecode, Encoding encoding)
		{
			// Remove illegal control characters
			toDecode = removeIllegalControlCharacters(toDecode);

			// This will hold our decoded string
			StringBuilder builder = new StringBuilder();

			// Run through the whole string that needs to be decoded
			for (int i = 0; i < toDecode.Length; i++)
			{
				char currentChar = toDecode[i];
				if (currentChar == '=')
				{
					// Check that there is at least two characters behind the equal sign
					if (toDecode.Length - i < 3)
					{
						// We are at the end of the toDecode string, but something is missing. Handle it the way RFC 2045 states
						builder.Append(DecodeEqualSignNotLongEnough(toDecode.Substring(i)));

						// Since it was the last part, we should stop parsing anymore
						break;
					}

					// Decode the Quoted-Printable part
					string QuotedPrintablePart = toDecode.Substring(i, 3);
					builder.Append(DecodeEqualSign(QuotedPrintablePart, encoding));

					// We now consumed two extra characters. Go forward two extra characters
					i += 2;
				} else
				{
					// This character is not quoted printable hex encoded.

					// Could it be the _ character, which represents space?
					if (currentChar == '_')
						builder.Append(" ");
					else
						builder.Append(currentChar); // This is not encoded at all. This is a literal which should just be included into the output.
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// RFC 2045 states about robustness:
		/// Control characters other than TAB, or CR and LF as parts of CRLF pairs,
		/// must not appear. The same is true for octets with decimal values greater
		/// than 126.  If found in incoming quoted-printable data by a decoder, a
		/// robust implementation might exclude them from the decoded data and warn
		/// the user that illegal characters were discovered.
		/// 
		/// Control characters are defined in RFC 2396 as
		/// control = US-ASCII coded characters 00-1F and 7F hexadecimal
		/// </summary>
		/// <param name="input">String to be stripped from illegal control characters</param>
		/// <returns>A string with no illegal control characters</returns>
		private static string removeIllegalControlCharacters(string input)
		{
			// First we remove any \r or \n which is not part of a \r\n pair
			input = RemoveCarriageReturnAndNewLinewIfNotInPair(input);

			// Here only legal \r\n is left over
			// We now simply keep them, and the \t which is also allowed
			// \x0A = \n
			// \x0D = \r
			// \x09 = \t)
			return Regex.Replace(input, "[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
		}

		/// <summary>
		/// This method will remove any \r and \n which is not paired as \r\n
		/// </summary>
		/// <param name="input">String to remove lonely \r and \n's from</param>
		/// <returns>A string without lonely \r and \n's</returns>
		private static string RemoveCarriageReturnAndNewLinewIfNotInPair(string input)
		{
			// Use this for building up the new string. This is used for performance instead
			// of altering the input string each time a illegal token is found
			StringBuilder newString = new StringBuilder(input.Length);

			for (int i = 0; i < input.Length; i++)
			{
				// There is a character after it
				// Check for lonely \r
				// There is a lonely \r if it is the last character in the input or if there
				// is no \n following it
				if (input[i] == '\r' && (i + 1 >= input.Length || input[i + 1] != '\n'))
				{
					// Illegal token \r found. Do not add it to the new string

					// Check for lonely \n
					// There is a lonely \n if \n is the first character or if there
					// is no \r in front of it
				} else if (input[i] == '\n' && (i - 1 < 0 || input[i - 1] != '\r'))
				{
					// Illegal token \n found. Do not add it to the new string
				} else
				{
					// No illegal tokens found. Simply insert the character we are at
					// in our new string
					newString.Append(input[i]);
				}
			}

			return newString.ToString();
		}

		/// <summary>
		/// RFC 2045 says that a robust implementation should handle:
		/// An "=" cannot be the ultimate or penultimate character in an encoded
		/// object. This could be handled as in case (2) above.
		/// "
		/// 
		/// Case (2) is:
		/// An "=" followed by a character that is neither a
		/// hexadecimal digit (including "abcdef") nor the CR character of a CRLF pair
		/// is illegal.  This case can be the result of US-ASCII text having been
		/// included in a quoted-printable part of a message without itself having
		/// been subjected to quoted-printable encoding.  A reasonable approach by a
		/// robust implementation might be to include the "=" character and the
		/// following character in the decoded data without any transformation and, if
		/// possible, indicate to the user that proper decoding was not possible at
		/// this point in the data.
		/// </summary>
		/// <param name="decode"></param>
		/// <returns></returns>
		private static string DecodeEqualSignNotLongEnough(string decode)
		{
			// We can only decode wrong length equal signs
			if (decode.Length >= 3)
				throw new ArgumentException();

			// First char must be =
			if (decode[0] != '=')
				throw new ArgumentException();

			// We will now believe that the string sent to us, was actually not encoded
			return decode;
		}

		/// <summary>
		/// This helper method will decode a string of the form "=XX" where X is any character.
		/// This method will never fail, unless an argument of length not equal to three is passed
		/// </summary>
		/// <param name="decode">The length 3 character that needs to be decoded</param>
		/// <param name="encoding">The encoding to use when decoding</param>
		/// <returns>A decoded string</returns>
		private static string DecodeEqualSign(string decode, Encoding encoding)
		{
			// We can only decode the string if it has length 3 - other calls to this function is invalid
			if (decode.Length != 3)
				throw new ArgumentException();

			// First char must be =
			if (decode[0] != '=')
				throw new ArgumentException();

			// There are two cases where an equal sign might appear
			// It might be a
			//   - hex-string like =3D, denoting the character with hex value 3D
			//   - it might be the last character on the line before a CRLF
			//     pair, denoting a soft linebreak, which simply
			//     splits the text up, because of the 76 chars per line restriction
			if (decode.Contains("\r\n"))
			{
				// Soft break detected
				return "";
			}

			// Hex string detected. Convertion needed.
			// It might be that the string located after the equal sign is not hex characters
			// An example: =JU
			// In that case we would like to catch the FormatException and do something else
			try
			{
				// The number part of the string is the last two digits. Here we simply remove the equal sign
				string numberString = decode.Substring(1);

				// Now we create a byte array with the converted number encoded in the string as a hex value (base 16)
				// This will also handle illegal encodings like =3d where the hex digits are not uppercase,
				// which is a robustness requirement from RFC 2045We need a byte array to store our hex number in
				byte[] oneByte = new[] { Convert.ToByte(numberString, 16) };

				// Now, using our encoding, get back the string that this byte array corrosponds to
				// which is actually a one char string, but GetString does only take arrays, not a single byte
				// and therefore has no method that returns a single char
				return encoding.GetString(oneByte);
			} catch (FormatException)
			{
				// RFC 2045 says about robust implementation:
				// An "=" followed by a character that is neither a
				// hexadecimal digit (including "abcdef") nor the CR
				// character of a CRLF pair is illegal.  This case can be
				// the result of US-ASCII text having been included in a
				// quoted-printable part of a message without itself
				// having been subjected to quoted-printable encoding.  A
				// reasonable approach by a robust implementation might be
				// to include the "=" character and the following
				// character in the decoded data without any
				// transformation and, if possible, indicate to the user
				// that proper decoding was not possible at this point in
				// the data.

				// So we choose to believe this is actually an un-encoded string
				return decode;
			}
		}
	}
}