using System;
using System.Collections.Generic;

namespace OpenPop.Mime.Decode
{
	/// <summary>
	/// Contains common operations needed while decoding.
	/// </summary>
	internal static class Utility
	{
		/// <summary>
		/// Separate header name and header value.
		/// </summary>
		/// <exception cref="ArgumentNullException">If <paramref name="rawHeader"/> is <see langword="null"/></exception>
		public static string[] GetHeadersValue(string rawHeader)
		{
			if (rawHeader == null)
				throw new ArgumentNullException("rawHeader");

			string[] array = new[] {string.Empty, string.Empty};
			int indexOfColon = rawHeader.IndexOf(':');

			// Check if it is allowed to make substring calls
			if (indexOfColon >= 0 && rawHeader.Length >= indexOfColon + 1)
			{
				array[0] = rawHeader.Substring(0, indexOfColon).Trim();
				array[1] = rawHeader.Substring(indexOfColon + 1).Trim();
			}

			return array;
		}

		/// <summary>
		/// Remove quotes, if found, around the string.
		/// </summary>
		/// <param name="text">Text with quotes or without quotes</param>
		/// <returns>Text without quotes</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/></exception>
		public static string RemoveQuotesIfAny(string text)
		{
			if(text == null)
				throw new ArgumentNullException("text");

			string returner = text;

			if (returner.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
				returner = returner.Substring(1);
			if (returner.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
				returner = returner.Substring(0, returner.Length - 1);

			return returner;
		}

		/// <summary>
		/// Split a string into a list of strings using a specified character.<br/>
		/// Everything inside quotes are ignored.
		/// </summary>
		/// <param name="input">A string to split</param>
		/// <param name="toSplitAt">The character to use to split with</param>
		/// <returns>A List of strings that was delimited by the <paramref name="toSplitAt"/> character</returns>
		public static List<string> SplitStringWithCharNotInsideQuotes(string input, char toSplitAt)
		{
			List<string> elements = new List<string>();

			int lastSplitLocation = 0;
			bool insideQuote = false;

			char[] characters = input.ToCharArray();

			for (int i = 0; i < characters.Length; i++)
			{
				char character = characters[i];
				if (character == '\"')
					insideQuote = !insideQuote;

				// Only split if we are not inside quotes
				if (character == toSplitAt && !insideQuote)
				{
					// We need to split
					int length = i - lastSplitLocation;
					elements.Add(input.Substring(lastSplitLocation, length));

					// Update last split location
					// + 1 so that we do not include the character used to split with next time
					lastSplitLocation = i + 1;
				}
			}

			// Add the last part
			elements.Add(input.Substring(lastSplitLocation, input.Length - lastSplitLocation));

			return elements;
		}
	}
}