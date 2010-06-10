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

/*
*Name:			OpenPOP.MIMEParser.Message
*Function:		Message Parser
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/6/28 01:32 GMT+8 by Unruled Boy
*Description:
*Changes:		
*				2004/6/28 01:32 GMT+8 by Unruled Boy
*					1.Fixed a bug in not docoding multi-line sender
*				2004/6/26 16:03 GMT+8 by Unruled Boy
*					1.Renamed set_attachments to SetAttachments(), modified it and related functions to handle forwarded email that treats original email as attachment
*				2004/6/16 18:34 GMT+8 by Unruled Boy
*					1.fixed a loop in message body decoding by .
*				2004/5/17 14:20 GMT+8 by Unruled Boy
*					1.Again, fixed something but do not remember :(
*				2004/5/11 17:00 GMT+8 by Unruled Boy
*					1.Fixed a bug in parsing ContentCharset
*					2.Fixed a bug in ParseStreamLines
*				2004/5/10 10:00 GMT+8 by Unruled Boy
*					1.Well, fixed something but do not remember :(
*				2004/5/8 17:00 GMT+8 by Unruled Boy
*					1.Fixed a bug in parsing boundary
*				2004/5/1 14:13 GMT+8 by Unruled Boy
*					1.Adding three more constructors
*					2.Adding descriptions to every public functions/property/void
*				2004/4/29 19:05 GMT+8 by Unruled Boy
*					1.Fixed the bug parsing headers/boundary
*				2004/4/28 19:06 GMT+8 by Unruled Boy
*					1.Adding DateTimeInfo property
*					2.Maybe we correct the HTML content type bug
*				2004/4/23 21:13 GMT+8 by Unruled Boy
*					1.New Contructor
*					2.Tidy up the codes to follow Hungarian Notation
*				2004/3/29 10:38 GMT+8 by Unruled Boy
*					1.removing bugs in decoding message
*				2004/3/29 17:22 GMT+8 by Unruled Boy
*					1.adding support for reply message using ms-tnef 
*					2.adding support for all custom headers
*					3.rewriting the header parser(adding 3 ParseStreamLines)
*					4.adding detail description for every function
*					5.cleaning up the codes
*				2004/3/30 09:15 GMT+8 by Unruled Boy
*					1.Adding ImportanceType
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// Message Parser.
	/// </summary>
	public class Message
	{
		#region Member Variables

	    private string _replyTo=null;
		private string _replyToEmail=null;
		private string _from=null;
		private string _fromEmail=null;
	    private string _subject=null;
	    private bool _html=false;
	    private string _received=null;
	    private string _basePath=null;

	    #endregion


		#region Properties

	    /// <summary>
	    /// custom headers
	    /// </summary>
	    public Hashtable CustomHeaders { get; set; }

	    /// <summary>
	    /// whether auto decoding MS-TNEF attachment files
	    /// </summary>
	    public bool AutoDecodeMSTNEF { get; set; }

	    /// <summary>
		/// path to extract MS-TNEF attachment files
		/// </summary>
		public string BasePath
		{
			get{return _basePath;}
			set
			{
				try
				{
					if(value.EndsWith("\\"))
						_basePath=value;
					else
						_basePath=value+"\\";
				}
				catch (Exception)
				{
                    // What is the idea here?, why is this catch needed?
				}
			}
		}

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
		public string Received
		{
			get{return _received;}
		}

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
	    /// Attachment Boundry
	    /// </summary>
	    public string AttachmentBoundry { get; private set; }

	    /// <summary>
	    /// Alternate Attachment Boundry
	    /// </summary>
	    public string AttachmentBoundry2 { get; private set; }

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
		public bool HTML
		{
			get{return _html;}
		}

	    /// <summary>
	    /// Date
	    /// </summary>
	    public string Date { get; private set; }

	    /// <summary>
	    /// DateTime Info
	    /// </summary>
	    public string DateTimeInfo { get; private set; }

	    /// <summary>
		/// From name
		/// </summary>
		public string From
		{
			get{return _from;}
		}

		/// <summary>
		/// From Email
		/// </summary>
		public string FromEmail
		{
			get{return _fromEmail;}
		}

		/// <summary>
		/// Reply to name
		/// </summary>
		public string ReplyTo
		{
			get{return _replyTo;}
		}

		/// <summary>
		/// Reply to email
		/// </summary>
		public string ReplyToEmail
		{
			get{return _replyToEmail;}
		}

	    /// <summary>
	    /// whether has attachment
	    /// </summary>
	    public bool HasAttachment { get; private set; }

	    /// <summary>
	    /// raw message body
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
	    /// raw header
	    /// </summary>
	    public string RawHeader { get; private set; }

	    /// <summary>
	    /// raw message
	    /// </summary>
	    public string RawMessage { get; private set; }

	    /// <summary>
	    /// return path
	    /// </summary>
	    public string ReturnPath { get; private set; }

	    /// <summary>
		/// subject
		/// </summary>
		public string Subject
		{
			get{return _subject;}
		}
		#endregion


		/// <summary>
		/// release all objects
		/// </summary>
        /// <remarks>
        /// foens:
        /// I do not belive destructors are needed for this purpose
        /// My suggestion is therefore to remove this.
        /// Please comment if reading this
        /// </remarks>
		~Message()
		{
			Attachments.Clear();
			Attachments=null;
			Keywords.Clear();
			Keywords=null;
			MessageBody.Clear();
			MessageBody=null;
			CustomHeaders.Clear();
			CustomHeaders=null;
		}

        /// <summary>
        /// Sets up a default new message
        /// </summary>
        private Message()
        {
            ReturnPath = null;
            RawMessage = null;
            RawHeader = null;
            MimeVersion = null;
            MessageID = null;
            RawMessageBody = null;
            HasAttachment = false;
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
            AttachmentBoundry2 = null;
            AttachmentBoundry = null;
            MessageBody = new List<string>();
            ContentTransferEncoding = null;
            ContentCharset = null;
            Importance = null;
            DispositionNotificationTo = null;
            Keywords = new List<string>();
            AutoDecodeMSTNEF = false;
            CustomHeaders = new Hashtable();
        }

		/// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnFinish">reference for the finishing state</param>
		/// <param name="strBasePath">path to extract MS-TNEF attachment files</param>
		/// <param name="blnAutoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		/// <param name="strEMLFile">file of email content to load from</param>
		public Message(ref bool blnFinish, string strBasePath, bool blnAutoDecodeMSTNEF, bool blnOnlyHeader, string strEMLFile)
            : this()
		{
		    string strMessage=null;
			if(Utility.ReadPlainTextFromFile(strEMLFile,ref strMessage))
			{
			    _basePath = strBasePath;
			    AutoDecodeMSTNEF = blnAutoDecodeMSTNEF;
				NewMessage(ref blnFinish,strMessage,blnOnlyHeader);
			}
			else
				blnFinish=true;
		}

		/// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnFinish">reference for the finishing state</param>
		/// <param name="strBasePath">path to extract MS-TNEF attachment files</param>
		/// <param name="blnAutoDecodeMSTNEF">whether auto decoding MS-TNEF attachments</param>
		/// <param name="strMessage">raw message content</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		public Message(ref bool blnFinish, string strBasePath, bool blnAutoDecodeMSTNEF, string strMessage, bool blnOnlyHeader)
            : this()
		{
		    _basePath = strBasePath;
		    AutoDecodeMSTNEF = blnAutoDecodeMSTNEF;
		    NewMessage(ref blnFinish,strMessage,blnOnlyHeader);
		}

	    /// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnFinish">reference for the finishing state</param>
		/// <param name="strMessage">raw message content</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		public Message(ref bool blnFinish, string strMessage, bool blnOnlyHeader)
            : this()
	    {
	        _basePath = "";
            NewMessage(ref blnFinish, strMessage, blnOnlyHeader);
	    }

	    /// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnFinish">reference for the finishing state</param>
		/// <param name="strMessage">raw message content</param>
		public Message(ref bool blnFinish, string strMessage)
            : this()
	    {
	        _basePath = "";
	        NewMessage(ref blnFinish,strMessage,false);
	    }

	    /// <summary>
		/// get valid attachment
		/// </summary>
		/// <param name="intAttachmentNumber">attachment index in the attachments collection</param>
		/// <returns>attachment</returns>
		public Attachment GetAttachment(int intAttachmentNumber)
		{
			if(intAttachmentNumber<0 || intAttachmentNumber>AttachmentCount || intAttachmentNumber>Attachments.Count)
			{
				Utility.LogError("GetAttachment():attachment not exist");
				throw new ArgumentOutOfRangeException("intAttachmentNumber");	
			}
			return (Attachment)Attachments[intAttachmentNumber];
		}

		/// <summary>
		/// New Message
		/// </summary>
		/// <param name="blnFinish">reference for the finishing state</param>
		/// <param name="strBasePath">path to extract MS-TNEF attachment files</param>
		/// <param name="strMessage">raw message content</param>
		/// <param name="blnOnlyHeader">whether only decode the header without body</param>
		/// <returns>construction result whether successfully new a message</returns>
		private void NewMessage(ref bool blnFinish, string strMessage, bool blnOnlyHeader)
		{
			StringReader srdReader=new StringReader(strMessage);
			StringBuilder sbdBuilder=new StringBuilder();

			RawMessage=strMessage;

			string strLine=srdReader.ReadLine();
			while(Utility.IsNotNullTextEx(strLine))
			{	
				sbdBuilder.Append(strLine + "\r\n");
				ParseHeader(sbdBuilder,srdReader,ref strLine);
				if(Utility.IsOrNullTextEx(strLine))
					break;
				
				strLine=srdReader.ReadLine();
			}

			RawHeader=sbdBuilder.ToString();
			
			SetAttachmentBoundry2(RawHeader);

			if(ContentLength==0)
				ContentLength=strMessage.Length;//_rawMessageBody.Length;

			if(blnOnlyHeader==false)
			{
				RawMessageBody=srdReader.ReadToEnd().Trim();

				//the auto reply mail by outlook uses ms-tnef format
				if((HasAttachment && AttachmentBoundry!=null)||MIMETypes.IsMSTNEF(ContentType))
				{
					SetAttachments();

					if (Attachments.Count>0)
					{
						Attachment at=GetAttachment(0);
						if(at!=null&&at.NotAttachment)
							GetMessageBody(at.DecodeAsText());
						
						//in case body parts as text[0] html[1]
						if(Attachments.Count>1&&!IsReport())
						{
							at=GetAttachment(1);
							if(at!=null&&at.NotAttachment)
								GetMessageBody(at.DecodeAsText());						
						}
					}
				}
				else
					GetMessageBody(RawMessageBody);
			}

			blnFinish=true;
		}

		/// <summary>
		/// parse message body
		/// </summary>
		/// <param name="strBuffer">raw message body</param>
		/// <returns>message body</returns>
		public static string GetTextBody(string strBuffer)
		{
			if(strBuffer.EndsWith("\r\n."))
				return strBuffer.Substring(0,strBuffer.Length-"\r\n.".Length);
			else
				return strBuffer;
		}

		/// <summary>
		/// parse message body
		/// </summary>
		/// <param name="strBuffer">raw message body</param>
		public void GetMessageBody(string strBuffer)
		{
		    MessageBody.Clear();

			try
			{
				if(Utility.IsOrNullTextEx(strBuffer))
					return;

				if(Utility.IsOrNullTextEx(ContentType) && ContentTransferEncoding==null)
				{
					MessageBody.Add(GetTextBody(strBuffer));
				}
				else if(ContentType!=null && ContentType.IndexOf("digest") >= 0)
				{
					// this is a digest method
					//ParseDigestMessage(strBuffer);
					MessageBody.Add(GetTextBody(strBuffer));
				}
				else
				{
				    string body;
				    if(AttachmentBoundry2==null)
				    {					
				        body=GetTextBody(strBuffer);

				        if(Utility.IsQuotedPrintable(ContentTransferEncoding))
				        {
				            body=DecodeQP.ConvertHexContent(body);
				        }
				        else if(Utility.IsBase64(ContentTransferEncoding))
				        {
				            body=Utility.deCodeB64s(Utility.RemoveNonB64(body));
				        }
				        else if(Utility.IsNotNullText(ContentCharset))
				        {
				            body=Encoding.GetEncoding(ContentCharset).GetString(Encoding.Default.GetBytes(body));
				        }
				        MessageBody.Add(Utility.RemoveNonB64(body));
				    }
				    else
				    {
				        int begin = 0;

				        while(begin!=-1)
				        {
				            // find "\r\n\r\n" denoting end of header
				            begin = strBuffer.IndexOf("--" + AttachmentBoundry2,begin);
				            if(begin!=-1)
				            {
				                string encoding=MIMETypes.GetContentTransferEncoding(strBuffer,begin);

				                begin = strBuffer.IndexOf("\r\n\r\n",begin+1);//strBuffer.LastIndexOfAny(ALPHABET.ToCharArray());
							
				                // find end of text
				                int end = strBuffer.IndexOf("--" + AttachmentBoundry2,begin+1);

				                if(begin!=-1)
				                {
				                    if(end!=-1)
				                    {
				                        begin += 4;
				                        if(begin>=end)
				                            continue;

				                        if (ContentEncoding!=null && ContentEncoding.IndexOf("8bit")!=-1)
				                            body=Utility.Change(strBuffer.Substring(begin, end - begin-2 ),ContentCharset);
				                        else
				                            body=strBuffer.Substring(begin, end - begin-2);
				                    }
				                    else
				                    {
				                        body=strBuffer.Substring(begin);
				                    }

				                    if(Utility.IsQuotedPrintable(encoding))
				                    {
				                        string ret=body;
				                        ret=DecodeQP.ConvertHexContent(ret);
				                        MessageBody.Add(ret);
				                    }
				                    else if(Utility.IsBase64(encoding))
				                    {
				                        string ret=Utility.RemoveNonB64(body);
				                        ret=Utility.deCodeB64s(ret);
				                        if(ret!="\0")
				                            MessageBody.Add(ret);
				                        else
				                            MessageBody.Add(body);
				                    }
				                    else
				                        MessageBody.Add(body);

				                    if(end==-1)break;
				                }
				                else
				                {
				                    break;
				                }
				            }
				            else
				            {
				                if(MessageBody.Count==0)
				                {
				                    MessageBody.Add(strBuffer);
				                }
				                break;
				            }
				        }
				    }
				}
			}
			catch(Exception e)
			{
				Utility.LogError("GetMessageBody():"+e.Message);
				MessageBody.Add(Utility.deCodeB64s(strBuffer));
			}

			if(MessageBody.Count>1)
				_html=true;
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
				return (attItem.ContentType.ToLower()=="message/rfc822".ToLower() || attItem.ContentFileName.ToLower().EndsWith(".eml".ToLower()));
			}
			catch(Exception e)
			{
				Utility.LogError("IsMIMEMailFile():"+e.Message);
				return false;
			}
		}

		public static bool IsMIMEMailFile2(Attachment attItem)
		{
			try
			{
				//return (attItem.ContentType.ToLower()=="multipart/related".ToLower() && attItem.ContentFileName=="");
				return (attItem.ContentType.ToLower().StartsWith("multipart/".ToLower()) && attItem.ContentFileName=="");
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
					Attachment att=GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName))
					{
						if(Utility.IsNotNullText(att.ContentID))
							//support for embedded pictures
							strBody=strBody.Replace("cid:"+att.ContentID,hsbFiles[att.ContentFileName].ToString());

						strBody=strBody.Replace(att.ContentFileName,hsbFiles[att.ContentFileName].ToString());
					}
				}
			}
			catch(Exception e)
			{
				Utility.LogError("TranslateHTMLPictureFiles():"+e.Message);
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
					strPath+="\\";
				}			
				for(int i=0;i<AttachmentCount;i++)
				{
					Attachment att=GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName))
					{
						if(Utility.IsNotNullText(att.ContentID))
							//support for embedded pictures
							strBody=strBody.Replace("cid:"+att.ContentID,strPath+att.ContentFileName);
						strBody=strBody.Replace(att.ContentFileName,strPath+att.ContentFileName);
					}
				}
			}			
			catch(Exception e)
			{
				Utility.LogError("TranslateHTMLPictureFiles():"+e.Message);
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
			for(int i=0;i<Attachments.Count;i++)
			{
				if(attItem.ContentFileName==attItem.DefaultFileName)
				{
					items++;
					attItem.ContentFileName=attItem.DefaultFileName2.Replace("*",items.ToString());
				}
			}
			string name=attItem.ContentFileName;
			
			//return (name==null||name==""?(IsReport()==true?(this.IsMIMEMailFile(attItem)==true?attItem.DefaultMIMEFileName:attItem.DefaultReportFileName):(attItem.ContentID!=null?attItem.ContentID:attItem.DefaultFileName)):name);
			if(string.IsNullOrEmpty(name))
				if(IsReport())
				{
					if(IsMIMEMailFile(attItem))
						return attItem.DefaultMIMEFileName;

					return attItem.DefaultReportFileName;
				}
				else
				{
					if(IsMIMEMailFile(attItem))
						return attItem.DefaultMIMEFileName;
					
                    if(attItem.ContentID!=null)
						return attItem.ContentID;
					
					return attItem.DefaultFileName;
				}
			
			return name;
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
						strPath+="\\";
					}			
					for(int i=0;i<Attachments.Count;i++)
					{
						Attachment att=GetAttachment(i);
						blnRet=SaveAttachment(att,strPath+GetAttachmentFileName(att));
						if(!blnRet)
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
				if(attItem.InBytes)
				{
					da=attItem.RawBytes;
				}
				else if(attItem.ContentFileName.Length>0)
				{
					da=attItem.DecodedAttachment;
				}
				else if(attItem.ContentType.ToLower()=="message/rfc822".ToLower())
				{
					da=Encoding.Default.GetBytes(attItem.RawAttachment);
				}
				else
				{
					GetMessageBody(attItem.DecodeAsText());
					da=Encoding.Default.GetBytes((string)MessageBody[MessageBody.Count-1]);
				}
				return Utility.SaveByteContentToFile(strFileName,da);
			}
			catch
			{
				/*Utility.LogError("SaveAttachment():"+e.Message);
				return false;*/
				da=Encoding.Default.GetBytes(attItem.RawAttachment);
				return Utility.SaveByteContentToFile(strFileName,da);
			}
		}

		/// <summary>
		/// set attachments
		/// </summary>
		private void SetAttachments()
		{
			int indexOfAttachmentStart=0;
		    bool processed=false;
			string strLine;
		    Message m;
			Attachment att;

			SetAttachmentBoundry2(RawMessageBody);

			while(!processed)
			{
			    int indexOfAttachmentEnd;
			    if(Utility.IsNotNullText(AttachmentBoundry))
				{
					indexOfAttachmentStart=RawMessageBody.IndexOf(AttachmentBoundry,indexOfAttachmentStart)+AttachmentBoundry.Length;
					if(RawMessageBody==""||indexOfAttachmentStart<0)return;
					
					indexOfAttachmentEnd=RawMessageBody.IndexOf(AttachmentBoundry,indexOfAttachmentStart+1);				
				}
				else
				{
					indexOfAttachmentEnd=-1;
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

				strLine=RawMessageBody.Substring(indexOfAttachmentStart,(indexOfAttachmentEnd-indexOfAttachmentStart-2));            
				bool isMSTNEF = MIMETypes.IsMSTNEF(ContentType);
				att=new Attachment(strLine.Trim(),ContentType,!isMSTNEF);

				//ms-tnef format might contain multiple attachments
			    if(MIMETypes.IsMSTNEF(att.ContentType) && AutoDecodeMSTNEF && !isMSTNEF) 
				{
					Utility.LogError("SetAttachments():found ms-tnef file");
					TNEFParser tnef=new TNEFParser();
					TNEFAttachment tatt;

				    tnef.Verbose=false;
					tnef.BasePath=BasePath;
					//tnef.LogFilePath=this.BasePath + "OpenPOP.TNEF.log";
					if (tnef.OpenTNEFStream(att.DecodedAsBytes()))
					{
						if(tnef.Parse())
						{
							for (IDictionaryEnumerator i = tnef.Attachments().GetEnumerator(); i.MoveNext();)
							{
								tatt=(TNEFAttachment)i.Value;
								Attachment attNew=new Attachment(tatt.FileContent,tatt.FileLength ,tatt.FileName,MIMETypes.GetMimeType(tatt.FileName));
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
					m=att.DecodeAsMessage(true,true);
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
		/// Set alternative attachment boundry
		/// </summary>
		/// <param name="strBuffer">raw message</param>
		private void SetAttachmentBoundry2(string strBuffer)
		{
		    int indexOfAttachmentBoundry2Begin = strBuffer.ToLower().IndexOf("Multipart/Alternative".ToLower());
		    if(indexOfAttachmentBoundry2Begin!=-1)
			{
				indexOfAttachmentBoundry2Begin=strBuffer.IndexOf("boundary=");
				if(indexOfAttachmentBoundry2Begin!=-1)
				{
					int indexOfAttachmentBoundry2End=strBuffer.IndexOf("\r\n",indexOfAttachmentBoundry2Begin+9);
					if(indexOfAttachmentBoundry2End==-1)
						indexOfAttachmentBoundry2End=strBuffer.Length;
					AttachmentBoundry2=Utility.RemoveQuote(strBuffer.Substring(indexOfAttachmentBoundry2Begin+9,indexOfAttachmentBoundry2End-indexOfAttachmentBoundry2Begin-9));
				}					
			}
			else
			{
				AttachmentBoundry2=AttachmentBoundry;
			}
		}

	    /// <summary>
		/// Save message content to eml file
		/// </summary>
		/// <param name="strFile"></param>
		/// <param name="blnReplaceExists"></param>
		/// <returns></returns>
		public bool SaveToMIMEEmailFile(string strFile,bool blnReplaceExists)
		{
			return Utility.SavePlainTextToFile(strFile,RawMessage,blnReplaceExists);
		}

		/// <summary>
		/// parse multi-line header
		/// </summary>
		/// <param name="sbdBuilder">string builder to hold header content</param>
		/// <param name="srdReader">string reader to get each line of the header</param>
		/// <param name="strValue">first line content</param>
		/// <param name="strLine">reference header line</param>
		/// <param name="alCollection">collection to hold every content line</param>
		private void ParseStreamLines(StringBuilder sbdBuilder
										,StringReader srdReader
										,string strValue
										,ref string strLine
										,IList alCollection)
		{
		    int intLines=0;
			alCollection.Add(strValue);

			sbdBuilder.Append(strLine);

			strLine=srdReader.ReadLine();

			while(strLine.Trim()!="" && (strLine.StartsWith("\t") || strLine.StartsWith(" ")))
			{
				string strFormmated = strLine.Substring(1);
				alCollection.Add(Utility.DecodeLine(strFormmated));
				sbdBuilder.Append(strLine);
				strLine=srdReader.ReadLine();
				intLines++;
			}

			if(strLine!="")
			{
				sbdBuilder.Append(strLine);
			}
			else
				if(intLines==0)
				{
					strLine=srdReader.ReadLine();
					sbdBuilder.Append(strLine);
				}

			ParseHeader(sbdBuilder,srdReader,ref strLine);
		}

		/// <summary>
		/// parse multi-line header
		/// </summary>
		/// <param name="sbdBuilder">string builder to hold header content</param>
		/// <param name="srdReader">string reader to get each line of the header</param>
		/// <param name="strName">collection key</param>
		/// <param name="strValue">first line content</param>
		/// <param name="strLine">reference header line</param>
		/// <param name="hstCollection">collection to hold every content line</param>
		private void ParseStreamLines(StringBuilder sbdBuilder
										,StringReader srdReader
										,string strName
										,string strValue
										,ref string strLine
										,Hashtable hstCollection)
		{
		    string strReturn=strValue;
			int intLines=0;

			//sbdBuilder.Append(strLine);

			strLine=srdReader.ReadLine();
			while(strLine.Trim()!="" && (strLine.StartsWith("\t") || strLine.StartsWith(" ")))
			{
				string strFormmated = strLine.Substring(1);
				strReturn+=Utility.DecodeLine(strFormmated);
				sbdBuilder.Append(strLine + "\r\n");
				strLine=srdReader.ReadLine();
				intLines++;
			}
			if(!hstCollection.ContainsKey(strName))
				hstCollection.Add(strName,strReturn);

			if(strLine!="")
			{
				sbdBuilder.Append(strLine + "\r\n");
			}
			else
				if(intLines==0)
				{
//					strLine=srdReader.ReadLine();
//					sbdBuilder.Append(strLine + "\r\n");
				}

			ParseHeader(sbdBuilder,srdReader,ref strLine);
		}

		/// <summary>
		/// parse multi-line header
		/// </summary>
		/// <param name="sbdBuilder">string builder to hold header content</param>
		/// <param name="srdReader">string reader to get each line of the header</param>
		/// <param name="strValue">first line content</param>
		/// <param name="strLine">reference header line</param>
		/// <param name="strReturn">return value</param>
		/// <param name="blnLineDecode">decode each line</param>
		private void ParseStreamLines(StringBuilder sbdBuilder
										,StringReader srdReader
										,string strValue
										,ref string strLine
										,ref string strReturn
										,bool blnLineDecode)
		{
		    int intLines=0;
			strReturn=strValue;

			sbdBuilder.Append(strLine + "\r\n");

			if(blnLineDecode)
				strReturn=Utility.DecodeLine(strReturn);

			strLine=srdReader.ReadLine();
			while(strLine.Trim()!="" && (strLine.StartsWith("\t") || strLine.StartsWith(" ")))
			{
				string strFormmated = strLine.Substring(1);
				strReturn+=(blnLineDecode?Utility.DecodeLine(strFormmated):"\r\n"+strFormmated);
				sbdBuilder.Append(strLine + "\r\n");
				strLine=srdReader.ReadLine();
				intLines++;
			}

			if(strLine!="")
			{
				sbdBuilder.Append(strLine + "\r\n");
			}
			else
				if(intLines==0)
				{
					strLine=srdReader.ReadLine();
					sbdBuilder.Append(strLine + "\r\n");
				}

			if(!blnLineDecode)
			{
				strReturn=Utility.RemoveWhiteBlanks(Utility.DecodeText(strReturn));
			}
	
			ParseHeader(sbdBuilder,srdReader,ref strLine);
		}

		/// <summary>
		/// Parse the headers populating respective member fields
		/// </summary>
		/// <param name="sbdBuilder">string builder to hold the header content</param>
		/// <param name="srdReader">string reader to get each line of the header</param>
		/// <param name="strLine">reference header line</param>
		private void ParseHeader(StringBuilder sbdBuilder,StringReader srdReader,ref string strLine)
		{
			string []array=Utility.GetHeadersValue(strLine);//Regex.Split(strLine,":");

			switch(array[0].ToUpper())
			{
				case "TO":
					TO=array[1].Split(',');
					for(int i=0;i<TO.Length;i++)
					{
						TO[i]=Utility.DecodeLine(TO[i].Trim());
					}
					break;

				case "CC":
					CC=array[1].Split(',');					
					for(int i=0;i<CC.Length;i++)
					{
						CC[i]=Utility.DecodeLine(CC[i].Trim());
					}
					break;

				case "BCC":
					BCC=array[1].Split(',');					
					for(int i=0;i<BCC.Length;i++)
					{
						BCC[i]=Utility.DecodeLine(BCC[i].Trim());
					}
					break;

				case "FROM":
					ParseStreamLines(sbdBuilder,srdReader,array[1].Trim(),ref strLine,ref _from,false);
					Utility.ParseEmailAddress(_from,ref _from,ref _fromEmail);
					break;

				case "REPLY-TO":
					ParseStreamLines(sbdBuilder,srdReader,array[1].Trim(),ref strLine,ref _replyTo,false);
					Utility.ParseEmailAddress(_replyTo,ref _replyTo,ref _replyToEmail);
					break;

				case "KEYWORDS": //ms outlook keywords
					ParseStreamLines(sbdBuilder,srdReader,array[1].Trim(),ref strLine,Keywords);
					break;

				case "RECEIVED":
					ParseStreamLines(sbdBuilder,srdReader,array[1].Trim(),ref strLine,ref _received,true);
					break;

				case "IMPORTANCE":
					Importance=array[1].Trim();
					break;

				case "DISPOSITION-NOTIFICATION-TO":
					DispositionNotificationTo=array[1].Trim();
					break;

				case "MIME-VERSION":
					MimeVersion=array[1].Trim();
					break;

				case "SUBJECT":
				case "THREAD-TOPIC":
					string strRet=null;
					for(int i=1;i<array.Length;i++)
					{
						strRet+=array[i];
					}
					ParseStreamLines(sbdBuilder,srdReader,strRet,ref strLine,ref _subject,false);
					break;

				case "RETURN-PATH":
					ReturnPath=array[1].Trim().Trim('>').Trim('<');
					break;

				case "MESSAGE-ID":
					MessageID=array[1].Trim().Trim('>').Trim('<');
					break;

				case "DATE":
					for(int i=1;i<array.Length;i++)
					{
						DateTimeInfo+=array[i];
					}
					DateTimeInfo=DateTimeInfo.Trim();
					Date=Utility.ParseEmailDate(DateTimeInfo);
					break;

				case "CONTENT-LENGTH":
					ContentLength=Convert.ToInt32(array[1]);
					break;

				case "CONTENT-TRANSFER-ENCODING":
					ContentTransferEncoding=array[1].Trim();
					break;

				case "CONTENT-TYPE":
					//if already content type has been assigned
					if(ContentType!=null)
						return;

					strLine=array[1];

					ContentType=strLine.Split(';')[0];
					ContentType=ContentType.Trim();

					int intCharset=strLine.IndexOf("charset=");
					if(intCharset!=-1)
					{
						int intBound2=strLine.ToLower().IndexOf(";",intCharset+8);
						if(intBound2==-1)
							intBound2=strLine.Length;
						intBound2-=(intCharset+8);
						ContentCharset=strLine.Substring(intCharset+8,intBound2);
						ContentCharset=Utility.RemoveQuote(ContentCharset);
					}
					else 
					{
						intCharset=strLine.ToLower().IndexOf("report-type=".ToLower());
						if(intCharset!=-1)
						{
							int intPos=strLine.IndexOf(";",intCharset+13);
							ReportType=strLine.Substring(intCharset+12,intPos-intCharset-13);
						}
						else if(strLine.ToLower().IndexOf("boundary=".ToLower())==-1)
						{
							strLine=srdReader.ReadLine();
							if (strLine=="")
								return;
							intCharset=strLine.ToLower().IndexOf("charset=".ToLower());
							if(intCharset!=-1)
								ContentCharset=strLine.Substring(intCharset+9,strLine.Length-intCharset-10);
							else if(strLine.IndexOf(":")!=-1)
							{
								sbdBuilder.Append(strLine + "\r\n");
								ParseHeader(sbdBuilder,srdReader,ref strLine);
								return;						
							}
							else
							{
								sbdBuilder.Append(strLine + "\r\n");
							}
						}
					}
					if(ContentType=="text/plain")
						return;

					if(ContentType.ToLower()=="text/html"||ContentType.ToLower().IndexOf("multipart/")!=-1)
						_html=true;

					if(strLine.Trim().Length==ContentType.Length+1 || strLine.ToLower().IndexOf("boundary=".ToLower())==-1)
					{
						strLine=srdReader.ReadLine();
						if(string.IsNullOrEmpty(strLine)||strLine.IndexOf(":")!=-1)
						{
							sbdBuilder.Append(strLine + "\r\n");
							ParseHeader(sbdBuilder,srdReader,ref strLine);
							return;
						}
						
						sbdBuilder.Append(strLine + "\r\n");

						if(strLine.ToLower().IndexOf("boundary=".ToLower())==-1)
						{
							AttachmentBoundry=srdReader.ReadLine();
							sbdBuilder.Append(AttachmentBoundry+"\r\n");
						}
						AttachmentBoundry=strLine;
					}
					else
					{
						/*if(strLine.IndexOf(";")!=-1)
							_attachmentboundry=strLine.Split(';')[1];
						else*/
						AttachmentBoundry=strLine;
					}

					int intBound=AttachmentBoundry.ToLower().IndexOf("boundary=");
					if(intBound!=-1)
					{
						int intBound2=AttachmentBoundry.ToLower().IndexOf(";",intBound+10);
						if(intBound2==-1)
							intBound2=AttachmentBoundry.Length;
						intBound2-=(intBound+9);
						AttachmentBoundry=AttachmentBoundry.Substring(intBound+9,intBound2);
					}
					AttachmentBoundry=Utility.RemoveQuote(AttachmentBoundry);
					HasAttachment=true;

					break;

				default:
					if(array.Length>1) //here we parse all custom headers
					{
						string headerName=array[0].Trim();
						if(headerName.ToUpper().StartsWith("X")) //every custom header starts with "X"
						{
							ParseStreamLines(sbdBuilder,srdReader,headerName,array[1].Trim(),ref strLine,CustomHeaders);
						}
					}
					break;
			}
		}


	}
}

