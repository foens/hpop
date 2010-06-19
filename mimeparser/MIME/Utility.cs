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
using System.Text.RegularExpressions;

namespace OpenPOP.MIMEParser
{
    /// <summary>
    /// foens: This class should be reworked.
    /// Right now it is just like "The blob" - which
    /// is lots of code that is totally unrelated to eachother
    /// </summary>
	public static class Utility
	{
	    private const string m_strLogFile = "OpenPOP.log";

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
		/// Parse date time info from MIME Date header
		/// </summary>
		/// <param name="strDate">Encoded MIME date time</param>
		/// <returns>Decoded date time info</returns>
		public static string ParseEmailDate(string strDate)
		{
			string strRet=strDate.Trim();
			int indexOfTag=strRet.IndexOf(",");
			if(indexOfTag!=-1)
			{
				strRet=strRet.Substring(indexOfTag+1);
			}

			strRet=QuoteText(strRet,"+");
			strRet=QuoteText(strRet,"-");
			strRet=QuoteText(strRet,"GMT");
			strRet=QuoteText(strRet,"CST");
			return strRet.Trim();
		}

		/// <summary>
		/// Quote the text according to a tag
		/// </summary>
		/// <param name="strText">Text to be quoted</param>
		/// <param name="strTag">Quote tag</param>
		/// <returns>Quoted Text</returns>
		public static string QuoteText(string strText, string strTag)
		{
			int indexOfTag=strText.IndexOf(strTag);
			if(indexOfTag!=-1)
				return strText.Substring(0,indexOfTag-1);
			
			return strText;
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
				strRet=RemoveQuote(strRet);
			}
			else
				strRet="";

			return strRet;
		}

		/// <summary>
		/// Parse email address from MIME header
        /// http://tools.ietf.org/html/rfc5322#section-3.4
		/// 
		/// Example of input:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// 
        /// It might also contain encoded text.
        /// A username in front of the emailaddress is not required.
        /// <see cref="DecodeText">For more information about encoded text</see>
		/// </summary>
		/// <param name="input">The value to parse out and email and/or a username</param>
        /// <param name="username">
        /// The decoded username in front.
        /// From the example this would be "Eksperten mailrobot"
        /// If there is no username, returned value will be empty.
        /// </param>
		/// <param name="emailAddress">
		/// The decoded email address.
        /// From the example this would be noreply@mail.eksperten.dk
        /// </param>
		public static void ParseEmailAddress(string input, out string username, out string emailAddress)
		{
            // Remove exesive whitespace
		    input = input.Trim();

            // Find the location of the email address
		    int indexStartEmail = input.LastIndexOf("<");
		    int indexEndEmail = input.LastIndexOf(">");

            if (indexStartEmail >= 0 && indexEndEmail >= 0)
			{
                // Check if there is a username in front of the email address
                if (indexStartEmail > 0)
                {
                    // Parse out the user
                    username = input.Substring(0, indexStartEmail).Trim();
                }
                else
                {
                    // There was no user
                    username = "";
                }

                // Parse out the email address without the "<"  and ">"
			    indexStartEmail = indexStartEmail + 1;
                int emailLength = indexEndEmail - indexStartEmail;
                emailAddress = input.Substring(indexStartEmail, emailLength);

                // Decode both values
                username = DecodeText(username);
                emailAddress = DecodeText(emailAddress);
			}
            else
            {
                // Check first to see, as a last resort, if it contains an email only
                if(input.Contains("@"))
                {
                    username = "";
                    emailAddress = input;
                } else
                {
                    // This must be a group name only then
                    username = input;
                    emailAddress = "";
                }
            }
		}

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

		/// <summary>
		/// Sepearte header name and header value
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
		/// Change text encoding
		/// </summary>
		/// <param name="strText">Source encoded text</param>
		/// <param name="strCharset">New charset</param>
		/// <returns>Encoded text with new charset</returns>
		public static string ChangeEncoding(string strText, string strCharset)
		{
			if (string.IsNullOrEmpty(strCharset))
				return strText;

			byte[] b = Encoding.Default.GetBytes(strText);
			return new string(Encoding.GetEncoding(strCharset).GetChars(b));
		}

		/// <summary>
		/// Remove non-standard base 64 characters
		/// </summary>
		/// <param name="strText">Source text</param>
		/// <returns>standard base 64 text</returns>
		public static string RemoveNonB64(string strText)
		{
			return strText.Replace("\0","");
		}

		/// <summary>
		/// Remove white blank characters
		/// </summary>
		/// <param name="strText">Source text</param>
		/// <returns>Text with white blanks</returns>
		public static string RemoveWhiteBlanks(string strText)
		{
			return strText.Replace("\0","").Replace("\r\n","");
		}

		/// <summary>
		/// Remove quotes
		/// </summary>
		/// <param name="strText">Text with quotes</param>
		/// <returns>Text without quotes</returns>
		public static string RemoveQuote(string strText)			
		{
			string strRet=strText;

			if(strRet.StartsWith("\""))
				strRet=strRet.Substring(1);
			if(strRet.EndsWith("\""))
				strRet=strRet.Substring(0,strRet.Length-1);

			return strRet;
		}

		/// <summary>
		/// Decode one line of text
		/// </summary>
		/// <param name="strText">Encoded text</param>
		/// <returns>Decoded text</returns>
		public static string DecodeLine(string strText)
		{
			return DecodeText(RemoveWhiteBlanks(strText));
		}

		/// <summary>
		/// Verifies wether the text is a valid MIME Text or not
		/// 
		/// foens: I do not belive the right term is "MIME Text"
		/// I think this is about checking if the text is encoded using the following BNF
        /// encoded-word = "=?" charset "?" encoding "?" encoded-text "?="
		/// </summary>
		/// <param name="strText">Text to be verified</param>
		/// <returns>True if MIME text, false if not</returns>
		private static bool IsValidMIMEText(string strText)
		{
		    int intPos = strText.IndexOf("=?");
		    return (intPos != -1 && strText.IndexOf("?=", intPos + 6) != -1 && strText.Length > 7);
		}

		/// <summary>
		/// Decode text that is encoded. See BNF below.
		/// This will decode any encoded-word found in the string.
		/// All unencoded parts will not be touched.	
		/// 
        /// From http://tools.ietf.org/html/rfc2047:
        /// Generally, an "encoded-word" is a sequence of printable ASCII
        /// characters that begins with "=?", ends with "?=", and has two "?"s in
        /// between.  It specifies a character set and an encoding method, and
        /// also includes the original text encoded as graphic ASCII characters,
        /// according to the rules for that encoding method.
        /// 
        /// BNF:
        /// encoded-word = "=?" charset "?" encoding "?" encoded-text "?="
        /// 
        /// Example:
        /// =?iso-8859-1?q?this=20is=20some=20text?= other text here
		/// </summary>
        /// <see cref="http://tools.ietf.org/html/rfc2047#section-2">RFC Part 2 "Syntax of encoded-words" for more detail</see>
		/// <param name="strText">Source text</param>
		/// <returns>Decoded text</returns>
		public static string DecodeText(string strText)
		{
            if(strText == null)
                return null;

		    string strRet = strText;
		    //string[] strParts = Regex.Split(strText, "\r\n");

            // This is the regex that should fit the BNF
            // RFC Says that NO WHITESPACE is allowed in this encoding. Again, see RFC for details.
		    const string strRegEx = @"\=\?(?<Charset>\S+?)\?(?<Encoding>\w)\?(?<Content>\S+?)\?\=";
            // \w	Matches any word character including underscore. Equivalent to "[A-Za-z0-9_]".
            // \S	Matches any nonwhite space character. Equivalent to "[^ \f\n\r\t\v]".
            // +?   non-gready equivalent to +

		    //for (int i = 0; i < strParts.Length; i++)
		    //{
                MatchCollection matches = Regex.Matches(strText, strRegEx);
		        foreach (Match match in matches)
		        {
		            if(match.Success)
		            {
		                string fullMatchValue = match.Value;

                        string encodedText = match.Groups["Content"].Value;
                        string encoding = match.Groups["Encoding"].Value;
                        string charset = match.Groups["Charset"].Value;

                        // Store decoded text here when done
                        string decodedText;

                        // Encoding may also be written in lowercase
                        switch (encoding.ToUpper())
                        {
                            // RFC:
                            // The "B" encoding is identical to the "BASE64" 
                            // encoding defined by RFC 2045.
                            case "B":
                                decodedText = deCodeB64s(encodedText, charset);
                                break;

                            // RFC:
                            // The "Q" encoding is similar to the "Quoted-Printable" content-
                            // transfer-encoding defined in RFC 2045.
                            // There are mo details to this. Please check section 4.2 in
                            // http://tools.ietf.org/html/rfc2047
                            case "Q":
                                try
                                {
                                    decodedText = DecodeQP.ConvertHexContent(encodedText, Encoding.GetEncoding(charset), 0);
                                }
                                catch (ArgumentException)
                                {
                                    // The encoding we are using is not supported.
                                    // Therefore we cannot decode it. We must simply return
                                    // the encoded form
                                    decodedText = fullMatchValue;
                                }
                                break;
                            default:
                                decodedText = encodedText;
                                break;
                        }

                        // Repalce our encoded value with our decoded value
		                strRet = strRet.Replace(fullMatchValue, decodedText);
		            }
		        //}
                /*
		        if (m.Success)
                {
                    // Fetch out the values
                    string encodedText = m.Groups["Content"].Value;
                    string encoding = m.Groups["Encoding"].Value;
                    string charset = m.Groups["Charset"].Value;

                    // Store decoded text here when done
                    string decodedText;

                    // Encoding may also be written in lowercase
                    switch (encoding.ToUpper())
                    {
                        // RFC:
                        // The "B" encoding is identical to the "BASE64" 
                        // encoding defined by RFC 2045.
                        case "B":
                            decodedText = deCodeB64s(encodedText, charset);
                            break;

                        // RFC:
                        // The "Q" encoding is similar to the "Quoted-Printable" content-
                        // transfer-encoding defined in RFC 2045.
                        // There are mo details to this. Please check section 4.2 in
                        // http://tools.ietf.org/html/rfc2047
                        case "Q":
                            decodedText = DecodeQP.ConvertHexContent(encodedText, Encoding.GetEncoding(charset), 0);
                            break;
                        default:
                            decodedText = encodedText;
                            break;
                    }

                    // There might be more text than just encoded text, therefore
                    // append what was in the source text, but with the encoded part removed
                    strRet += decodedText + strParts[i].Replace(m.Value, "");
                }
                else
                {
                    if (!IsValidMIMEText(strParts[i]))
                        strRet += strParts[i];
                    else
                    {
                        // foens: I do not be that to above if statement is needed.
                        // Therefore I throw this exception, and if anyone ever sees this exception, then it was actually needed
                        // If noone says that this exception has been thrown, remove above if.
                        throw new Exception("If you ever see this exception - please tell developers to remove this throw. Please give following too:\n" + strParts[i]);
                    }
                }
                 * */
		    }

		    return strRet;
		}
		
		public static string deCodeB64s(string strText)
		{
			return Encoding.Default.GetString(deCodeB64(strText));
		}
		
		public static string deCodeB64s(string strText,string strEncoding)
		{
			try
			{
				if(strEncoding.ToLower().Equals("ISO-8859-1".ToLower()))
					return deCodeB64s(strText);
				
				return Encoding.GetEncoding(strEncoding).GetString(deCodeB64(strText));
			}
			catch
			{
				return deCodeB64s(strText);
			}
		}
		
		private static byte[] deCodeB64(string strText)
		{
		    byte[] by = null;
			try
			{ 
				if(!"".Equals(strText))
				{
				    by = Convert.FromBase64String(strText);
				}
			} 
			catch(Exception e) 
			{
			    by = Encoding.Default.GetBytes("\0");
			    LogError("deCodeB64():" + e.Message);
			}
			return by;
		}

	    /// <summary>
	    /// Turns file logging on and off.
	    /// </summary>
	    /// <remarks>Comming soon.</remarks>
	    public static bool Log { get; set; }

	    internal static void LogError(string strText) 
		{
			//Log=true;
			if(Log)
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
					if(sw != null)
					{
						sw.Close();
					}
				}
			}
		}

		public static bool IsQuotedPrintable(string strText)
		{
		    if(strText != null)
				return strText.ToLower().Equals("quoted-printable");

		    return false;
		}

	    public static bool IsBase64(string strText)
		{
			if(strText != null)
				return strText.ToLower().Equals("base64");
			
			return false;
		}

		public static string[] SplitOnSemiColon(string strText)
		{
			if(strText==null)
				throw new ArgumentNullException("strText","Argument was null");

		    int indexOfColon=strText.IndexOf(";");

            if (indexOfColon == -1)
            {
                // Return string[] with single value
                return new[] { strText };
            }

		    string[] array = new string[2];
		    if (strText.Length > indexOfColon + 1)
		    {
		        array[0] = strText.Substring(0, indexOfColon).Trim();
		        array[1] = strText.Substring(indexOfColon + 1).Trim();
		    }

		    return array;
		}

		public static bool IsNotNullText(string strText)
		{
			return (!string.IsNullOrEmpty(strText));
		}

		public static bool IsNotNullTextEx(string strText)
		{
			return strText != null && !strText.Trim().Equals("");
		}

		public static bool IsOrNullTextEx(string strText)
		{
		    return strText == null || strText.Trim().Equals("");
		}
	}
}