using System;
using System.Text;
using System.Text.RegularExpressions;
using OpenPop.Mime.Header;

namespace OpenPop.Mime.Decode
{
	/// <summary>
	/// Utility class for dealing with encoded word strings
	/// 
	/// EncodedWord encoded strings are only in ASCII, but can embed information
	/// about characters in other character sets.
	/// 
	/// It is done by specifying the character set, an encoding that maps from ASCII to
	/// the correct bytes and the actual encoded string.
	/// 
	/// It is specified in a format that is best summarized by a BNF:
	/// "=?" character_set "?" encoding "?" encoded-text "?="
	/// Example:
	/// =?ISO-8859-1?Q?=2D?=
	/// Here ISO-8859-1 is the character set
	/// Q is the encoding method (quoted-printable). B is also supported (Base 64).
	/// The encoded text is the =2D part which is decoded to a space.
	/// </summary>
	internal static class EncodedWord
	{
		/// <summary>
		/// Decode text that is encoded with the <see cref="EncodedWord"/> encoding.
		///
		/// This method will decode any encoded-word found in the string.
		/// All parts which is not encoded will not be touched.
		/// 
		/// From <a href="http://tools.ietf.org/html/rfc2047">RFC 2047</a>:
		/// Generally, an "encoded-word" is a sequence of printable ASCII
		/// characters that begins with "=?", ends with "?=", and has two "?"s in
		/// between.  It specifies a character set and an encoding method, and
		/// also includes the original text encoded as graphic ASCII characters,
		/// according to the rules for that encoding method.
		/// 
		/// Example:
		/// =?ISO-8859-1?q?this=20is=20some=20text?= other text here
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc2047#section-2">RFC 2047 section-2</a> "Syntax of encoded-words" for more details</remarks>
		/// <param name="encodedWords">Source text. May be content which is not encoded.</param>
		/// <returns>Decoded text</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="encodedWords"/> is <see langword="null"/></exception>
		public static string Decode(string encodedWords)
		{
			if(encodedWords == null)
				throw new ArgumentNullException("encodedWords");

			string decodedWords = encodedWords;

			// Notice that RFC2231 redefines the BNF to
			// encoded-word := "=?" charset ["*" language] "?" encoded-text "?="
			// but no usage of this BNF have been spotted yet. It is here to
			// ease debugging if such a case is discovered.

			// This is the regex that should fit the BNF
			// RFC Says that NO WHITESPACE is allowed in this encoding, but there are examples
			// where whitespace is there, and therefore this regex allows for such.
			const string strRegEx = @"\=\?(?<Charset>\S+?)\?(?<Encoding>\w)\?(?<Content>.+?)\?\=";
			// \w	Matches any word character including underscore. Equivalent to "[A-Za-z0-9_]".
			// \S	Matches any nonwhite space character. Equivalent to "[^ \f\n\r\t\v]".
			// +?   non-gready equivalent to +
			// (?<NAME>REGEX) is a named group with name NAME and regular expression REGEX

			MatchCollection matches = Regex.Matches(encodedWords, strRegEx);
			foreach (Match match in matches)
			{
				// If this match was not a success, we should not use it
				if (!match.Success) continue;

				string fullMatchValue = match.Value;

				string encodedText = match.Groups["Content"].Value;
				string encoding = match.Groups["Encoding"].Value;
				string charset = match.Groups["Charset"].Value;

				// Get the encoding which corrosponds to the character set
				Encoding charsetEncoding = HeaderFieldParser.ParseCharsetToEncoding(charset);

				// Store decoded text here when done
				string decodedText;

				// Encoding may also be written in lowercase
				switch (encoding.ToUpper())
				{
						// RFC:
						// The "B" encoding is identical to the "BASE64" 
						// encoding defined by RFC 2045.
					case "B":
						decodedText = Base64.Decode(encodedText, charsetEncoding);
						break;

						// RFC:
						// The "Q" encoding is similar to the "Quoted-Printable" content-
						// transfer-encoding defined in RFC 2045.
						// There are more details to this. Please check section 4.2 in
						// http://tools.ietf.org/html/rfc2047
					case "Q":
						decodedText = QuotedPrintable.Decode(encodedText, charsetEncoding);
						break;

					default:
						throw new ArgumentException("The encoding " + encoding + " was not recognized");
				}

				// Repalce our encoded value with our decoded value
				decodedWords = decodedWords.Replace(fullMatchValue, decodedText);
			}

			return decodedWords;
		}
	}
}