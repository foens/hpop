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
using System.IO;
using System.Collections;
using System.Text;

namespace OpenPOP.MIMEParser
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

        /// <summary>
        /// All headers which were not recognized and explicitly dealt with.
        /// This should mostly be custom headers, like X-[name].
        /// </summary>
        public NameValueCollection CustomHeaders { get; private set; }

	    /// <summary>
	    /// message keywords
	    /// </summary>
	    public List<string> Keywords { get; private set; }

	    /// <summary>
	    /// disposition notification
	    /// </summary>
	    public string DispositionNotificationTo { get; private set; }

	    /// <summary>
	    /// received server
	    /// </summary>
	    public string Received { get; private set; }

	    /// <summary>
	    /// importance level
	    /// </summary>
	    public string Importance { get; private set; }

	    /// <summary>
		/// importance level type
		/// </summary>
		public MessageImportanceType ImportanceType
		{
			get
			{
				switch(Importance.ToUpper())
				{
					case "5":
					case "HIGH":
						return MessageImportanceType.HIGH;
					case "3":
					case "NORMAL":
						return MessageImportanceType.NORMAL;
					case "1":
					case "LOW":
						return MessageImportanceType.LOW;
					default:
						return MessageImportanceType.NORMAL;
				}
			}
		}

	    /// <summary>
	    /// Content Charset
	    /// </summary>
	    public string ContentCharset { get; private set; }

	    /// <summary>
	    /// Content Transfer Encoding
	    /// </summary>
	    public string ContentTransferEncoding { get; private set; }

	    /// <summary>
	    /// Message Bodies
	    /// </summary>
	    public List<string> MessageBody { get; private set; }

	    /// <summary>
	    /// The boundary between each message in the body
	    /// </summary>
	    public string MultipartBoundary { get; private set; }

	    /// <summary>
	    /// Attachment Count
	    /// </summary>
	    public int AttachmentCount { get; private set; }

	    /// <summary>
	    /// Attachments
	    /// </summary>
	    public List<Attachment> Attachments { get; private set; }

	    /// <summary>
	    /// CC
	    /// </summary>
	    public string[] CC { get; private set; }

	    /// <summary>
	    /// BCC
	    /// </summary>
	    public string[] BCC { get; private set; }

	    /// <summary>
	    /// TO
	    /// </summary>
	    public string[] TO { get; private set; }

	    /// <summary>
	    /// Content Encoding
	    /// </summary>
	    public string ContentEncoding { get; private set; }

	    /// <summary>
	    /// Content Length
	    /// </summary>
	    public long ContentLength { get; private set; }

	    /// <summary>
	    /// Content Type
	    /// </summary>
	    public string ContentType { get; private set; }

	    /// <summary>
	    /// Report Type
	    /// </summary>
	    public string ReportType { get; private set; }

	    /// <summary>
	    /// HTML
	    /// </summary>
	    public bool HTML { get; private set; }

	    /// <summary>
	    /// This is the Date header parsed, where
	    /// timezone and day-of-week has been stripped.
	    /// </summary>
	    public string Date { get; private set; }

	    /// <summary>
	    /// DateTime Info
	    /// This is the raw value of the Date header.
	    /// </summary>
	    public string DateTimeInfo { get; private set; }

	    /// <summary>
	    /// The name of the person that sent the email
	    /// 
	    /// If the from header was:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// this field would be
        /// Eksperten mailrobot
	    /// </summary>
	    public string From { get; private set; }

        /// <summary>
        /// The email of the person that sent the email
        /// 
        /// If the from header was:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// this field would be
        /// noreply@mail.eksperten.dk
        /// </summary>
	    public string FromEmail { get; private set; }

        /// <summary>
        /// The name of the person that is in the reply-to header field
        /// 
        /// If the reply-to header was:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// this field would be
        /// Eksperten mailrobot
        /// </summary>
	    public string ReplyTo { get; private set; }

        /// <summary>
        /// The emailaddress of the person that is in the reply-to header field
        /// 
        /// If the reply-to header was:
        /// Eksperten mailrobot <noreply@mail.eksperten.dk>
        /// this field would be
        /// noreply@mail.eksperten.dk
        /// </summary>
	    public string ReplyToEmail { get; private set; }

	    /// <summary>
	    /// Whether this message is a multipart message
	    /// </summary>
	    public bool isMultipart { get; private set; }

	    /// <summary>
	    /// The raw message body part of the RawMessage that this message was constructed with
	    /// </summary>
	    public string RawMessageBody { get; private set; }

	    /// <summary>
	    /// Message ID
	    /// </summary>
	    public string MessageID { get; private set; }

	    /// <summary>
	    /// MIME version
	    /// </summary>
	    public string MimeVersion { get; private set; }

	    /// <summary>
	    /// The header part from the RawMessage that this message was constructed with
	    /// </summary>
	    public string RawHeader { get; private set; }

	    /// <summary>
	    /// The raw content from which this message has been constructed
	    /// </summary>
	    public string RawMessage { get; private set; }

	    /// <summary>
	    /// return path
	    /// </summary>
	    public string ReturnPath { get; private set; }

	    /// <summary>
	    /// The subject line of the message in decoded, one line state.
	    /// </summary>
	    public string Subject { get; private set; }
	    #endregion

        #region Constructors
        /// <summary>
        /// Sets up a default new message
        /// </summary>
        private Message()
        {
            ReplyTo = null;
            FromEmail = null;
            Subject = null;
            Received = null;
            ReturnPath = null;
            RawMessage = null;
            RawHeader = null;
            MimeVersion = null;
            MessageID = null;
            RawMessageBody = null;
            isMultipart = false;
            DateTimeInfo = null;
            Date = null;
            ReportType = null;
            ContentType = null;
            ContentLength = 0;
            ContentEncoding = null;
            TO = new string[0];
            BCC = new string[0];
            CC = new string[0];
            Attachments = new List<Attachment>();
            AttachmentCount = 0;
            MultipartBoundary = null;
            MessageBody = new List<string>();
            ContentTransferEncoding = null;
            ContentCharset = null;
            Importance = null;
            DispositionNotificationTo = null;
            Keywords = new List<string>();
            AutoDecodeMSTNEF = false;
            CustomHeaders = null;
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
			if(Utility.IsNotNullText(ContentType))
				return (ContentType.ToLower().IndexOf("report".ToLower())!=-1);
			
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
                        if (Utility.IsNotNullText(att.ContentID))
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
                        if (Utility.IsNotNullText(att.ContentID))
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
                    da = attItem.DecodedAttachment;
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
			if(Utility.IsNotNullText(strPath))
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
            NameValueCollection headersTest;
            ParseHeaders(input, out rawHeadersTemp, out headersTest);
            RawHeader = rawHeadersTemp;

            // Create a holder for custom headers
            CustomHeaders = new NameValueCollection();

            // Now specificly parse each header. Some headers require special parsing.
            foreach (string headerName in headersTest.Keys)
            {
                string[] values = headersTest.GetValues(headerName);
                if (values != null)
                    foreach (string headerValue in values)
                    {
                        // Parse the header. If it was not recognized, it must have been a custom header
                        if (!ParseHeader(headerName, headerValue))
                        {
                            CustomHeaders.Add(headerName, headerValue);
                        }
                    }
            }

            if (ContentLength == 0)
                ContentLength = input.Length;

            if (onlyParseHeaders == false)
            {
                // The message body must be the full raw message, with headers removed.
                // Also remove any CRLF in top or bottom.
                RawMessageBody = RawMessage.Replace(RawHeader, "").Trim();

                //the auto reply mail by outlook uses ms-tnef format
                if (isMultipart || MIMETypes.IsMSTNEF(ContentType))
                {
                    SetAttachments();

                    if (Attachments.Count > 0)
                    {
                        Attachment at = GetAttachment(0);
                        if (at != null && at.NotAttachment)
                            GetMessageBody(at.DecodeAsText());

                        //in case body parts as text[0] html[1]
                        if (Attachments.Count > 1 && !IsReport())
                        {
                            at = GetAttachment(1);
                            if (at != null && at.NotAttachment)
                                GetMessageBody(at.DecodeAsText());
                        }
                    }
                }
                else
                    GetMessageBody(RawMessageBody);
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

            while(!processed)
			{
			    int indexOfAttachmentEnd;
			    if(!string.IsNullOrEmpty(MultipartBoundary))
				{
				    indexOfAttachmentStart = RawMessageBody.IndexOf(MultipartBoundary, indexOfAttachmentStart) + MultipartBoundary.Length;
                    if (RawMessageBody.Equals("") || indexOfAttachmentStart < 0) return;

				    indexOfAttachmentEnd = RawMessageBody.IndexOf(MultipartBoundary, indexOfAttachmentStart + 1);
				}
				else
				{
				    indexOfAttachmentEnd = -1;
				}

				//if(indexOfAttachmentEnd<0)return;
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
			    bool isMSTNEF = MIMETypes.IsMSTNEF(ContentType);
			    Attachment att = new Attachment(strLine.Trim(), ContentType, !isMSTNEF);

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

                if (Utility.IsOrNullTextEx(ContentType) && ContentTransferEncoding == null)
                {
                    MessageBody.Add(strBuffer);
                }
                else if (ContentType != null && ContentType.Contains("digest"))
                {
                    MessageBody.Add(strBuffer);
                }
                else
                {
                    string body;
                    if (!isMultipart)
                    {
                        // This is not a multipart message.
                        // It only contains some text
                        body = strBuffer;

                        // Now we only need to decode the text according to encoding
                        if (Utility.IsQuotedPrintable(ContentTransferEncoding))
                        {
                            if (!string.IsNullOrEmpty(ContentCharset))
                                body = DecodeQP.ConvertHexContent(body, Encoding.GetEncoding(ContentCharset), 0);
                            else
                                body = DecodeQP.ConvertHexContent(body);
                        }
                        else if (Utility.IsBase64(ContentTransferEncoding))
                            body = Utility.deCodeB64s(Utility.RemoveNonB64(body));
                        else if (!string.IsNullOrEmpty(ContentCharset))
                            body = Encoding.GetEncoding(ContentCharset).GetString(Encoding.Default.GetBytes(body));

                        MessageBody.Add(Utility.RemoveNonB64(body));
                    }
                    else
                    {
                        // This is a multipart message with multiple message bodies or attachments
                        int begin = 0;

                        // Foreach part
                        while (begin != -1)
                        {
                            // The start of a part of the message body is indicated by a "--" and the MutlipartBoundary
                            // Find this start, which should not be included in the message
                            begin = strBuffer.IndexOf("--" + MultipartBoundary, begin);
                            if (begin != -1)
                            {
                                // Find the encoding of this part
                                string encoding = MIMETypes.GetContentTransferEncoding(strBuffer, begin);

                                // The message itself is located after the MultipartBoundary. It may contain headers, which is ended
                                // by a empty line, which corrosponds to "\r\n\r\n". We don't want to include the "\r\n", so skip them.
                                begin = strBuffer.IndexOf("\r\n\r\n", begin) + "\r\n\r\n".Length;

                                // Find end of text
                                // This is again ended by the "--" and the MultipartBoundary, where we don't want the last line delimter in the message
                                int end = strBuffer.IndexOf("--" + MultipartBoundary, begin) - "\r\n".Length;

                                // Calculate the message length
                                int messageLength = end - begin;

                                if (ContentEncoding != null && ContentEncoding.IndexOf("8bit") != -1)
                                    body = Utility.ChangeEncoding(strBuffer.Substring(begin, messageLength), ContentCharset);
                                else
                                    body = strBuffer.Substring(begin, messageLength);
                                
                                // We have now found the body. Now we need to decode the body
                                if (Utility.IsQuotedPrintable(encoding))
                                {
                                    string ret;
                                    if (Utility.IsNotNullText(ContentCharset))
                                        ret = DecodeQP.ConvertHexContent(body, Encoding.GetEncoding(ContentCharset), 0);
                                    else
                                        ret = DecodeQP.ConvertHexContent(body);

                                    MessageBody.Add(ret);
                                }
                                else if (Utility.IsBase64(encoding))
                                {
                                    string ret = Utility.RemoveNonB64(body);
                                    ret = Utility.deCodeB64s(ret);
                                    if (ret != "\0")
                                        MessageBody.Add(ret);
                                    else
                                        MessageBody.Add(body);
                                }
                                else
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
                MessageBody.Add(Utility.deCodeB64s(strBuffer));
            }

            if (MessageBody.Count > 1)
                HTML = true;
        }
        #endregion

        #region Header parser functions
        /// <summary>
        /// Method that takes a full message and parses the headers from it.
        /// </summary>
        /// <param name="message">The message to parse headers from</param>
        /// <param name="rawHeaders">The portion of the message that was headers</param>
        /// <param name="headers">A collection of Name and Value pairs of headers</param>
        private static void ParseHeaders(string message, out string rawHeaders, out NameValueCollection headers)
        {
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

                // Read a single header. It might be a multi line header
                if (IsMoreLinesInHeaderValue(messageReader))
                {
                    // Keep reading until we would hit next header
                    while (IsMoreLinesInHeaderValue(messageReader))
                    {
                        // Unfolding is accomplished by simply removing any CRLF
                        // that is immediately followed by WSP
                        // This was done using ReadLine
                        string moreHeaderValue = messageReader.ReadLine();
                        headerValue += moreHeaderValue.Substring(1); // Remove the first whitespace

                        rawHeadersBuilder.Append(moreHeaderValue + "\r\n");
                    }

                    // Now we have the name and full value. Add it
                    headers.Add(headerName, headerValue);
                }
                else
                {
                    // This is a single line header. Simply insert it
                    headers.Add(headerName, headerValue);
                }
            }

            // Set the out parameter to our raw header. Remember to remove the last line ending.
            rawHeaders = rawHeadersBuilder.ToString().TrimEnd(new[] { '\r', '\n' });
        }

        /// <summary>
        /// Check if the next line is part of the current header value we are parsing by
        /// peeking on the next character of the TextReader.
        /// This should only be called while parsing headers
        /// </summary>
        /// <param name="reader">The reader from which the header is read from</param>
        /// <returns>true if multi-line header. False otherwise</returns>
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

	    /// <summary>
        /// Parses a single header and sets member variables according to it.
        /// </summary>
        /// <param name="name">The name of the header</param>
        /// <param name="value">The value of the header in unfolded state (only one line)</param>
        /// <returns>True if the message was understood and parsed. False if it was not (custom headers)</returns>
        private bool ParseHeader(string name, string value)
        {
            switch (name.ToUpper())
            {
                // See http://tools.ietf.org/html/rfc5322#section-3.6.3
                case "TO":
                    TO = value.Split(',');
                    for (int i = 0; i < TO.Length; i++)
                    {
                        TO[i] = Utility.DecodeLine(TO[i].Trim());
                    }
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.3
                case "CC":
                    CC = value.Split(',');
                    for (int i = 0; i < CC.Length; i++)
                    {
                        CC[i] = Utility.DecodeLine(CC[i].Trim());
                    }
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.3
                case "BCC":
                    BCC = value.Split(',');
                    for (int i = 0; i < BCC.Length; i++)
                    {
                        BCC[i] = Utility.DecodeLine(BCC[i].Trim());
                    }
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.2
                case "FROM":
                    string fromTemp;
                    string fromEmailTemp;
                    Utility.ParseEmailAddress(value, out fromTemp, out fromEmailTemp);
                    From = fromTemp;
                    FromEmail = fromEmailTemp;
                    break;

                // http://tools.ietf.org/html/rfc5322#section-3.6.2
                // The implementation here might be wrong
                case "REPLY-TO":
                    string replyToTemp;
                    string replyToEmailTemp;
                    Utility.ParseEmailAddress(value, out replyToTemp, out replyToEmailTemp);
                    ReplyTo = replyToTemp;
                    ReplyToEmail = replyToEmailTemp;
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.5
                // RFC 5322:
                // The "Keywords:" field contains a comma-separated list of one or more
                // words or quoted-strings.
                // The field are intended to have only human-readable content
                // with information about the message
                case "KEYWORDS": //ms outlook keywords
                    string[] KeywordsTemp = value.Split(',');
                    for (int i = 0; i < KeywordsTemp.Length; i++)
                    {
                        // Remove the quote if there is any
                        Keywords.Add(Utility.RemoveQuote(KeywordsTemp[i].Trim()));
                    }
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.7
                case "RECEIVED":
                    Received = value;
                    break;

                case "IMPORTANCE":
                    Importance = value.Trim();
                    break;

                case "DISPOSITION-NOTIFICATION-TO":
                    DispositionNotificationTo = value.Trim();
                    break;

                case "MIME-VERSION":
                    MimeVersion = value.Trim();
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.5
                case "SUBJECT":
                case "THREAD-TOPIC":
                    Subject = Utility.DecodeLine(value);
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.7
                case "RETURN-PATH":
                    ReturnPath = value.Trim().TrimEnd('>').TrimStart('<');
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.4
                // Example Message-ID
                // <33cdd74d6b89ab2250ecd75b40a41405@nfs.eksperten.dk>
                case "MESSAGE-ID":
                    MessageID = value.Trim().TrimEnd('>').TrimStart('<');
                    break;

                // See http://tools.ietf.org/html/rfc5322#section-3.6.1
                case "DATE":
                    DateTimeInfo = value.Trim();
                    Date = Utility.ParseEmailDate(DateTimeInfo);
                    break;

                case "CONTENT-LENGTH":
                    ContentLength = Convert.ToInt32(value);
                    break;

                case "CONTENT-TRANSFER-ENCODING":
                    ContentTransferEncoding = value.Trim();
                    break;

                // See http://www.ietf.org/rfc/rfc2045.txt section 5.1.
                // For more details on CONTENT-TYPE.
                // Example: Content-type: text/plain; charset="us-ascii"
                case "CONTENT-TYPE":
                    //if already content type has been assigned
                    if (ContentType != null)
                        break;

                    // ContentType is the first value, and it is required.
                    ContentType = value.Split(';')[0].Trim();

                    // We need the string in lower-case to search in it
                    // and we don't want to create that lower-case string each time
                    string lowerValue = value.ToLower();

                    const string charsetFind = "charset=";
                    int charsetStart = lowerValue.IndexOf(charsetFind);
                    if (charsetStart != -1)
                    {
                        charsetStart = charsetStart + charsetFind.Length;

                        // If there is a attribute more behind the charset, charset should
                        // be ended with an ";"
                        int intCharsetEnd = value.IndexOf(";", charsetStart);
                        string contentCharset;
                        if (intCharsetEnd != -1)
                        {
                            int intCharsetLength = intCharsetEnd - charsetStart;
                            contentCharset = value.Substring(charsetStart, intCharsetLength);
                        }
                        else
                        {
                            // If there is no ";" then there should be no more attributes,
                            // and then charset extends to the end of the line
                            contentCharset = value.Substring(charsetStart);
                        }

                        // The content might be qouted. Remove them if any
                        ContentCharset = Utility.RemoveQuote(contentCharset);
                    }
                    else
                    {
                        // report-type is explained in
                        // http://tools.ietf.org/html/rfc3462
                        // If the MIME subtype is report, then
                        // report-type and boundary attributes must be set

                        const string reportTypeFind = "report-type=";

                        int reportTypeStart = lowerValue.IndexOf(reportTypeFind);
                        if (reportTypeStart != -1)
                        {
                            reportTypeStart = reportTypeStart + reportTypeFind.Length;

                            // If there is a attribute more behind the report-type, report-type should
                            // be ended with an ";"
                            int reportTypeEnd = value.IndexOf(";", reportTypeStart);
                            string reportType;
                            if (reportTypeEnd != -1)
                            {
                                int reportTypeLength = reportTypeEnd - reportTypeStart;
                                reportType = value.Substring(reportTypeStart, reportTypeLength);
                            }
                            else
                            {
                                // If there is no ";" then there should be no more attributes,
                                // and then report-type extends to the end of the line
                                reportType = value.Substring(reportTypeStart);
                            }

                            // Remove qoutes if any
                            ReportType = Utility.RemoveQuote(reportType);
                        }

                        const string boundaryFind = "boundary=";
                        int boundaryStart = lowerValue.IndexOf(boundaryFind);
                        if(boundaryStart != -1)
                        {
                            boundaryStart = boundaryStart + boundaryFind.Length;

                            // If there is a attribute more behind the boundary, boundary should
                            // be ended with an ";"
                            int boundaryEnd = lowerValue.IndexOf(";", boundaryStart);
                            string boundary;
                            if (boundaryEnd != -1)
                            {
                                int boundaryLength = boundaryEnd - boundaryStart;
                                boundary = value.Substring(boundaryStart, boundaryLength);
                            }
                            else
                            {
                                // If there is no ";" then there should be no more attributes,
                                // and then boundary extends to the end of the line
                                boundary = value.Substring(boundaryStart);
                            }

                            // Remove qoutes if any
                            MultipartBoundary = Utility.RemoveQuote(boundary);
                            isMultipart = true;
                        }
                    }

                    // Checking if we need to set additional fields according to the Centent-Type
                    if (ContentType == "text/plain")
                        break;

                    if (ContentType.ToLower().Equals("text/html") || ContentType.ToLower().IndexOf("multipart/") != -1)
                        HTML = true;
                    break;

                default:
                    // This is a unknown header
                    
                    // Custom headers are allowed. That means headers
                    // that are not mentionen in the RFC.
                    // Such headers start with the letter "X"
                    // We do not have any special parsing of such
                    return false;
            }

            // This header was parsed correctly.
            return true;
        }
        #endregion
    }
}