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
using System.Text;
using System.IO;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;

namespace OpenPOP.MIME
{
    /// <summary>
    /// foens: This class should be reworked.
    /// Right now it is just like "The blob" - which
    /// is lots of code that is totally unrelated to eachother
    /// </summary>
	public static class Utility
    {
        #region Logging
        private const string m_strLogFile = "OpenPOP.log";

        /// <summary>
        /// Turns file logging on and off.
        /// </summary>
        /// <remarks>Comming soon.</remarks>
        public static bool Log { get; set; }

        internal static void LogError(string strText)
        {
            //Log=true;
            if (Log)
            {
                FileInfo file;
                StreamWriter sw = null;
                try
                {
                    file = new FileInfo(m_strLogFile);
                    sw = file.AppendText();
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine(strText);
                    sw.WriteLine("\r\n");
                    sw.Flush();
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                    }
                }
            }
        }
        #endregion

        /// <summary>
		/// Verifies whether the filename is of picture type or not by
		/// checking what the extension is
		/// </summary>
		/// <param name="filename">Filename to be checked</param>
		/// <returns>True if filename is of picture type, false if not</returns>
		public static bool IsPictureFile(string filename)
		{
			if(!string.IsNullOrEmpty(filename))
			{
				filename = filename.ToLower();
				if(filename.EndsWith(".jpg") ||
                    filename.EndsWith(".bmp") ||
                    filename.EndsWith(".ico") ||
                    filename.EndsWith(".gif") ||
                    filename.EndsWith(".png"))
					return true;
				return false;
			}
			return false;
		}

        /// <summary>
		/// Parse file name from MIME header
		/// </summary>
		/// <param name="strHeader">MIME header</param>
		/// <returns>Decoded file name</returns>
		public static string ParseFileName(string strHeader)
		{
		    string strTag = "filename=";
			int intPos=strHeader.ToLower().IndexOf(strTag);
			if(intPos==-1)
			{
				strTag="name=";
				intPos=strHeader.ToLower().IndexOf(strTag);
			}
			string strRet;
			if(intPos!=-1)
			{
				strRet=strHeader.Substring(intPos+strTag.Length);
				intPos=strRet.ToLower().IndexOf(";");
				if(intPos!=-1)
					strRet=strRet.Substring(1,intPos-1);
				strRet=RemoveQuotes(strRet);
			}
			else
				strRet="";

			return strRet;
		}

        #region Saving/loading to/from files
        /// <summary>
		/// Save byte content to a file.
		/// If file exists it is deleted!
		/// </summary>
		/// <param name="strFile">File to be saved to</param>
		/// <param name="bytContent">Byte array content</param>
		/// <returns>True if saving succeeded, false if failed</returns>
		public static bool SaveByteContentToFile(string strFile,byte[] bytContent)
		{
			try
			{
				if(File.Exists(strFile))
					File.Delete(strFile);
				FileStream fs = File.Create(strFile);
				fs.Write(bytContent, 0, bytContent.Length);
                fs.Flush();
				fs.Close();
				return true;
			}
			catch(Exception e)
			{
				LogError("SaveByteContentToFile():" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Save text content to a file
		/// </summary>
		/// <param name="strFile">File to be saved to</param>
		/// <param name="strText">Text content</param>
		/// <param name="blnReplaceExists">Replace file if exists</param>
		/// <returns>True if saving succeeded, false if failed</returns>
		public static bool SavePlainTextToFile(string strFile, string strText, bool blnReplaceExists)
		{
			try
			{
				if(File.Exists(strFile))
				{
					if(blnReplaceExists)
						File.Delete(strFile);
					else
					    return false; // Failure. File exist but we may not delete it
				}

				StreamWriter sw = File.CreateText(strFile);
				sw.Write(strText);
                sw.Flush();
				sw.Close();

				return true; // Success
			}
			catch(Exception e)
			{
				LogError("SavePlainTextToFile():" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Read text content from a file
		/// </summary>
		/// <param name="strFile">File to be read from</param>
		/// <param name="strText">This is where the content of the file is placed</param>
		/// <returns>True if reading succeeded, false if failed</returns>
		public static bool ReadPlainTextFromFile(string strFile, ref string strText)
		{
			if(File.Exists(strFile))
			{
				StreamReader fs = new StreamReader(strFile);
				strText = fs.ReadToEnd();
				fs.Close();
				return true;
			}
			
			return false;
		}
        #endregion

		/// <summary>
		/// Seperate header name and header value
		/// </summary>
		public static string[] GetHeadersValue(string strRawHeader)
		{
			if(strRawHeader==null)
				throw new ArgumentNullException("strRawHeader", "Argument was null");

			string[] array = new[] {"",""};
			int indexOfColon=strRawHeader.IndexOf(":");

            // Check if it is allowed to make substring calls
            if(indexOfColon >= 0 && strRawHeader.Length > indexOfColon+1)
            {
                array[0] = strRawHeader.Substring(0, indexOfColon).Trim();
                array[1] = strRawHeader.Substring(indexOfColon + 1).Trim();
            }

			return array;
		}

		/// <summary>
		/// Get quoted text
		/// </summary>
		/// <param name="strText">Text with quotes</param>
		/// <param name="strSplitter">Splitter</param>
		/// <param name="strTag">Target tag</param>
		/// <returns>Text without quote</returns>
		public static string GetQuotedValue(string strText, string strSplitter, string strTag)
		{
			if(strText == null)
				throw new ArgumentNullException("strText", "Argument was null");

			string[] array = new[] {"",""};
			int indexOfstrSplitter=strText.IndexOf(strSplitter);

            if (indexOfstrSplitter >= 0 && strText.Length > indexOfstrSplitter + 1)
            {
                array[0] = strText.Substring(0, indexOfstrSplitter).Trim();
                array[1] = strText.Substring(indexOfstrSplitter + 1).Trim();

                // Check for quoted value
                int pos = array[1].IndexOf("\"");
                if (pos != -1 && array[1].Length > pos + 1)
                {
                    int pos2 = array[1].IndexOf("\"", pos + 1);
                    if (pos2 != -1)
                        array[1] = array[1].Substring(pos + 1, pos2 - pos - 1);
                }
            }

			if(array[0].ToLower() == strTag.ToLower())
				return array[1].Trim();
			
			return null;
		}

        /// <summary>
		/// Remove quotes
		/// </summary>
		/// <param name="strText">Text with quotes</param>
		/// <returns>Text without quotes</returns>
		public static string RemoveQuotes(string strText)			
		{
			string strRet=strText;

			if(strRet.StartsWith("\""))
				strRet=strRet.Substring(1);
			if(strRet.EndsWith("\""))
				strRet=strRet.Substring(0,strRet.Length-1);

			return strRet;
		}

        public static bool IsOrNullTextEx(string strText)
		{
		    return strText == null || strText.Trim().Equals("");
		}

        public static string DoDecode(string input, ContentTransferEncoding contentTransferEncoding, string charSet)
        {
            switch (contentTransferEncoding)
            {
                case ContentTransferEncoding.QuotedPrintable:
                    if (!string.IsNullOrEmpty((charSet)))
                        return QuotedPrintable.ConvertHexContent(input, Encoding.GetEncoding(charSet), 0);

                    return QuotedPrintable.ConvertHexContent(input);

                case ContentTransferEncoding.Base64:
                    return Base64.Decode(input);

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
        /// <param name="strText">Source encoded text</param>
        /// <param name="strCharset">New charset</param>
        /// <returns>Encoded text with new charset</returns>
        private static string ChangeEncoding(string strText, string strCharset)
        {
            if (string.IsNullOrEmpty(strCharset))
                return strText;

            byte[] b = Encoding.Default.GetBytes(strText);
            return Encoding.GetEncoding(strCharset).GetString(b);
        }

        /// <summary>
        /// Replace the first occurence of a string in a string
        /// </summary>
        /// <param name="original">The original string to replace in</param>
        /// <param name="toReplace">The string that is to be replaced</param>
        /// <param name="toReplaceWith">The string that is to be placed instead of the replaced string</param>
        /// <returns>
        /// The original string with the first occurrance of toReplace replaced with toReplaceWith.
        /// The original is returned if toReplace was not found.
        /// </returns>
        /// <see cref="http://fortycal.blogspot.com/2007/07/replace-first-occurrence-of-string-in-c.html">For author</see>
        public static string ReplaceFirstOccurrance(string original, string toReplace, string toReplaceWith)
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