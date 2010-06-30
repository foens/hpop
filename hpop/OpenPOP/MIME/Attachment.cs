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
        /// Headers for this Attachment
        /// </summary>
        public MessageHeader Headers { get; private set; }

	    /// <summary>
		/// verify the attachment whether it is a real attachment or not
		/// </summary>
		/// <remarks>this is so far not comprehensive and needs more work to finish</remarks>
        public bool NotAttachment
        {
            get
            {
                if (Headers != null && (Headers.ContentType.MediaType == null || ContentFileName == "") && Headers.ContentID == null)
                    return true;
                return false;
            }
        }

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
	    /// Content File Name
	    /// </summary>
	    public string ContentFileName { get; set; }

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
        /// <param name="strFileName">file name</param>
        private Attachment(string strFileName)
        {
            // Setup defaults
            RawAttachment = null;
            RawContent = null;
            DefaultMIMEFileName = _defaultMIMEFileName;
            DefaultReportFileName = _defaultReportFileName;
            DefaultFileName2 = _defaultFileName2;
            DefaultFileName = _defaultFileName;

            // Setup parameters
            ContentFileName = strFileName;
        }

		/// <summary>
		/// Create an Attachment from byte contents. These are NOT parsed in any way, but assumed to be correct.
		/// This is used for MS-TNEF attachments
		/// </summary>
		/// <param name="bytAttachment">attachment bytes content</param>
		/// <param name="strFileName">file name</param>
		/// <param name="strContentType">content type</param>
		public Attachment(byte[] bytAttachment, string strFileName, string strContentType)
            : this(strFileName)
		{
            string bytesInAString = Encoding.Default.GetString(bytAttachment);
		    RawContent = bytesInAString;
		    RawAttachment = bytesInAString;
		    Headers = new MessageHeader(HeaderFieldParser.ParseContentType(strContentType));
		}

	    /// <summary>
	    /// Create an attachment from a string, with some headers use from the message it is inside
	    /// </summary>
	    /// <param name="strAttachment">attachment content</param>
	    /// <param name="headersFromMessage">The attachments headers defaults to some of the message headers, this is the headers from the message</param>
	    public Attachment(string strAttachment, MessageHeader headersFromMessage)
            : this("")
		{
            if (strAttachment == null)
                throw new ArgumentNullException("strAttachment");

            RawContent = strAttachment;

            string rawHeaders;
            NameValueCollection headers;
            HeaderExtractor.ExtractHeaders(strAttachment, out rawHeaders, out headers);

            Headers = new MessageHeader(headers, headersFromMessage.ContentType, headersFromMessage.ContentTransferEncoding);

            // If we parsed headers, as we just did, the RawAttachment is found by removing the headers and trimming
		    RawAttachment = Utility.ReplaceFirstOccurrance(strAttachment, rawHeaders, "");

            // Set the filename
            if (!string.IsNullOrEmpty(Headers.ContentType.Name))
                ContentFileName = Headers.ContentType.Name;
            else if(Headers.ContentDisposition != null)
                ContentFileName = Headers.ContentDisposition.FileName;
		}
        #endregion

	    /// <summary>
		/// Decode the attachment to text
		/// </summary>
		/// <returns>Decoded attachment text</returns>
		public string DecodeAsText()
		{
            try
            {
                if (Headers.ContentType.MediaType.ToLower().Equals("message/rfc822"))
                    return EncodedWord.Decode(RawAttachment);

                return Utility.DoDecode(RawAttachment, Headers.ContentTransferEncoding, Headers.ContentType.CharSet);
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
            return Encoding.Default.GetBytes(DecodeAsText());
		}

        public int CompareTo(Attachment attachment)
        {
            return RawAttachment.CompareTo(attachment.RawAttachment);
        }
	}
}