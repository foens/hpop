using System;
using System.IO;
using System.Collections;

namespace OpenPOP
{
	/// <summary>
	/// Summary description for MessageParser.
	/// </summary>
	public class Message
	{
		#region Member Variables
		private ArrayList _attachments=new ArrayList();
		private string _rawHeader=null;
		private string _rawMessage=null;
		private string _rawMessageBody=null;
		private int _attachmentCount=0;
		private string _from=null;
		private string _fromEmail=null;
		private string _date=null;
		private string _subject=null;
		private string[] _to=new string[0];
		private string[] _cc=new string[0];
		private string _contentType=null;
		private long _contentLength=0;
		private string _contentEncoding=null;
		private string _returnPath=null;
		private string _mimeVersion=null;		
		private string _messageID=null;
		private string _attachmentboundry=null;		
		private bool _hasAttachment=false;
		private string _messageBody=null;
		#endregion

		#region Properties

		public string MessageBody
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


		public string[] TO
		{
			get
			{
				return _cc;
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
				throw new ArgumentOutOfRangeException("attachmentNumber");	

			return (Attachment)_attachments[attachmentNumber];		
		}


		internal Message(string wMessage)
		{
			_rawMessage=wMessage;
			StringReader reader=new StringReader(wMessage);
			string temp=null;
			while( (temp=reader.ReadLine()) !=null && temp.Trim()!="")
			{	
				_rawHeader+=temp;
				parseHeader(temp);				
			}
			
			_rawMessageBody=reader.ReadToEnd().Trim();			

			if(_hasAttachment==true && _attachmentboundry!=null)
			{
				set_attachmentCount();
				set_attachments();
			}
			else
			{
				_messageBody=_rawMessage;
			}
		}		

		/// <summary>
		/// Parse the message for attachment boundry and calculate number of _attachments
		/// based on that
		/// </summary>
		private void set_attachmentCount()
		{
			int indexOf_attachmentboundry=0;

			while( (indexOf_attachmentboundry=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentboundry+1))>0)
			{
				_attachmentCount++;
			}
			_attachmentCount--;
		}


		private void set_attachments()
		{
			int indexOf_attachmentstart=0;
			int indexOfAttachmentEnd=0;
			while(true)
			{
				indexOf_attachmentstart=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentstart+1)+_attachmentboundry.Length;
				if(indexOf_attachmentstart<0)return;

				indexOfAttachmentEnd=_rawMessageBody.IndexOf(_attachmentboundry,indexOf_attachmentstart+1);				
				if(indexOfAttachmentEnd<0)return;

				string temp=_rawMessageBody.Substring(indexOf_attachmentstart,(indexOfAttachmentEnd-indexOf_attachmentstart-2));            
				_attachments.Add(new Attachment(temp.Trim()));
			}
		}


		private void setMail(string rawData)
		{			

		}

		/// <summary>
		/// Parse the headers populating respective member fields
		/// </summary>
		/// <param name="temp"></param>
		/// 
		private void parseHeader(string temp)
		{
			string []array=Utility.getHeadersValue(temp);		

			switch(array[0].ToUpper())
			{
				case "TO":
					_to=array[1].Split(',');
					for(int i=0;i<_to.Length;i++)
					{
						_to[i]=_to[i].Trim();
					}
					break;

				case "CC":
					_cc=array[1].Split(',');					
					break;

				case "FROM":
					int indexOfAB=array[1].LastIndexOf("<");
					int indexOfEndAB=array[1].LastIndexOf(">");
					_from=array[1].Substring(0,indexOfAB-1);					
					_from=_from.Trim();

					_fromEmail=array[1].Substring(indexOfAB+1,indexOfEndAB-(indexOfAB+1));
					_fromEmail=_fromEmail.Trim();
					break;

				case "MIME-VERSION":
					_mimeVersion=temp.Split(':')[1].Trim();
					break;

				case "SUBJECT":
					_subject=temp.Split(':')[1].Trim();
					break;
				
				case "RETURN-PATH":
					_returnPath=temp.Split(':')[1].Trim().Trim('>').Trim('<');
					break;

				case "MESSAGE-ID":
					_messageID=array[1].Trim().Trim('>').Trim('<');;
					break;

				case "DATE":
					for(int i=1;i<array.Length;i++)
					{
						_date+=array[i];
					}
					_date=_date.Trim();
					break;

				case "CONTENT-LENGTH":
					_contentLength=Convert.ToInt32(array[1]);
					break;

				case "CONTENT-TYPE":

					//if already content type has been assigned
					if(_contentType!=null)
						return;

					_contentType=array[1].Split(';')[0];
					_contentType=_contentType.Trim();

					if(_contentType=="text/plain")
						return;

					_attachmentboundry=array[1].Split(';')[1];
					int indexOfQuote=_attachmentboundry.IndexOf("\"");
					int lastIndexOfQuote=_attachmentboundry.LastIndexOf("\"");
					_attachmentboundry=_attachmentboundry.Substring(indexOfQuote+1,lastIndexOfQuote-(indexOfQuote+1));

					_hasAttachment=true;
					break;				
			}
		}


	}
}
