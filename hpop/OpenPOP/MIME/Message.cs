using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.IO;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;

namespace OpenPOP.MIME
{
	/// <summary>
	/// The class represents a MIME Message
	/// </summary>
	public class Message
	{
		#region Properties
        /// <summary>
	    /// Whether to auto decode MS-TNEF attachment files
	    /// </summary>
        private bool AutoDecodeMSTNEF { get; set; }

        /// <summary>
        /// Headers of the Message.
        /// </summary>
        public MessageHeader Headers { get; private set; }

        /// <summary>
        /// These are the text/plain and text/html bodies that could be found in the message
        /// The last message should be the message most faithfull to what the user sent
        /// Commonly the second message is HTML and the first is plain text
        /// </summary>
	    public List<string> MessageBody { get; private set; }

        /// <summary>
        /// Attachments for the Message
        /// </summary>
	    public List<Attachment> Attachments { get; private set; }

	    /// <summary>
	    /// The raw message body part of the RawMessage that this message was constructed with.
	    /// The Raw message is simply the message body part of the message, but the message body has NOT
	    /// been decoded or converted in any way.
	    /// You properly want to <see cref="MessageBody"/> instead.
	    /// </summary>
	    public string RawMessageBody { get; private set; }

	    /// <summary>
	    /// The header part from the RawMessage that this message was constructed with
	    /// </summary>
	    public string RawHeader { get; private set; }

	    /// <summary>
	    /// The raw content from which this message has been constructed
	    /// </summary>
	    public string RawMessage { get; private set; }
	    #endregion

        #region Constructors
        /// <summary>
        /// Sets up a default new message
        /// </summary>
        private Message()
        {
            RawMessage = null;
            RawHeader = null;
            RawMessageBody = null;
            Attachments = new List<Attachment>();
            MessageBody = new List<string>();
            AutoDecodeMSTNEF = false;
        }

        /// <summary>
        /// Initializes a message from a .eml file
        /// </summary>
        /// <param name="blnAutoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
        /// <param name="blnOnlyHeader">whether only decode the header without body</param>
        /// <param name="strEMLFile">File with email content to load from</param>
        public Message(bool blnAutoDecodeMSTNEF, bool blnOnlyHeader, string strEMLFile)
            : this()
        {
            string strMessage = null;
            if (Utility.ReadPlainTextFromFile(strEMLFile, ref strMessage))
            {
                AutoDecodeMSTNEF = blnAutoDecodeMSTNEF;
                InitializeMessage(strMessage, blnOnlyHeader);
            } else
            {
                throw new FileNotFoundException("Could not find file " + strEMLFile);
            }
        }

		/// <summary>
		/// Creates a new message from a string
		/// </summary>
		/// <param name="blnAutoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="strMessage">raw message content</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		public Message(bool blnAutoDecodeMSTNEF, string strMessage, bool blnOnlyHeader)
            : this()
		{
		    AutoDecodeMSTNEF = blnAutoDecodeMSTNEF;
		    InitializeMessage(strMessage,blnOnlyHeader);
		}
        #endregion

        #region Public functions
		/// <summary>
		/// Verify if the message is a report
		/// </summary>
		/// <returns>true if message is a report message, false otherwise</returns>
		public bool IsReport()
		{
			if(!string.IsNullOrEmpty(Headers.ContentType.MediaType))
                return (Headers.ContentType.MediaType.ToLower().IndexOf("report".ToLower()) != -1);
			
			return false;
		}

		/// <summary>
		/// translate pictures url within the body
		/// </summary>
		/// <param name="strBody">message body</param>
		/// <param name="hsbFiles">pictures collection</param>
		/// <returns>translated message body</returns>
		public string TranslateHTMLPictureFiles(string strBody, Hashtable hsbFiles)
		{
			foreach(Attachment attachment in Attachments)
			{
				if(Utility.IsPictureFile(attachment.ContentFileName))
				{
                    if (!string.IsNullOrEmpty(attachment.Headers.ContentID))
                        strBody = strBody.Replace("cid:" + attachment.Headers.ContentID, hsbFiles[attachment.ContentFileName].ToString());
                    else
                        strBody = strBody.Replace(attachment.ContentFileName, hsbFiles[attachment.ContentFileName].ToString());
				}
			}
			
			return strBody;
		}

		/// <summary>
		/// Translate inline pictures within the body to a path where the images are saved
		/// under their ContentFileName.
		/// </summary>
		/// <param name="strBody">The body to be changedy</param>
		/// <param name="strPath">Path to the location of the pictures</param>
		/// <returns>A Translated message body</returns>
		public string TranslateHTMLPictureFiles(string strBody, string strPath)
		{
			if(!strPath.EndsWith("\\"))
				strPath += "\\";
			
			foreach(Attachment attachment in Attachments)
			{
				if(Utility.IsPictureFile(attachment.ContentFileName))
				{
                    if (!string.IsNullOrEmpty(attachment.Headers.ContentID))
                        strBody = strBody.Replace("cid:" + attachment.Headers.ContentID, strPath + attachment.ContentFileName);
                    else
					    strBody = strBody.Replace(attachment.ContentFileName, strPath + attachment.ContentFileName);
				}
			}

			return strBody;
		}

		/// <summary>
		/// Save all Attachments included in this message to a defined path.
		/// The attachments name will be appended to the path, and saved under that name.
		/// </summary>
		/// <param name="strPath">Path to place the attachments</param>
		/// <returns>true if all attachments was saved successfully, false if just one failed</returns>
		public bool SaveAttachments(string strPath)
		{
            if (string.IsNullOrEmpty(strPath))
                return false;
			
			try
			{
                bool blnRet = true;

				if(!strPath.EndsWith("\\"))
					strPath += "\\";
				
                foreach (Attachment attachment in Attachments)
                {
                    blnRet = attachment.SaveToFile(strPath + attachment.ContentFileName);
                    if (blnRet == false)
                        break;
                }
				return blnRet;
			}
			catch(Exception e)
			{
				Utility.LogError(e.Message);
				return false;
			}
        }

        /// <summary>
        /// Save message content to an eml file
        /// </summary>
        /// <param name="strFile">The File location to save the message to</param>
        /// <param name="blnReplaceExists">Should the file be replaced if it exists?</param>
        /// <returns>True on success, false otherwsie</returns>
        public bool SaveToMIMEEmailFile(string strFile, bool blnReplaceExists)
        {
            return Utility.SavePlainTextToFile(strFile, RawMessage, blnReplaceExists);
        }
        #endregion

        #region Main parser function
        /// <summary>
        /// Initializes a new message from raw MIME content.
        /// This method parses headers, messagebody and attachments.
        /// </summary>
        /// <param name="input">Raw message content from which parsing will begin</param>
        /// <param name="onlyParseHeaders">Whether only to parse and decode headers</param>
        private void InitializeMessage(string input, bool onlyParseHeaders)
        {
            // Keep the raw message for later usage
            RawMessage = input;

            // Genericly parse out header names and values
            // Also include the rawHeader text for later use
            string rawHeadersTemp;
            NameValueCollection headersUnparsedCollection;
            HeaderExtractor.ExtractHeaders(input, out rawHeadersTemp, out headersUnparsedCollection);
            RawHeader = rawHeadersTemp;

            // Parse the headers
            Headers = new MessageHeader(headersUnparsedCollection);

            if (onlyParseHeaders == false)
            {
                // The message body must be the full raw message, with headers removed.
                // Also remove any CRLF in top or bottom.
                RawMessageBody = Utility.ReplaceFirstOccurrance(RawMessage, RawHeader, "").Trim();

                // Check if the message is a multipart message (which means, has multiple message bodies)
                if (Headers.ContentType.MediaType.ToLower().Contains("multipart"))
                {
                    // Set up attachments
                    ParseMultipartMessageBody();

                    // Some of the attachments can be text and html, these we want in our MessageBody instead
                    if (Attachments.Count > 0)
                    {
                        List<Attachment> toRemoveFromAttachments = new List<Attachment>();

                        // Check if the first attachment is the message
                        foreach (Attachment attachment in Attachments)
                        {
                            if (attachment.Headers.ContentType != null &&
                                (attachment.Headers.ContentType.MediaType.Contains("text/plain") ||
                                attachment.Headers.ContentType.MediaType.Contains("text/html")))
                            {
                                MessageBody.Add(attachment.DecodeAsText());
                                toRemoveFromAttachments.Add(attachment);
                            }
                        }

                        foreach (Attachment removeFromAttachment in toRemoveFromAttachments)
                        {
                            Attachments.Remove(removeFromAttachment);
                        }
                    }
                }
                else
                {
                    // This is not a multipart message.
                    // This means that the whole message body is the actual message
                    // Parse this according to encoding and such
                    GetMessageBody(RawMessageBody);
                }
            }
        }
        #endregion

        #region Body parser functions
        /// <summary>
		/// Parses the MessageBody as a Multipart message.
		/// This method will add these parts as Attachments
		/// </summary>
		private void ParseMultipartMessageBody()
		{
            string multipartBoundary = Headers.ContentType.Boundary;

            if (string.IsNullOrEmpty(multipartBoundary))
                throw new ArgumentException("The body is a multipart message, but there is no multipart boundary");

            int indexOfAttachmentStart = 0;
            bool moreParts = true;

            // Keep working until we have parsed every message part in this message
            while(moreParts)
			{
                // Find the start of the message parts multipartBoundary
			    indexOfAttachmentStart = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart);

                if(indexOfAttachmentStart == -1)
                    throw new ArgumentException("The start of the attachment could not be found");

                // Find the start of this message part - which does not include the multipartBoundary or the trailing CRLF
                indexOfAttachmentStart = indexOfAttachmentStart + multipartBoundary.Length + "\r\n".Length;

			    // Find the end of the attachment, were we do not want the last line
			    int indexOfAttachmentEnd = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart);

			    if(indexOfAttachmentEnd == -1)
                    throw new ArgumentException("The end of the attachment could not be found");

                // Check if this is the last part, which ends with the multipartBoundary followed by "--"
                if(RawMessageBody.Substring(indexOfAttachmentEnd).StartsWith(multipartBoundary + "--"))
                    moreParts = false;

                // Calculate the length. We do not want to include the last "\r\n" in the attachment
			    int attachmentLength = indexOfAttachmentEnd - indexOfAttachmentStart - "\r\n".Length;

                string messagePart = RawMessageBody.Substring(indexOfAttachmentStart, attachmentLength);
			    Attachment att = new Attachment(messagePart, Headers);

				// Check if this is the MS-TNEF attachment type
                // which has ContentType application/ms-tnef
                // and also if we should decode it
			    if(MIMETypes.IsMSTNEF(att.Headers.ContentType.MediaType) && AutoDecodeMSTNEF) 
				{
                    // It was a MS-TNEF attachment. Now we should parse it.
                    TNEFParser tnef = new TNEFParser(att.DecodedAsBytes());

                    if (tnef.Parse())
                    {
                        // ms-tnef attachment might contain multiple attachments inside it
                        foreach (TNEFAttachment tatt in tnef.Attachments())
                        {
                            Attachment attNew = new Attachment(tatt.FileContent, tatt.FileName, MIMETypes.GetMimeType(tatt.FileName));
                            Attachments.Add(attNew);
                        }
                    }
                    else
                        throw new ArgumentException("Could not parse TNEF attachment");
				}
				else if(att.isMultipartAttachment())
				{
                    // The attachment itself is a multipart message
                    // Parse it as such, and take the attachments from it
                    // and add it to our message
                    // This will in reality flatten the structure
				    Message m = att.DecodeAsMessage(true,true);
				    foreach (string body in m.MessageBody)
				    {
				        MessageBody.Add(body);
				    }

				    foreach (Attachment attachment in m.Attachments)
				        Attachments.Add(attachment);
				}
				else
				{
                    // This must be an attachment
					Attachments.Add(att);
				}
			}
        }

        /// <summary>
        /// Parses message body of a MIME message
        /// </summary>
        /// <param name="strBuffer">Raw message body</param>
        private void GetMessageBody(string strBuffer)
        {
            MessageBody.Clear();

            try
            {
                if (Utility.IsOrNullTextEx(strBuffer))
                    return;

                if (Utility.IsOrNullTextEx(Headers.ContentType.MediaType) && Headers.ContentTransferEncoding == ContentTransferEncoding.EightBit)
                {
                    MessageBody.Add(strBuffer);
                }
                else if (Headers.ContentType.MediaType != null && Headers.ContentType.MediaType.ToLower().Contains("digest"))
                {
                    MessageBody.Add(strBuffer);
                }
                else
                {
                    string body;
                    if (Headers.ContentType.MediaType != null && !Headers.ContentType.MediaType.ToLower().Contains("multipart"))
                    {
                        // This is not a multipart message.
                        // It only contains some text
                        body = strBuffer;

                        // Now we only need to decode the text according to encoding
                        body = Utility.DoDecode(body, Headers.ContentTransferEncoding, Headers.ContentType.CharSet);

                        MessageBody.Add(body);
                    }
                    else
                    {
                        // This is a multipart message with multiple message bodies or attachments
                        int begin = 0;

                        // Foreach part
                        while (begin != -1)
                        {
                            string multipartBoundary = Headers.ContentType.Boundary;

                            // The start of a part of the message body is indicated by a "--" and the MutlipartBoundary
                            // Find this start, which should not be included in the message
                            begin = strBuffer.IndexOf("--" + multipartBoundary, begin);
                            if (begin != -1)
                            {
                                // Find the encoding of this part
                                ContentTransferEncoding encoding = HeaderFieldParser.ParseContentTransferEncoding(MIMETypes.GetContentTransferEncoding(strBuffer, begin));

                                // The message itself is located after the MultipartBoundary. It may contain headers, which is ended
                                // by a empty line, which corrosponds to "\r\n\r\n". We don't want to include the "\r\n", so skip them.
                                begin = strBuffer.IndexOf("\r\n\r\n", begin) + "\r\n\r\n".Length;

                                // Find end of text
                                // This is again ended by the "--" and the MultipartBoundary, where we don't want the last line delimter in the message
                                int end = strBuffer.IndexOf("--" + multipartBoundary, begin) - "\r\n".Length;

                                // Calculate the message length
                                int messageLength = end - begin;

                                // Now get the body out of the full message
                                body = strBuffer.Substring(begin, messageLength);

                                // Decode the body
                                body = Utility.DoDecode(body, encoding, Headers.ContentType.CharSet);

                                MessageBody.Add(body);
                            }
                            else
                            {
                                // If we did not find any parts in the multipart message
                                // We just add everything as a message
                                if (MessageBody.Count == 0)
                                {
                                    MessageBody.Add(strBuffer);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LogError("GetMessageBody():" + e.Message);
                MessageBody.Add(Base64.Decode(strBuffer));
            }
        }
        #endregion
    }
}