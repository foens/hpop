using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace OpenPOP.MIME.Header
{
	///<summary>
	/// Includes methods that pulls out the headers of an email message.
	///</summary>
	internal static class HeaderExtractor
	{
		/// <summary>
		/// Method that takes a full message and extract the headers from it.
		/// </summary>
		/// <param name="message">The message to extract headers from</param>
		/// <param name="rawHeaders">The portion of the message that was headers</param>
		/// <param name="headers">A collection of Name and Value pairs of headers</param>
		public static void ExtractHeaders(string message, out string rawHeaders, out NameValueCollection headers)
		{
			if(message == null)
				throw new ArgumentNullException("message");

			headers = new NameValueCollection();
			StringBuilder rawHeadersBuilder = new StringBuilder();

			StringReader messageReader = new StringReader(message);

			// Read until all headers have ended. It ends with an empty line
			string line;
			while (!"".Equals(line = messageReader.ReadLine()))
			{
				rawHeadersBuilder.Append(line + "\r\n");

				// Split into name and value
				string[] splittedValue = Utility.GetHeadersValue(line);
				string headerName = splittedValue[0];
				string headerValue = splittedValue[1];

				// Keep reading until we would hit next header
				// This if for handling multi line headers
				while (IsMoreLinesInHeaderValue(messageReader))
				{
					// Unfolding is accomplished by simply removing any CRLF
					// that is immediately followed by WSP
					// This was done using ReadLine
					string moreHeaderValue = messageReader.ReadLine();

					// if this exception is ever raised, there is an algorithm failure
					// IsMoreLinesInHeaderValue should not return true if the next line
					// does not exist
					if (moreHeaderValue == null)
						throw new NullReferenceException("This should never happen, since IsMoreLinesInHeaderValue method call was true");

					headerValue += moreHeaderValue.Substring(1); // Remove the first whitespace

					rawHeadersBuilder.Append(moreHeaderValue + "\r\n");
				}

				// Now we have the name and full value. Add it
				headers.Add(headerName, headerValue);
				
			}

			// Set the out parameter to our raw header. Remember to remove the last line ending.
			rawHeaders = rawHeadersBuilder.ToString().TrimEnd(new[] {'\r', '\n'});
		}

		/// <summary>
		/// Check if the next line is part of the current header value we are parsing by
		/// peeking on the next character of the <see cref="TextReader"/>.
		/// This should only be called while parsing headers
		/// </summary>
		/// <param name="reader">The reader from which the header is read from</param>
		/// <returns><see langword="true"/> if multi-line header. <see langword="false"/> otherwise</returns>
		private static bool IsMoreLinesInHeaderValue(TextReader reader)
		{
			int peek = reader.Peek();
			if (peek == -1)
				return false;

			char peekChar = (char)peek;

			// A multi line header must have a whitespace character
			// on the next line if it is to be continued
			return peekChar == ' ' || peekChar == '\t';
		}
	}
}