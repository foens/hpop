using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenPop.Mime.Decode;

namespace OpenPop.Mime.Header
{
	/// <summary>
	/// Class that hold information about one "Received:" header line.
	/// 
	/// Visit these RFCs for more information:
	/// <see href="http://tools.ietf.org/html/rfc5321#section-4.4">RFC 5321 section 4.4</see>
	/// <see href="http://tools.ietf.org/html/rfc4021#section-3.6.7">RFC 4021 section 3.6.7</see>
	/// <see href="http://tools.ietf.org/html/rfc2822#section-3.6.7">RFC 2822 section 3.6.7</see>
	/// <see href="http://tools.ietf.org/html/rfc2821#section-4.4">RFC 2821 section 4.4</see>
	/// </summary>
	public class Received
	{
		/// <summary>
		/// The date of this received line.
		/// </summary>
		public DateTime Date { get; private set; }

		/// <summary>
		/// A dictionary that contains the names and values of the
		/// received header line.
		/// </summary>
		/// <example>
		/// If the header lines looks like:
		/// <code>
		/// from sending.com (localMachine [127.0.0.1]) by test.net (Postfix)
		/// </code>
		/// then the dictionary will contain two keys: "from" and "by" with the values
		/// "sending.com (localMachine [127.0.0.1])" and "test.net (Postfix)".
		/// </example>
		public Dictionary<string, string> Names { get; private set; }

		/// <summary>
		/// The raw input string that was parsed into this class.
		/// </summary>
		public string Raw { get; private set; }

		/// <summary>
		/// Parses a Received header value.
		/// </summary>
		/// <param name="headerValue">The value for the header to be parsed</param>
		/// <exception cref="ArgumentNullException"><exception cref="ArgumentNullException">If <paramref name="headerValue"/> is <see langword="null"/></exception></exception>
		public Received(string headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			string datePart = headerValue.Substring(headerValue.LastIndexOf(';') + 1);
			
			Date = Rfc2822DateTime.StringToDate(datePart);
			Names = ParseDictionary(headerValue);
			Raw = headerValue;
		}

		/// <summary>
		/// Parses the Received header name-value-list into a dictionary.
		/// </summary>
		/// <param name="headerValue">The full header value for the Received header</param>
		/// <returns>A dictionary where the name-value-list has been parsed into</returns>
		private static Dictionary<string, string> ParseDictionary(string headerValue)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			// Remove the date part from the full headerValue
			string headerValueWithoutDate = headerValue.Substring(0, headerValue.LastIndexOf(';'));

			// The regex below should capture the following:
			// The name consists of non-whitespace characters followed by a whitespace and then the value follows.
			// There are multiple cases for the value part:
			//   1: Value is just some characters not including any whitespace
			//   2: Value is some characters, a whitespace followed by a
			//      parenthesized value which can contain whitespaces
			//   3: Value is some characters, a whitespace followed by to parenthesized values
			//      which can contain whitespaces and is delimited by whitespace.
			//      This case is believed to be illigal according to the RFC's, but nonetheless
			//      has been spotted in the wild.
			//
			// Cheat sheet for regex:
			// \s means every whitespace character
			// [^\s] means every character except whitespace characters
			// +? is a non-greedy equivalent of +
			const string pattern = @"(?<name>[^\s]+)\s(?<value>[^\s]+(\s\(.+?\)(\s\(.+?\))?)?)";

			// Find each match in the string
			MatchCollection matches = Regex.Matches(headerValueWithoutDate, pattern);
			foreach (Match match in matches)
			{
				// Add the name and value part found in the matched result to the dictionary
				string name = match.Groups["name"].Value;
				string value = match.Groups["value"].Value;
				dictionary.Add(name, value);
			}

			return dictionary;
		}
	}
}