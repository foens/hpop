using System;
using System.Text;
using System.IO;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME
{
	/// <summary>
	/// foens: This class should be reworked.
	/// Right now it is just like "The blob" - which
	/// is lots of code that is totally unrelated to each other
	/// 
	/// The class should, in time, be made internal
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Verifies whether the filename is of picture type or not by
		/// checking what the extension is
		/// </summary>
		/// <param name="filename">Filename to be checked</param>
		/// <returns><see langword="true"/> if filename is of picture type, <see langword="false"/> if not</returns>
		public static bool IsPictureFile(string filename)
		{
			if (!string.IsNullOrEmpty(filename))
			{
				filename = filename.ToLower();
				if (filename.EndsWith(".jpg") ||
				    filename.EndsWith(".bmp") ||
				    filename.EndsWith(".ico") ||
				    filename.EndsWith(".gif") ||
				    filename.EndsWith(".png"))
					return true;
				return false;
			}
			return false;
		}

		#region Saving/loading to/from files
		/// <summary>
		/// Save byte content to a file.
		/// If file exists it is deleted!
		/// </summary>
		/// <param name="file">File to be saved to</param>
		/// <param name="contents">Byte array content</param>
		/// <returns><see langword="true"/> if saving succeeded, <see langword="false"/> if failed</returns>
		public static bool SaveByteContentToFile(FileInfo file, byte[] contents)
		{
			try
			{
				if (file.Exists)
					file.Delete();
				using (FileStream fs = file.Create())
				{
					fs.Write(contents, 0, contents.Length);
				}

				return true;
			} catch (Exception e)
			{
				DefaultLogger.CreateLogger().LogError("SaveByteContentToFile():" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Save text content to a file
		/// </summary>
		/// <param name="file">File to be saved to</param>
		/// <param name="content">Text content</param>
		/// <param name="replaceFileIfExists">Replace file if exists</param>
		/// <returns><see langword="true"/> if saving succeeded, <see langword="false"/> if failed</returns>
		public static bool SavePlainTextToFile(FileInfo file, string content, bool replaceFileIfExists)
		{
			try
			{
				if (file.Exists)
				{
					if (replaceFileIfExists)
						file.Delete();
					else
						return false; // Failure. File exist but we may not delete it
				}

				using (StreamWriter sw = new StreamWriter(file.Create()))
				{
					sw.Write(content);
				}

				return true; // Success
			} catch (Exception e)
			{
				DefaultLogger.CreateLogger().LogError("SavePlainTextToFile():" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Read text content from a file
		/// </summary>
		/// <param name="file">File to be read from</param>
		/// <param name="content">This is where the content of the file is placed</param>
		/// <returns><see langword="true"/> if reading succeeded, <see langword="false"/> if failed</returns>
		public static bool ReadPlainTextFromFile(FileInfo file, ref string content)
		{
			if (file.Exists)
			{
				using (StreamReader fs = new StreamReader(file.OpenRead()))
				{
					content = fs.ReadToEnd();
				}
				return true;
			}

			return false;
		}
		#endregion

		/// <summary>
		/// Separate header name and header value
		/// </summary>
		public static string[] GetHeadersValue(string rawHeader)
		{
			if (rawHeader == null)
				throw new ArgumentNullException("rawHeader", "Argument was null");

			string[] array = new[] {"", ""};
			int indexOfColon = rawHeader.IndexOf(":");

			// Check if it is allowed to make substring calls
			if (indexOfColon >= 0 && rawHeader.Length > indexOfColon + 1)
			{
				array[0] = rawHeader.Substring(0, indexOfColon).Trim();
				array[1] = rawHeader.Substring(indexOfColon + 1).Trim();
			}

			return array;
		}

		/// <summary>
		/// Remove quotes
		/// </summary>
		/// <param name="text">Text with quotes</param>
		/// <returns>Text without quotes</returns>
		public static string RemoveQuotes(string text)
		{
			string returner = text;

			if (returner.StartsWith("\""))
				returner = returner.Substring(1);
			if (returner.EndsWith("\""))
				returner = returner.Substring(0, returner.Length - 1);

			return returner;
		}

		/// <summary>
		/// Checks to see if a string is null, empty, only whitespace
		/// </summary>
		/// <param name="text">The string to check</param>
		/// <returns>Returns True if <paramref name="text"/> is <see langword="null"/>, empty, or contains only whitespace.</returns>
		public static bool IsOrNullTextEx(string text)
		{
			return text == null || text.Trim().Equals("");
		}

		/// <summary>
		/// Decodes a string based upon the Content Transfer encoding
		/// </summary>
		/// <param name="input">The string to decode</param>
		/// <param name="contentTransferEncoding">The <see cref="ContentTransferEncoding"/> of the string</param>
		/// <param name="charSet">The name of the character set encoding</param>
		/// <returns>The decoded string</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="contentTransferEncoding"/> is unsupported</exception>
		public static string DoDecode(string input, ContentTransferEncoding contentTransferEncoding, string charSet)
		{
			switch (contentTransferEncoding)
			{
				case ContentTransferEncoding.QuotedPrintable:
					if (!string.IsNullOrEmpty((charSet)))
						return QuotedPrintable.Decode(input, Encoding.GetEncoding(charSet));
					return QuotedPrintable.Decode(input, Encoding.Default);

				case ContentTransferEncoding.Base64:
					try
					{
						return Base64.Decode(input, charSet);
					} catch (Exception)
					{
						// We cannot decode it.Simply return the encoded form.
						return input;
					}

				case ContentTransferEncoding.SevenBit:
				case ContentTransferEncoding.Binary:
				case ContentTransferEncoding.EightBit:
					if (!string.IsNullOrEmpty(charSet))
						return ChangeEncoding(input, charSet);

					// Nothing needed to be done
					return input;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Change text encoding
		/// </summary>
		/// <param name="text">Source encoded text</param>
		/// <param name="newEncoding">New charset</param>
		/// <returns>Encoded text with new charset</returns>
		private static string ChangeEncoding(string text, string newEncoding)
		{
			if (string.IsNullOrEmpty(newEncoding))
				return text;

			byte[] bytes = Encoding.Default.GetBytes(text);
			return Encoding.GetEncoding(newEncoding).GetString(bytes);
		}

		/// <summary>
		/// Replace the first occurrence of a string in a string
		/// </summary>
		/// <param name="original">The original string to replace in</param>
		/// <param name="toReplace">The string that is to be replaced</param>
		/// <param name="toReplaceWith">The string that is to be placed instead of the replaced string</param>
		/// <returns>
		/// The original string with the first occurrence of <paramref name="toReplace"/> replaced with <paramref name="toReplaceWith"/>.
		/// The original is returned if <paramref name="toReplace"/> was not found.
		/// </returns>
		/// <remarks><a href="http://fortycal.blogspot.com/2007/07/replace-first-occurrence-of-string-in-c.html">See For author</a></remarks>
		public static string ReplaceFirstOccurrence(string original, string toReplace, string toReplaceWith)
		{
			if (String.IsNullOrEmpty(original))
				return String.Empty;
			if (String.IsNullOrEmpty(toReplace))
				return original;
			if (String.IsNullOrEmpty(toReplaceWith))
				toReplaceWith = String.Empty;

			int loc = original.IndexOf(toReplace);

			if (loc == -1)
				return original;

			return original.Remove(loc, toReplace.Length).Insert(loc, toReplaceWith);
		}
	}
}