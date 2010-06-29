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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using OpenPOP.MIME.Decode;
using OpenPOP.MIME.Header;

namespace OpenPOP.MIME
{
	/// <summary>
	/// Message Parser.
	/// 
	/// foens: This class has become a big big blob class with unrelated methods
	/// My idea is to make a Message class that just hold messages
	/// Then we should have a messageparser which can create a Message class from string,
	/// file and should also be able to save to file.
	/// </summary>
	public class Message
	{
		#region Properties
        /// <summary>
	    /// whether auto decoding MS-TNEF attachment files
	    /// </summary>
	    public bool AutoDecodeMSTNEF { get; set; }

        public MessageHeader Headers { get; private set; }

	    /// <summary>
	    /// Message Bodies
	    /// </summary>
	    public List<string> MessageBody { get; private set; }

	    /// <summary>
	    /// Attachment Count
	    /// </summary>
	    public int AttachmentCount { get; private set; }

	    /// <summary>
	    /// Attachments
	    /// </summary>
	    public List<Attachment> Attachments { get; private set; }

	    /// <summary>
	    /// HTML
	    /// </summary>
	    public bool HTML { get; private set; }

	    /// <summary>
	    /// The raw message body part of the RawMessage that this message was constructed with.
	    /// The Raw message is simply the message body of the message, but the message body has NOT
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
            AttachmentCount = 0;
            MessageBody = new List<string>();
            AutoDecodeMSTNEF = false;
        }

		/// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnAutoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		/// <param name="strEMLFile">file of email content to load from</param>
		public Message(bool blnAutoDecodeMSTNEF, bool blnOnlyHeader, string strEMLFile)
            : this()
		{
		    string strMessage=null;
			if(Utility.ReadPlainTextFromFile(strEMLFile,ref strMessage))
			{
			    AutoDecodeMSTNEF = blnAutoDecodeMSTNEF;
				InitializeMessage(strMessage,blnOnlyHeader);
			}
		}

		/// <summary>
		/// New Message
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

	    /// <summary>
		/// New Message
		/// </summary>
		/// <param name="strMessage">raw message content</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		public Message(string strMessage, bool blnOnlyHeader)
            : this()
	    {
            InitializeMessage(strMessage, blnOnlyHeader);
	    }

	    /// <summary>
		/// New Message
		/// </summary>
		/// <param name="strMessage">raw message content</param>
		public Message(string strMessage)
            : this()
	    {
	        InitializeMessage(strMessage,false);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// get valid attachment
        /// </summary>
        /// <param name="intAttachmentNumber">attachment index in the attachments collection</param>
        /// <returns>attachment</returns>
        public Attachment GetAttachment(int intAttachmentNumber)
        {
            if (intAttachmentNumber < 0 || intAttachmentNumber > AttachmentCount || intAttachmentNumber > Attachments.Count)
            {
                Utility.LogError("GetAttachment():attachment not exist");
                throw new ArgumentOutOfRangeException("intAttachmentNumber");
            }
            return Attachments[intAttachmentNumber];
        }

		/// <summary>
		/// verify if the message is a report
		/// </summary>
		/// <returns>if it is a report message, return true, else, false</returns>
		public bool IsReport()
		{
			if(!string.IsNullOrEmpty(Headers.ContentType.MediaType))
                return (Headers.ContentType.MediaType.ToLower().IndexOf("report".ToLower()) != -1);
			
			return false;
		}

		/// <summary>
		/// verify if the attachment is MIME Email file
		/// </summary>
		/// <param name="attItem">attachment</param>
		/// <returns>if MIME Email file, return true, else, false</returns>
		public static bool IsMIMEMailFile(Attachment attItem)
		{
			try
			{
			    return attItem.ContentType.ToLower() == "message/rfc822".ToLower() ||
			           attItem.ContentFileName.ToLower().EndsWith(".eml".ToLower());
			}
			catch(Exception e)
			{
			    Utility.LogError("IsMIMEMailFile():" + e.Message);
				return false;
			}
		}

		public static bool IsMIMEMailFile2(Attachment attItem)
		{
			try
			{
			    return attItem.ContentType.ToLower().StartsWith("multipart/".ToLower()) && attItem.ContentFileName == "";
			}
			catch(Exception e)
			{
				Utility.LogError("IsMIMEMailFile2():"+e.Message);
				return false;
			}
		}

		/// <summary>
		/// translate pictures url within the body
		/// </summary>
		/// <param name="strBody">message body</param>
		/// <param name="hsbFiles">pictures collection</param>
		/// <returns>translated message body</returns>
		public string TranslateHTMLPictureFiles(string strBody, Hashtable hsbFiles)
		{
			try
			{
				for(int i=0;i<AttachmentCount;i++)
				{
				    Attachment att = GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName))
					{
                        if (!string.IsNullOrEmpty(att.ContentID))
                            //support for embedded pictures
                            strBody = strBody.Replace("cid:" + att.ContentID, hsbFiles[att.ContentFileName].ToString());

					    strBody = strBody.Replace(att.ContentFileName, hsbFiles[att.ContentFileName].ToString());
					}
				}
			}
			catch(Exception e)
			{
			    Utility.LogError("TranslateHTMLPictureFiles():" + e.Message);
			}
			return strBody;
		}

		/// <summary>
		/// translate pictures url within the body
		/// </summary>
		/// <param name="strBody">message body</param>
		/// <param name="strPath">path of the pictures</param>
		/// <returns>translated message body</returns>
		public string TranslateHTMLPictureFiles(string strBody, string strPath)
		{
			try
			{
				if(!strPath.EndsWith("\\"))
				{
				    strPath += "\\";
				}			
				for(int i=0;i<AttachmentCount;i++)
				{
					Attachment att=GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName))
					{
                        if (!string.IsNullOrEmpty(att.ContentID))
                            //support for embedded pictures
                            strBody = strBody.Replace("cid:" + att.ContentID, strPath + att.ContentFileName);
					    strBody = strBody.Replace(att.ContentFileName, strPath + att.ContentFileName);
					}
				}
			}			
			catch(Exception e)
			{
			    Utility.LogError("TranslateHTMLPictureFiles():" + e.Message);
			}
			return strBody;
		}

		/// <summary>
		/// Get the proper attachment file name
		/// </summary>
		/// <param name="attItem">attachment</param>
		/// <returns>propery attachment file name</returns>
		public string GetAttachmentFileName(Attachment attItem)
		{
			int items=0;

			//return unique body file names
            for (int i = 0; i < Attachments.Count; i++)
            {
                if (attItem.ContentFileName == attItem.DefaultFileName)
                {
                    items++;
                    attItem.ContentFileName = attItem.DefaultFileName2.Replace("*", items.ToString());
                }
            }
		    string name = attItem.ContentFileName;
			
			//return (name==null||name==""?(IsReport()==true?(this.IsMIMEMailFile(attItem)==true?attItem.DefaultMIMEFileName:attItem.DefaultReportFileName):(attItem.ContentID!=null?attItem.ContentID:attItem.DefaultFileName)):name);
			if(string.IsNullOrEmpty(name))
				if(IsReport())
				{
                    if (IsMIMEMailFile(attItem))
                        return attItem.DefaultMIMEFileName;

					return attItem.DefaultReportFileName;
				}
				else
				{
                    if (IsMIMEMailFile(attItem))
                        return attItem.DefaultMIMEFileName;

                    if (attItem.ContentID != null)
                        return attItem.ContentID;

				    return attItem.DefaultFileName;
				}
			
			return name;
		}

        /// <summary>
        /// save attachment to file
        /// </summary>
        /// <param name="attItem">Attachment</param>
        /// <param name="strFileName">File to be saved to</param>
        /// <returns>true if save successfully, false if failed</returns>
        public bool SaveAttachment(Attachment attItem, string strFileName)
        {
            byte[] da;
            try
            {
                if (attItem.InBytes)
                {
                    da = attItem.RawBytes;
                }
                else if (attItem.ContentFileName.Length > 0)
                {
                    da = attItem.DecodedAsBytes();
                }
                else if (attItem.ContentType.ToLower() == "message/rfc822")
                {
                    da = Encoding.Default.GetBytes(attItem.RawAttachment);
                }
                else
                {
                    GetMessageBody(attItem.DecodeAsText());
                    da = Encoding.Default.GetBytes(MessageBody[MessageBody.Count - 1]);
                }
                return Utility.SaveByteContentToFile(strFileName, da);
            }
            catch
            {
                /*Utility.LogError("SaveAttachment():"+e.Message);
                return false;*/
                da = Encoding.Default.GetBytes(attItem.RawAttachment);
                return Utility.SaveByteContentToFile(strFileName, da);
            }
        }

		/// <summary>
		/// save attachments to a defined path
		/// </summary>
		/// <param name="strPath">path to have attachments to be saved to</param>
		/// <returns>true if save successfully, false if failed</returns>
		public bool SaveAttachments(string strPath)
		{
            if (!string.IsNullOrEmpty(strPath))
			{
				try
				{
					bool blnRet=true;

					if(!strPath.EndsWith("\\"))
					{
					    strPath += "\\";
					}
                    for (int i = 0; i < Attachments.Count; i++)
                    {
                        Attachment att = GetAttachment(i);
                        blnRet = SaveAttachment(att, strPath + GetAttachmentFileName(att));
                        if (!blnRet)
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
			
			return false;
        }

        /// <summary>
        /// Save message content to eml file
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="blnReplaceExists"></param>
        /// <returns></returns>
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
            Headers = MessageHeader.ParseHeaders(headersUnparsedCollection);

            if (onlyParseHeaders == false)
            {
                // The message body must be the full raw message, with headers removed.
                // Also remove any CRLF in top or bottom.
                RawMessageBody = RawMessage.Replace(RawHeader, "").Trim();

                // Check if the message is a multipart message (which means, has multiple message bodies)
                if (Headers.ContentType != null && (Headers.ContentType.MediaType.Contains("multipart") || MIMETypes.IsMSTNEF(Headers.ContentType.MediaType)))
                {
                    SetAttachments();

                    if (Attachments.Count > 0)
                    {
                        // Check if the first attachment is the message
                        Attachment at = GetAttachment(0);
                        if (at != null && at.NotAttachment)
                            GetMessageBody(at.DecodeAsText());

                        // In case body parts as text[0] html[1]
                        if (Attachments.Count > 1 && !IsReport())
                        {
                            at = GetAttachment(1);
                            if (at != null && at.NotAttachment)
                                GetMessageBody(at.DecodeAsText());
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
		/// set attachments
		/// </summary>
		private void SetAttachments()
		{
            int indexOfAttachmentStart = 0;
            bool processed = false;

            string multipartBoundary = Headers.ContentType.Boundary;

            while(!processed)
			{
			    int indexOfAttachmentEnd;
			    if(!string.IsNullOrEmpty(multipartBoundary))
				{
				    indexOfAttachmentStart = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart) + multipartBoundary.Length;
                    if (RawMessageBody.Equals("") || indexOfAttachmentStart < 0) return;

				    indexOfAttachmentEnd = RawMessageBody.IndexOf(multipartBoundary, indexOfAttachmentStart + 1);
				}
				else
				{
				    indexOfAttachmentEnd = -1;
				}

				if(indexOfAttachmentEnd!=-1)
				{
				}
				else if(indexOfAttachmentEnd==-1&&AttachmentCount==0) 
				{
					processed=true;
					indexOfAttachmentEnd=RawMessageBody.Length;
				}
				else
					return;

				if(indexOfAttachmentStart==indexOfAttachmentEnd-9)
				{
					indexOfAttachmentStart=0;
					processed=true;
				}

			    string strLine = RawMessageBody.Substring(indexOfAttachmentStart, indexOfAttachmentEnd - indexOfAttachmentStart - 2);
			    bool isMSTNEF = MIMETypes.IsMSTNEF(Headers.ContentType.MediaType);
                Attachment att = new Attachment(strLine.Trim(), Headers.ContentType.MediaType, !isMSTNEF);

				//ms-tnef format might contain multiple attachments
			    if(MIMETypes.IsMSTNEF(att.ContentType) && AutoDecodeMSTNEF && !isMSTNEF) 
				{
				    Utility.LogError("SetAttachments():found ms-tnef file");
				    TNEFParser tnef = new TNEFParser();
				    tnef.Verbose=false;

					if (tnef.OpenTNEFStream(att.DecodedAsBytes()))
					{
						if(tnef.Parse())
						{
						    foreach (TNEFAttachment tatt in tnef.Attachments())
						    {
                                Attachment attNew = new Attachment(tatt.FileContent, tatt.FileLength, tatt.FileName, MIMETypes.GetMimeType(tatt.FileName));
                                AttachmentCount++;
                                Attachments.Add(attNew);
						    }
						}
						else
							Utility.LogError("SetAttachments():ms-tnef file parse failed");
					}
					else
						Utility.LogError("SetAttachments():ms-tnef file open failed");
				}
				else if(IsMIMEMailFile2(att))
				{
				    Message m = att.DecodeAsMessage(true,true);
				    for(int i=0;i<m.AttachmentCount;i++)
					{
						att=m.GetAttachment(i);
						AttachmentCount++;
						Attachments.Add(att);
					}
				}
				else
				{
					AttachmentCount++;
					Attachments.Add(att);
				}

				indexOfAttachmentStart++;
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
                MessageBody.Add(Base64.decode(strBuffer));
            }

            if (MessageBody.Count > 1)
                HTML = true;
        }
        #endregion
    }
}