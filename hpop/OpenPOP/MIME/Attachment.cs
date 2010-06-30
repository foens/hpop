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
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;

namespace OpenPOP.MIME
{
	public class Attachment : IComparable<Attachment>
	{
		#region Member Variables
        private const string _defaultMIMEFileName = "body.eml";
        private const string _defaultReportFileName = "report.htm";
        private const string _defaultFileName2 = "body*.htm";
        private const string _defaultFileName = "body.htm";
	    #endregion

		#region Properties
	    /// <summary>
	    /// raw attachment content bytes
	    /// </summary>
	    public byte[] RawBytes { get; private set; }

	    /// <summary>
	    /// whether attachment is in bytes
	    /// </summary>
	    public bool InBytes { get; private set; }

	    /// <summary>
	    /// Content length
	    /// </summary>
	    public long ContentLength { get; private set; }

	    /// <summary>
		/// verify the attachment whether it is a real attachment or not
		/// </summary>
		/// <remarks>this is so far not comprehensive and needs more work to finish</remarks>
        public bool NotAttachment
        {
            get
            {
                if ((ContentType == null || ContentFileName == "") && ContentID == null)
                    return true;
                return false;
            }
        }

	    /// <summary>
	    /// Content format
	    /// </summary>
	    public string ContentFormat { get; private set; }

	    /// <summary>
	    /// Content charset
	    /// </summary>
	    public string ContentCharset { get; private set; }

	    /// <summary>
	    /// default file name
	    /// </summary>
	    public string DefaultFileName { get; set; }

	    /// <summary>
	    /// default file name 2
	    /// </summary>
	    public string DefaultFileName2 { get; set; }

	    /// <summary>
	    /// default report file name
	    /// </summary>
	    public string DefaultReportFileName { get; set; }

	    /// <summary>
	    /// default MIME File Name
	    /// </summary>
	    public string DefaultMIMEFileName { get; set; }

	    /// <summary>
	    /// Content Type
	    /// </summary>
	    public string ContentType { get; private set; }

	    /// <summary>
	    /// Content Transfer Encoding
	    /// </summary>
	    public string ContentTransferEncoding { get; private set; }

	    /// <summary>
	    /// Content Description
	    /// </summary>
	    public string ContentDescription { get; private set; }

	    /// <summary>
	    /// Content File Name
	    /// </summary>
	    public string ContentFileName { get; set; }

	    /// <summary>
	    /// Content Disposition
	    /// </summary>
	    public string ContentDisposition { get; private set; }

	    /// <summary>
	    /// Content ID
	    /// </summary>
	    public string ContentID { get; private set; }

	    /// <summary>
	    /// Raw Content
	    /// Full Attachment, with headers and everthing.
	    /// The raw string used to create this attachment
	    /// </summary>
	    public string RawContent { get; private set; }

	    /// <summary>
	    /// Raw Attachment Content (headers removed if was specified at creation)
	    /// </summary>
	    public string RawAttachment { get; private set; }
		#endregion

        #region Constructors
        /// <summary>
        /// Used to create a new attachment internally to avoid any
        /// duplicate code for setting up an attachment
        /// </summary>
        /// <param name="bytAttachment">attachment bytes content</param>
        /// <param name="lngFileLength">file length</param>
        /// <param name="strFileName">file name</param>
        /// <param name="strContentType">content type</param>
        /// <param name="blnInBytes">wheter attachment is in bytes</param>
        private Attachment(byte[] bytAttachment, long lngFileLength, string strFileName, string strContentType, bool blnInBytes)
        {
            // Setup defaults
            RawAttachment = null;
            RawContent = null;
            ContentID = null;
            ContentDisposition = null;
            ContentDescription = null;
            ContentTransferEncoding = null;
            DefaultMIMEFileName = _defaultMIMEFileName;
            DefaultReportFileName = _defaultReportFileName;
            DefaultFileName2 = _defaultFileName2;
            DefaultFileName = _defaultFileName;
            ContentCharset = null;
            ContentFormat = null;

            // Setup parameters
            InBytes = blnInBytes;
            RawBytes = bytAttachment;
            ContentLength = lngFileLength;
            ContentFileName = strFileName;
            ContentType = strContentType;
        }

		/// <summary>
		/// Create an Attachment from byte contents. These are NOT parsed in any way, but assumed to be correct.
		/// </summary>
		/// <param name="bytAttachment">attachment bytes content</param>
		/// <param name="lngFileLength">file length</param>
		/// <param name="strFileName">file name</param>
		/// <param name="strContentType">content type</param>
		public Attachment(byte[] bytAttachment, long lngFileLength, string strFileName, string strContentType)
            : this(bytAttachment, lngFileLength, strFileName, strContentType, true)
		{ }

		/// <summary>
		/// Create an attachment from a string, with some headers use from the message it is inside
		/// </summary>
		/// <param name="strAttachment">attachment content</param>
		public Attachment(string strAttachment)
            : this(null, 0, "", null, false)
		{
            if (strAttachment == null)
                throw new ArgumentNullException("strAttachment");

            // The attachment was not specified by using bytes
            InBytes = false;

            RawContent = strAttachment;

            string rawHeaders;
            NameValueCollection headers;
            HeaderExtractor.ExtractHeaders(strAttachment, out rawHeaders, out headers);

            // Now specificly parse each header. Some headers require special parsing.
            foreach (string headerName in headers.Keys)
            {
                string[] values = headers.GetValues(headerName);
                if (values != null)
                {
                    foreach (string headerValue in values)
                    {
                        // Parse the header
                        ParseHeader(headerName, headerValue);
                    }
                }
            }

            // If we parsed headers, as we just did, the RawAttachment is found by removing the headers and trimming
		    RawAttachment = Utility.ReplaceFirstOccurrance(strAttachment, rawHeaders, "");

            ContentLength = RawAttachment.Length;
		}
        #endregion

        private void ParseHeader(string headerName, string headerValue)
        {
            string[] values = Regex.Split(headerValue, ";");

            switch (headerName.ToUpper())
            {
                case "CONTENT-TYPE":
                    if (values.Length > 0)
                        ContentType = values[0].Trim();
                    if (values.Length > 1)
                    {
                        ContentCharset = Utility.GetQuotedValue(values[1], "=", "charset");
                    }
                    if (values.Length > 2)
                    {
                        ContentFormat = Utility.GetQuotedValue(values[2], "=", "format");
                    }
                    ContentFileName = Utility.ParseFileName(headerValue);
                    break;

                case "CONTENT-TRANSFER-ENCODING":
                    ContentTransferEncoding = values[0].Trim();
                    break;

                case "CONTENT-DESCRIPTION":
                    ContentDescription = EncodedWord.Decode(values[0].Trim());
                    break;

                case "CONTENT-DISPOSITION":
                    if (values.Length > 0)
                        ContentDisposition = values[0].Trim();

                    if (string.IsNullOrEmpty(ContentFileName))
                    {
                        if (values.Length > 1)
                            ContentFileName = values[1];
                        else
                            ContentFileName = "";

                        ContentFileName = ContentFileName.Replace("\t", "");
                        ContentFileName = Utility.GetQuotedValue(ContentFileName, "=", "filename");
                        ContentFileName = EncodedWord.Decode(ContentFileName);
                    }
                    break;

                case "CONTENT-ID":
                    ContentID = values[0].Trim('<').Trim('>');
                    break;
            }
        }

	    /// <summary>
		/// Decode the attachment to text
		/// </summary>
		/// <returns>Decoded attachment text</returns>
		public string DecodeAsText()
		{
            try
            {
                if (ContentType.ToLower() == "message/rfc822".ToLower())
                    return EncodedWord.Decode(RawAttachment);

                return Utility.DoDecode(RawAttachment, HeaderFieldParser.ParseContentTransferEncoding(ContentTransferEncoding), ContentCharset);
            }
            catch(Exception)
            {
                // TODO It seems that the only time there is an exception here, is when the Content-Transfer-Encoding is QuotedPrintable. So it seems there is a bug therein
                // TODO FIX QuotedPrintable and remove this try catch usage
                return RawAttachment;
            }
		}

		/// <summary>
		/// Decode attachment to be a message object
		/// </summary>
		/// <param name="blnRemoveHeaderBlankLine"></param>
		/// <param name="blnUseRawContent"></param>
		/// <returns>new message object</returns>
		public Message DecodeAsMessage(bool blnRemoveHeaderBlankLine, bool blnUseRawContent)
		{
		    string strContent = blnUseRawContent ? RawContent : RawAttachment;

            if (blnRemoveHeaderBlankLine && strContent.StartsWith("\r\n"))
                strContent = strContent.Substring(2, strContent.Length - 2);
		    return new Message(false, strContent, false);
		}

		/// <summary>
		/// Decode the attachment to bytes
		/// </summary>
		/// <returns>Decoded attachment bytes</returns>
		public byte[] DecodedAsBytes()
		{
            if (RawAttachment == null)
                return null;

            // Todo Figure out why we look at the ContentFileName...
            if (ContentFileName != "")
                return Encoding.Default.GetBytes(DecodeAsText());

		    return null;
		}

        public int CompareTo(Attachment attachment)
        {
            return RawAttachment.CompareTo(attachment.RawAttachment);
        }
	}
}