using System;
using System.IO;
using System.Collections;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Summary description for MessageParser.
	/// </summary>
	public class Message
	{
		#region Member Variables
		//public const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";		
		private ArrayList _attachments=new ArrayList();
		private string _rawHeader=null;
		private string _rawMessage=null;
		private string _rawMessageBody=null;
		private int _attachmentCount=0;
		private string _replyTo=null;
		private string _replyToEmail=null;
		private string _from=null;
		private string _fromEmail=null;
		private string _date=null;
		private string _subject=null;
		private string[] _to=new string[0];
		private string[] _cc=new string[0];
		private string[] _bcc=new string[0];
		private ArrayList _keywords=new ArrayList();
		private string _contentType=null;
		private string _contentCharset=null;
		private string _reportType=null;
		private string _contentTransferEncoding=null;
		private bool _html=false;
		private long _contentLength=0;
		private string _contentEncoding=null;
		private string _returnPath=null;
		private string _mimeVersion=null;
		private string _received=null;
		private string _xMailer=null;
		private string _xOriginatingIP=null;
		private string _xPriority=null;
		private string _xMSMailPriority=null;
		private string _importance=null;
		private string _xOriginalArrivalTime=null;
		private string _messageID=null;
		private string _attachmentboundry=null;		
		private string _attachmentboundry2=null;		
		private bool _hasAttachment=false;
		private string _dispositionNotificationTo=null;
		private ArrayList _messageBody=new ArrayList();
		private string _basePath=null;
		private bool _autoDecodeMSTNEF=false;
		#endregion

		#region Properties
		public bool AutoDecodeMSTNEF
		{
			get{return _autoDecodeMSTNEF;}
			set{_autoDecodeMSTNEF=value;}
		}

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
				catch
				{
				}
			}
		}
		public ArrayList Keywords
		{
			get
			{
				return _keywords;
			}
		}

		public string DispositionNotificationTo
		{
			get
			{
				return _dispositionNotificationTo;
			}
		}

		public string Received
		{
			get
			{
				return _received;
			}
		}

		public string XMailer
		{
			get
			{
				return _xMailer;
			}
		}

		public string XOriginatingIP
		{
			get
			{
				return _xOriginatingIP;
			}
		}

		public string XPriority
		{
			get
			{
				return _xPriority;
			}
		}

		public string XMSMailPriority
		{
			get
			{
				return _xMSMailPriority;
			}
		}

		public string Importance
		{
			get
			{
				return _importance;
			}
		}

		public string XOriginalArrivalTime
		{
			get
			{
				return _xOriginalArrivalTime;
			}
		}

		public string ContentCharset
		{
			get
			{
				return _contentCharset;
			}
		}

		public string ContentTransferEncoding
		{
			get
			{
				return _contentTransferEncoding;
			}
		}

		public ArrayList MessageBody
		{
			get
			{
				return _messageBody;
			}
		}

		public string AttachmentBoundry
		{
			get
			{
				return _attachmentboundry;
			}
		}

		public string AttachmentBoundry2
		{
			get
			{
				return _attachmentboundry2;
			}
		}


		public int AttachmentCount
		{
			get
			{
				return _attachmentCount;
			}
		}


		public ArrayList Attachments
		{
			get
			{
				return _attachments;
			}
		}
		

		public string[] CC
		{
			get
			{
				return _cc;
			}
		}


		public string[] BCC
		{
			get
			{
				return _bcc;
			}
		}


		public string[] TO
		{
			get
			{
				return _to;
			}
		}


		public string ContentEncoding
		{
			get
			{
				return _contentEncoding;
			}
		}


		public long ContentLength
		{
			get
			{
				return _contentLength;
			}
		}


		public string ContentType
		{
			get
			{
				return _contentType;
			}
		}

		public string ReportType
		{
			get
			{
				return _reportType;
			}
		}

		public bool HTML
		{
			get
			{
				return _html;
			}
		}

		public string Date
		{
			get
			{
				return _date;
			}
		}


		public string From
		{
			get
			{
				return _from;
			}
		}


		public string FromEmail
		{
			get
			{
				return _fromEmail;
			}
		}


		public string ReplyTo
		{
			get
			{
				return _replyTo;
			}
		}


		public string ReplyToEmail
		{
			get
			{
				return _replyToEmail;
			}
		}


		public bool HasAttachment
		{
			get
			{
				return _hasAttachment;
			}
		}


		public string RawMessageBody
		{
			get
			{
				return _rawMessageBody;
			}
		}


		public string MessageID
		{
			get
			{
				return _messageID;
			}
		}


		public string MimeVersion
		{
			get
			{
				return _mimeVersion;
			}
		}


		public string RawHeader
		{
			get
			{
				return _rawHeader;
			}
		}


		public string RawMessage
		{
			get
			{
				return _rawMessage;
			}
		}


		public string ReturnPath
		{
			get
			{
				return _returnPath;
			}
		}


		public string Subject
		{
			get
			{
				return _subject;
			}
		}		
		

		#endregion

		public Attachment GetAttachment(int attachmentNumber)
		{
			if(attachmentNumber<0 || attachmentNumber>_attachmentCount || attachmentNumber>_attachments.Count)
			{
				Utility.LogError("GetAttachment():attachment not exist");
				throw new ArgumentOutOfRangeException("attachmentNumber");	
			}
			return (Attachment)_attachments[attachmentNumber];		
		}

		internal Message(POPClient pop, string wMessage, bool onlyHeader)
		{
			BasePath=pop.BasePath;
			AutoDecodeMSTNEF=pop.AutoDecodeMSTNEF;
			_rawMessage=wMessage;
			StringReader reader=new StringReader(wMessage);
			StringBuilder builder=new StringBuilder();
//			string temp=reader.ReadLine();
//			while( temp!=".")
//			{	
//				builder.Append(temp);
//				parseHeader(builder,reader,temp);
//				temp=reader.ReadLine();
//			}
			string temp=reader.ReadLine();
			while(temp!=null && temp.Trim()!="")
			{	
				builder.Append(temp);
				parseHeader(builder,reader,ref temp);
				if(temp==null || temp.Trim()=="")
					break;
				else
					temp=reader.ReadLine();
			}

			_rawHeader=builder.ToString();
			
			SetAttachmentBoundry2(_rawHeader);

			if(_contentLength==0)
				_contentLength=wMessage.Length;//_rawMessageBody.Length;

			if(onlyHeader==false)
			{
				_rawMessageBody=reader.ReadToEnd().Trim();

				if(_hasAttachment==true && _attachmentboundry!=null)
				{
					//set_attachmentCount();
					set_attachments();

					if (this.Attachments.Count>0)
					{
						Attachment at=this.GetAttachment(0);
						if(at.NotAttachment)
							this.GetMessageBody(at.DecodeAsText());
						else
						{}
						if(this.Attachments.Count>1)
						{
							at=this.GetAttachment(1);
							if(at!=null&&at.NotAttachment)
								this.GetMessageBody(at.DecodeAsText());						
							else
							{}
						}
					}
					else
					{}
				}
				else
				{
					//_messageBody=GetTextMessage(_rawMessage);
					GetMessageBody(_rawMessageBody);
				}
			}

			if(pop!=null)
				pop._receiveFinish=true;
		}

		public string GetTextBody(string Buffer)
		{
			if(Buffer.EndsWith("\r\n."))
				return Buffer.Substring(0,Buffer.Length-"\r\n.".Length);
			else
				return Buffer;
		}

		public void GetMessageBody(string Buffer)
		{
			int end, begin;
			string body;
			string encoding="";
			
			begin = end = 0;
			_messageBody.Clear();

			try
			{
				if(_contentType==null||_contentType=="")
				{
					_messageBody.Add(GetTextBody(Buffer));
				}
				else if(_contentType.IndexOf("digest") >= 0)
				{
					// this is a digest method
					//ParseDigestMessage(Buffer);
					_messageBody.Add(GetTextBody(Buffer));
				}
				else if(_attachmentboundry2==null)
				{					
//					body=Buffer;
//					if(body.EndsWith("\r\n."))
//						body=body.Substring(0,body.Length-"\r\n.".Length);
					body=GetTextBody(Buffer);

					if(Utility.IsQuotedPrintable(_contentTransferEncoding))
					{
						body=DecodeQP.ConvertHexContent(body);
					}
					else if(Utility.IsBase64(_contentTransferEncoding))
					{
						body=Utility.deCodeB64s(Utility.RemoveNonB64(body));
					}
					else if(_contentCharset!=""&&_contentCharset!=null)
					{
						body=Encoding.GetEncoding(_contentCharset).GetString(Encoding.Default.GetBytes(body));
					}
					_messageBody.Add(Utility.RemoveNonB64(body));
				}
				else
				{
					begin =0;
					//int intBodies=0;
					while(begin!=-1)
					{
						// find "\r\n\r\n" denoting end of header
						begin = Buffer.IndexOf("--" + _attachmentboundry2,begin);
						if(begin!=-1)
						{
							encoding=Utility.GetContentTransferEncoding(Buffer,begin);

							begin = Buffer.IndexOf("\r\n\r\n",begin+1);//Buffer.LastIndexOfAny(ALPHABET.ToCharArray());
							//if((end = Buffer.LastIndexOfAny(NON_WHITE.ToCharArray())) < 0)
							
							end = Buffer.IndexOf("--" + _attachmentboundry2,begin+1);
							// find end of text
							//end = Buffer.IndexOf("\r\n--" + _attachmentboundry2,begin+1);//Buffer.LastIndexOfAny(ALPHABET.ToCharArray());

							if(begin!=-1)
							{
								if(end!=-1)
								{
									begin += 4;
									if(begin>=end)
										continue;
									else if (this._contentEncoding!=null && this._contentEncoding.IndexOf("8bit")!=-1)
										body=Utility.Change(Buffer.Substring(begin, end - begin-2 ),_contentCharset);
									else
										body=Buffer.Substring(begin, end - begin-2);
					
									//intBodies++;
									// find "\r\n\r\n" denoting end of header
									//								begin = Buffer.IndexOf("\r\n\r\n",begin);
									//								// find end of text
									//								if(begin!=-1)
									//									end = Buffer.IndexOf("\r\n--" + _attachmentboundry2,begin+1);
									//								else
									//									break;
								}
								else
								{
									body=Buffer.Substring(begin);
								}

								if(Utility.IsQuotedPrintable(encoding))
								{
									string ret=body;
									ret=DecodeQP.ConvertHexContent(ret);
									_messageBody.Add(ret);
								}
								else if(Utility.IsBase64(encoding))
								{
									string ret=Utility.RemoveNonB64(body);
									ret=Utility.deCodeB64s(ret);
									if(ret!="\0")
										_messageBody.Add(ret);
									else
										_messageBody.Add(body);
								}
								else
									_messageBody.Add(body);

								if(end==-1)break;
							}
							else
							{
								//_messageBody[0]=Buffer;
								break;
							}
						}
						else
						{
							//_messageBody[0]=Buffer;
							if(_messageBody.Count==0)
							{
								//_messageBody.Add(Utility.deCodeB64s(Buffer));
								_messageBody.Add(Buffer);
							}
							break;
						}
					}
				}
			}
			catch(Exception e)
			{
				Utility.LogError("GetMessageBody():"+e.Message);
				_messageBody.Add(Utility.deCodeB64s(Buffer));
			}
			if(_messageBody.Count>1)
				_html=true;
		}

//		public string GetTextMessage(string buffer) 
//		{
//			// find "\r\n\r\n" denoting end of header
//			int start=buffer.IndexOf("\r\n\r\n")+4;
//			int end=buffer.LastIndexOf("\r\n.\r\n");
//			//change charset if contentTransferEncoding is 8bit
//			if(start!=-1&&end!=-1)
//			{
//				if (this._contentEncoding!=null && this._contentEncoding.IndexOf("8bit")!=-1)
//					return Utility.Change(buffer.Substring(start,end-start),_contentCharset);
//				else
//					return buffer.Substring(start,end-start);			
//			}
//			else
//				return buffer;
//		}

		public bool IsReport()
		{
			if(_contentType!=null)
				return (_contentType.ToLower().IndexOf("report".ToLower())!=-1);
			else
				return false;
		}

		public bool IsMIMEMailFile(Attachment attItem)
		{
			try
			{
				return (attItem.ContentFileName.ToLower().EndsWith(".eml".ToLower()) || attItem.ContentType.ToLower()=="message/rfc822".ToLower());
			}
			catch(Exception e)
			{
				Utility.LogError("IsMIMEMailFile():"+e.Message);
				return false;
			}
		}

		public string TranslateHTMLPictureFiles(string strBody, Hashtable hsbFiles)
		{
			try
			{
				for(int i=0;i<this.AttachmentCount;i++)
				{
					Attachment att=this.GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName)==true)
					{
						if(att.ContentID!=null && att.ContentID!="")
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

		public string TranslateHTMLPictureFiles(string strBody, string strPath)
		{
			try
			{
				if(!strPath.EndsWith("\\"))
				{
					strPath+="\\";
				}			
				for(int i=0;i<this.AttachmentCount;i++)
				{
					Attachment att=this.GetAttachment(i);
					if(Utility.IsPictureFile(att.ContentFileName)==true)
					{
						if(att.ContentID!=null && att.ContentID!="")
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

		public string GetAttachmentFileName(Attachment attItem)
		{
			
			//bool duplicated=false;
			int items=0;

			for(int i=0;i<_attachments.Count;i++)
			{
				if(attItem.ContentFileName==attItem.DefaultFileName)
				{
					items++;
					attItem.ContentFileName=attItem.DefaultFileName2.Replace("*",items.ToString());
				}
//				if(items>1)
//					duplicated=true;
//				if(duplicated==true)
//				{
//					attItem.ContentFileName=attItem.DefaultFileName2;
//					break;
//				}
			}
			string name=attItem.ContentFileName;
			
			return (name==null||name==""?(IsReport()==true?(this.IsMIMEMailFile(attItem)==true?attItem.DefaultMIMEFileName:attItem.DefaultReportFileName):attItem.DefaultFileName):name);
		}

		public bool SaveAttachments(string strPath)
		{
			if(strPath!=null&&strPath!="")
			{
				try
				{
					bool blnRet=true;

					if(!strPath.EndsWith("\\"))
					{
						strPath+="\\";
					}			
					for(int i=0;i<this.Attachments.Count;i++)
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
			else
				return false;
		}

		/// <summary>
		/// save attachment to file
		/// </summary>
		/// <param name="attItem">Attachment</param>
		/// <param name="strFileName">File to be saved to</param>
		/// <returns></returns>
		public bool SaveAttachment(Attachment attItem, string strFileName)
		{
			try
			{
//				FileStream fs=File.Create(strFileName);
//				byte[] da;
//				if(attItem.ContentFileName.Length>0)
//				{
//					da=attItem.DecodedAttachment;
//				}
//				else
//				{
//					this.GetMessageBody(attItem.DecodeAttachmentAsText());
//					da=Encoding.Default.GetBytes((string)this.MessageBody[this.MessageBody.Count-1]);
//				}
//				fs.Write(da,0,da.Length);
//				fs.Close();
//				return true;
				byte[] da;
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
					this.GetMessageBody(attItem.DecodeAsText());
					da=Encoding.Default.GetBytes((string)this.MessageBody[this.MessageBody.Count-1]);
				}
				return Utility.SaveByteContentToFile(da,strFileName);
			}
			catch(Exception e)
			{
				Utility.LogError("SaveAttachment():"+e.Message);
				return false;
			}
		}

//		/// <summary>
//		/// Parse the message for attachment boundry and calculate number of _attachments
//		/// based on that
//		/// </summary>
//		private void set_attachmentCount()
//		{
//			int indexOf_attachmentboundry=0;
//
//			while( (indexOf_attachmentboundry=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentboundry+1))>0)
//			{
//				_attachmentCount++;
//			}
//			_attachmentCount--;
//		}

		/// <summary>
		/// set attachments
		/// </summary>
		private void set_attachments()
		{
			int indexOf_attachmentstart=0;
			int indexOfAttachmentEnd=0;
			bool processed=false;

			Attachment att=null;

			SetAttachmentBoundry2(_rawMessageBody);
			
			while(true)
			{
				indexOf_attachmentstart=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentstart+1)+_attachmentboundry.Length;
				if(indexOf_attachmentstart<0)return;

				indexOfAttachmentEnd=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentstart+1);				
				//if(indexOfAttachmentEnd<0)return;
				if(indexOfAttachmentEnd!=-1)
				{
				}
				else if(indexOfAttachmentEnd==-1&&!processed&&_attachmentCount==0) 
				{
					processed=true;
					indexOfAttachmentEnd=_rawMessageBody.Length;
				}
				else
					return;

				string temp=_rawMessageBody.Substring(indexOf_attachmentstart,(indexOfAttachmentEnd-indexOf_attachmentstart-2));            
				att=new Attachment(temp.Trim());

				//ms-tnef format might contain multiple attachments
				if(att.ContentType.ToLower() == "application/ms-tnef".ToLower() && AutoDecodeMSTNEF) 
				{
					Utility.LogError("set_attachments():found ms-tnef file");
					TNEF.TNEFParser tnef=new OpenPOP.TNEF.TNEFParser();
					TNEF.TNEFAttachment tatt=new OpenPOP.TNEF.TNEFAttachment();
					Attachment attNew=null;
					
					tnef.Verbose=false;
					tnef.BasePath=this.BasePath;
					//tnef.LogFilePath=this.BasePath + "OpenPOP.TNEF.log";
					if (tnef.OpenTNEFStream(att.DecodedAsBytes()))
					{
						if(tnef.Parse())
						{
							for (IDictionaryEnumerator i = tnef.Attachments().GetEnumerator(); i.MoveNext();)
							{
								tatt=(TNEF.TNEFAttachment)i.Value;
								attNew=new Attachment(tatt.FileContent,tatt.FileLength ,tatt.FileName,Utility.getMimeType(tatt.FileName));
								_attachmentCount++;
								_attachments.Add(attNew);
							}
						}
						else
							Utility.LogError("set_attachments():ms-tnef file parse failed");
					}
					else
						Utility.LogError("set_attachments():ms-tnef file open failed");
				}
				else
				{
					_attachmentCount++;
					_attachments.Add(att);
				}
			}
		}


//		private void setMail(string rawData)
//		{			
//
//		}

		/// <summary>
		/// Set alternative attachment boundry
		/// </summary>
		/// <param name="buffer">raw message</param>
		private void SetAttachmentBoundry2(string buffer)
		{
			int indexOfAttachmentBoundry2Begin=0;
			int indexOfAttachmentBoundry2End=0;
			indexOfAttachmentBoundry2Begin=buffer.ToLower().IndexOf("Multipart/Alternative".ToLower());
			if(indexOfAttachmentBoundry2Begin!=-1)
			{
				indexOfAttachmentBoundry2Begin=buffer.IndexOf("boundary=\"");
				indexOfAttachmentBoundry2End=buffer.IndexOf("\"",indexOfAttachmentBoundry2Begin+10);
				if(indexOfAttachmentBoundry2Begin!=-1&&indexOfAttachmentBoundry2End!=-1)
					_attachmentboundry2=buffer.Substring(indexOfAttachmentBoundry2Begin+10,indexOfAttachmentBoundry2End-indexOfAttachmentBoundry2Begin-10).Trim();
			}
			else
			{
				_attachmentboundry2=_attachmentboundry;
			}
		}

		public bool SaveToMIMEEmailFile(string file)
		{
			return Utility.SaveByteContentToFile(Encoding.Default.GetBytes(_rawMessage),file);
		}

		/// <summary>
		/// Parse the headers populating respective member fields
		/// </summary>
		/// <param name="temp"></param>
		/// 
		private void parseHeader(StringBuilder builder,StringReader reader,ref string temp)
		{
			string []array=Utility.getHeadersValue(temp);		
			string line="";

			switch(array[0].ToUpper())
			{
				case "TO":
					_to=array[1].Split(',');
					for(int i=0;i<_to.Length;i++)
					{
						_to[i]=Utility.deCodeLine(_to[i].Trim());
					}
					break;

				case "CC":
					_cc=array[1].Split(',');					
					for(int i=0;i<_cc.Length;i++)
					{
						_cc[i]=Utility.deCodeLine(_cc[i].Trim());
					}
					break;

				case "BCC":
					_bcc=array[1].Split(',');					
					for(int i=0;i<_bcc.Length;i++)
					{
						_bcc[i]=Utility.deCodeLine(_bcc[i].Trim());
					}
					break;

				case "FROM":
					Utility.ParseEmailAddress(array[1],ref _from,ref _fromEmail);
//					int indexOfAB=array[1].Trim().LastIndexOf("<");
//					int indexOfEndAB=array[1].Trim().LastIndexOf(">");
//					_from=array[1];
//					_fromEmail=array[1];
//					if(indexOfAB>=0&&indexOfEndAB>=0)
//						{
//						if(indexOfAB>0)
//						{
//							_from=_from.Substring(0,indexOfAB-1);					
//							if(_from.IndexOf("\"")>=0)
//							{
//								_from=_from.Substring(1,_from.Length-2);
//							}
//						}
//						_fromEmail=_fromEmail.Substring(indexOfAB+1,indexOfEndAB-(indexOfAB+1));
//					}
//					_from=_from.Trim();
//					_from=Utility.deCodeLine(_from);
//					_fromEmail=_fromEmail.Trim();					
					break;

				case "REPLY-TO":
					Utility.ParseEmailAddress(array[1],ref _replyTo,ref _replyToEmail);
					break;

				case "KEYWORDS":
					_keywords.Add(array[1].Trim());
					line=reader.ReadLine();
					while(line.IndexOf(":")==-1 && line.Trim()!="")
					{
						_keywords.Add(Utility.deCodeLine(line));
						builder.Append(line);
						line=reader.ReadLine();
					}
					builder.Append(line);
					temp=line;
					parseHeader(builder,reader,ref line);
					break;

				case "RECEIVED":
					_received=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "X-ORIGINATING-IP":
					_xOriginatingIP=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "X-PRIORITY":
					_xPriority=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "X-MSMAIL-PRIORITY":
					_xMSMailPriority=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "IMPORTANCE":
					_importance=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "X-MAILER":
					_xMailer=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "DISPOSITION-NOTIFICATION-TO":
					_dispositionNotificationTo=array[1].Trim();
					break;

				case "MIME-VERSION":
					_mimeVersion=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "X-OriginalArrivalTime":
					_xOriginalArrivalTime=array[1].Trim();//temp.Split(':')[1].Trim();
					break;

				case "SUBJECT":
				case "THREAD-TOPIC":
					_subject=array[1].Trim();//temp.Split(':')[1].Trim();

					line=reader.ReadLine();
					while(line.IndexOf(":")==-1 && line.Trim()!="")
					{
						_subject+=line;
						builder.Append(line);
						line=reader.ReadLine();
					}
					_subject=Utility.deCodeLine(_subject);
					builder.Append(line);
					temp=line;
					parseHeader(builder,reader,ref line);
					break;
				
				case "RETURN-PATH":
					_returnPath=array[1].Trim().Trim('>').Trim('<');//temp.Split(':')[1].Trim().Trim('>').Trim('<');
					break;

				case "MESSAGE-ID":
					_messageID=array[1].Trim().Trim('>').Trim('<');//array[1].Trim().Trim('>').Trim('<');;
					break;

				case "DATE":
					for(int i=1;i<array.Length;i++)
					{
						_date+=array[i];
					}
					_date=_date.Trim();
					_date=Utility.ParseEmailDate(_date);
					break;

				case "CONTENT-LENGTH":
					_contentLength=Convert.ToInt32(array[1]);
					break;

				case "CONTENT-TRANSFER-ENCODING":
					_contentTransferEncoding=array[1].Trim();
					break;

				case "CONTENT-TYPE":
					//if already content type has been assigned
					if(_contentType!=null)
						return;

					temp=array[1];

					_contentType=temp.Split(';')[0];
					_contentType=_contentType.Trim();

					int intCharset=temp.IndexOf("charset=");
					if(intCharset!=-1)
					{
						//int intPos=temp.IndexOf("\"",intCharset+9);
						//_contentCharset=temp.Substring(intCharset+9,intPos-intCharset-9);
						_contentCharset=temp.Substring(intCharset+8);
						_contentCharset=Utility.RemoveQuote(_contentCharset);
					}
					else 
					{
						intCharset=temp.ToLower().IndexOf("report-type=".ToLower());
						if(intCharset!=-1)
						{
							int intPos=temp.IndexOf(";",intCharset+13);
							_reportType=temp.Substring(intCharset+12,intPos-intCharset-13);
						}
						else if(temp.ToLower().IndexOf("boundary=".ToLower())==-1)
						{
							temp=reader.ReadLine();
							intCharset=temp.ToLower().IndexOf("charset=".ToLower());
							if(intCharset!=-1)
								_contentCharset=temp.Substring(intCharset+9,temp.Length-intCharset-10);
							else if(temp.IndexOf(":")!=-1)
							{
								builder.Append(temp);
								temp=line;
								parseHeader(builder,reader,ref line);
								return;						
							}
							else
							{
								builder.Append(temp);
							}
						}
					}
					if(_contentType=="text/plain")
						return;
					else if(_contentType=="text/html")
						_html=true;

					if(temp.Trim().Length==_contentType.Length+1 || temp.ToLower().IndexOf("boundary=".ToLower())==-1)
					{
						temp=reader.ReadLine();
						if(temp==null||temp==""||temp.IndexOf(":")!=-1)
						{
							builder.Append(temp);
							temp=line;
							parseHeader(builder,reader,ref line);
							return;
						}
						else
						{
							builder.Append(temp);
						}

						if(temp.ToLower().IndexOf("boundary=".ToLower())==-1)
						{
							_attachmentboundry=reader.ReadLine();
							builder.Append(_attachmentboundry);
						}
					}
					else
					{
						if(temp.IndexOf(";")!=-1)
							_attachmentboundry=temp.Split(';')[1];
						else
							_attachmentboundry=temp;
					}

					_attachmentboundry=temp;

					int indexOfQuote=_attachmentboundry.IndexOf("\"");
					int lastIndexOfQuote=_attachmentboundry.LastIndexOf("\"");
					if(indexOfQuote>=0&&lastIndexOfQuote>=0)
					{
						_attachmentboundry=_attachmentboundry.Substring(indexOfQuote+1,lastIndexOfQuote-(indexOfQuote+1));

						_hasAttachment=true;
					}
					
					break;
			}
		}


	}
}
