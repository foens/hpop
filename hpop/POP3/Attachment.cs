using System;
using System.IO;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Summary description for Attachment.
	/// </summary>
	public class Attachment
	{
		#region Member Variables
		private string _contentType=null;
		private string _contentCharset=null;
		private string _contentFormat=null;
		private string _contentTransferEncoding=null;
		private string _contentDescription=null;
		private string _contentDisposition=null;
		private string _contentFileName="";
		private string _defaultFileName="body.htm";
		private string _defaultFileName2="body*.htm";
		private string _defaultReportFileName="report.htm";
		private string _defaultMIMEFileName="body.eml";
		private string _contentID=null;
		private long _contentLength=0;
		private string _rawAttachment=null;
		private bool _inBytes=false;
		private byte[] _rawBytes=null;
		#endregion

		#region Properties
		public byte[] RawBytes
		{
			get
			{
				return _rawBytes;
			}
			set
			{
				_rawBytes=value;
			}
		}

		public bool InBytes
		{
			get
			{
				return _inBytes;
			}
			set
			{
				_inBytes=value;
			}
		}

		public long ContentLength
		{
			get
			{
				return _contentLength;
			}
		}

		public bool NotAttachment
		{
			get
			{
/*				if (_contentDisposition==null||_contentType==null)
					return true;
				else
					return (_contentDisposition.IndexOf("attachment")==-1 && _contentType.IndexOf("text/plain")!=-1); */
/*				if (_contentType==null)
					return true;
				else
					return (_contentFileName!="");*/
				if (_contentType==null||_contentFileName==""||_contentFileName==null)
					return true;
				else
					return false;

			}
		}

		public string ContentFormat
		{
			get
			{
				return _contentFormat;
			}
		}

		public string ContentCharset
		{
			get
			{
				return _contentCharset;
			}
		}

		public string DefaultFileName
		{
			get
			{
				return _defaultFileName;
			}
		}

		public string DefaultFileName2
		{
			get
			{
				return _defaultFileName2;
			}
		}

		public string DefaultReportFileName
		{
			get
			{
				return _defaultReportFileName;
			}
		}

		public string DefaultMIMEFileName
		{
			get
			{
				return _defaultMIMEFileName;
			}
		}

		public string ContentType
		{
			get
			{
				return _contentType;
			}
		}


		public string ContentTransferEncoding
		{
			get
			{
				return _contentTransferEncoding;
			}
		}


		public string ContentDescription
		{
			get
			{
				return _contentDescription;
			}
		}


		public string ContentFileName
		{
			get
			{
				return _contentFileName;
			}
			set
			{
				_contentFileName=value;
			}
		}


		public string ContentDisposition
		{
			get
			{
				return _contentDisposition;
			}
		}


		public string ContentID
		{
			get
			{
				return _contentID;
			}
		}


		public string RawAttachment
		{
			get
			{
				return _rawAttachment;
			}
		}


		/// <summary>
		/// decoded attachment in bytes
		/// </summary>
		public byte[] DecodedAttachment
		{
			get
			{
				return DecodedAttachmentAsBytes();
			}
		}


		#endregion

		public Attachment(byte[] bytAttachment, long lngFileLength, string strFileName, string strContentType)
		{
			_inBytes=true;
			_rawBytes=bytAttachment;
			_contentLength=lngFileLength;
			_contentFileName=strFileName;
			_contentType=strContentType;
		}

		public Attachment(string sAttachment)
		{	
			_inBytes=false;

			if(sAttachment==null)
				throw new ArgumentNullException("sAttachment");

			StringReader sr=new StringReader(sAttachment);

			string temp=null;
			while( (temp=sr.ReadLine())!=null & temp!="")
			{
				parseHeader(sr,temp);
			}

			this._rawAttachment=sr.ReadToEnd();
			_contentLength=this._rawAttachment.Length;
		}


		/// <summary>
		/// Parse header fields and set member variables
		/// </summary>
		/// <param name="sr">string reader</param>
		/// <param name="temp">header line</param>
		private void parseHeader(StringReader sr,string temp)
		{
			string []array=Utility.getHeadersValue(temp);
			string []values=array[1].Split(";".ToCharArray());
			string strRet=null;

			switch(array[0].ToUpper())
			{				
				case "CONTENT-TYPE":
					//_contentType=Utility.splitOnSemiColon(array[1])[0].Trim();
					//if(array[1].ToLower().IndexOf("charset".ToLower())!=-1)
					//	_contentCharset=Utility.splitOnSemiColon(array[1])[1].Trim();
					//break;
					if(values.Length>0)
						_contentType=values[0].Trim();
					if(values.Length>1)
					{
						_contentCharset=Utility.GetQuotedValue(values[1],"=","charset");
					}
					if(values.Length>2)
					{
						_contentFormat=Utility.GetQuotedValue(values[2],"=","format");
					}
					//if(_contentType.ToLower().IndexOf("image".ToLower())!=-1&&_contentType.ToLower().IndexOf("name".ToLower())==-1)
					if(_contentType.ToLower().IndexOf("name".ToLower())==-1)
					{
						strRet=sr.ReadLine();
						if(strRet.IndexOf("filename=\"")!=-1)
						{
							_contentFileName=Utility.GetQuotedValue(strRet,"=","filename");
							_contentFileName=Utility.deCode(_contentFileName);
						}
						else if(strRet.IndexOf("name=\"")!=-1)
						{
							_contentFileName=Utility.GetQuotedValue(strRet,"=","name");
							_contentFileName=Utility.deCode(_contentFileName);
						}
						else
						{
							parseHeader(sr,strRet);
						}
					}
					break;
				case "CONTENT-TRANSFER-ENCODING":
					_contentTransferEncoding=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
				case "CONTENT-DESCRIPTION":
					_contentDescription=Utility.deCode(Utility.splitOnSemiColon(array[1])[0].Trim());
					break;
				case "CONTENT-DISPOSITION":
					//_contentDisposition=Utility.splitOnSemiColon(array[1])[0].Trim();
//					_contentFileName=Utility.splitOnSemiColon(array[1])[1].Trim();
//					_contentFileName=_contentFileName.Substring(10,_contentFileName.Length-11);
					if(values.Length>0)
						_contentDisposition=values[0].Trim();
					
					_contentFileName=values[1];
					
					if(_contentFileName=="")
						_contentFileName=sr.ReadLine();

					_contentFileName=Utility.GetQuotedValue(_contentFileName,"=","filename");
					_contentFileName=Utility.deCode(_contentFileName);
					break;
				case "CONTENT-ID":
					_contentID=Utility.splitOnSemiColon(array[1])[0].Trim("<".ToCharArray()).Trim(">".ToCharArray());
					break;
			}
		}

		/// <summary>
		/// verify the encoding
		/// </summary>
		/// <param name="encoding">encoding to verify</param>
		/// <returns>true if encoding</returns>
		public bool IsEncoding(string encoding)
		{
			return _contentTransferEncoding.ToLower().IndexOf(encoding.ToLower())!=-1;
		}

		/// <summary>
		/// Decode the attachment to text
		/// </summary>
		/// <returns>Decoded attachment text</returns>
		public string DecodeAttachmentAsText()
		{
			string decodedAttachment=null;

			if(_contentType.ToLower()=="message/rfc822".ToLower())
				decodedAttachment=Utility.deCode(_rawAttachment);
			else if(_contentTransferEncoding!=null)
			{
				decodedAttachment=_rawAttachment;

				if(!IsEncoding("7bit"))
				{
					if(IsEncoding("8bit")&&_contentCharset!=null&_contentCharset!="")
						decodedAttachment=Utility.Change(decodedAttachment,_contentCharset);

					if(Utility.IsQuotedPrintable(_contentTransferEncoding))
						decodedAttachment=DecodeQP.ConvertHexContent(decodedAttachment);
					else if(IsEncoding("8bit"))
						decodedAttachment=decodedAttachment;
					else
						decodedAttachment=Utility.deCodeB64s(Utility.RemoveNonB64(decodedAttachment));
					//decodedAttachment=Encoding.Default.GetString(Convert.FromBase64String(Utility.RemoveNonB64(decodedAttachment)));
				}
			}
			else if(_contentCharset!=null)
				decodedAttachment=Utility.Change(_rawAttachment,_contentCharset);//Encoding.Default.GetString(Encoding.GetEncoding(_contentCharset).GetBytes(_rawAttachment));
			else
				decodedAttachment=_rawAttachment;

			return decodedAttachment;
		}

		public Message DecodeAsMessage()
		{
			return new Message(null,_rawAttachment,false);
		}

		/// <summary>
		/// Decode the attachment to bytes
		/// </summary>
		/// <returns>Decoded attachment bytes</returns>
		public byte[] DecodedAttachmentAsBytes()
		{
			if(_rawAttachment==null)
				return null;
			if(_contentFileName!="")
				//return Convert.FromBase64String(Utility.RemoveNonB64(_rawAttachment));
				//return Encoding.Default.GetBytes(DecodeAttachment());
			{
				byte []decodedBytes=null;

				if(_contentType!=null && _contentType.ToLower()=="message/rfc822".ToLower())
					decodedBytes=Encoding.Default.GetBytes(Utility.deCode(_rawAttachment));
				else if(_contentTransferEncoding!=null)
				{
					string content=_rawAttachment;

					if(!IsEncoding("7bit"))
					{
						if(IsEncoding("8bit")&&_contentCharset!=null&_contentCharset!="")
							content=Utility.Change(content,_contentCharset);

						if(Utility.IsQuotedPrintable(_contentTransferEncoding))
							decodedBytes=Encoding.Default.GetBytes(DecodeQP.ConvertHexContent(content));
						else if(IsEncoding("8bit"))
							decodedBytes=Encoding.Default.GetBytes(content);
						else
							decodedBytes=Convert.FromBase64String(Utility.RemoveNonB64(content));
					}
					else
						decodedBytes=Encoding.Default.GetBytes(content);
				}
				else if(_contentCharset!=null)
					decodedBytes=Encoding.Default.GetBytes(Utility.Change(_rawAttachment,_contentCharset));//Encoding.Default.GetString(Encoding.GetEncoding(_contentCharset).GetBytes(_rawAttachment));
				else
					decodedBytes=Encoding.Default.GetBytes(_rawAttachment);

				return decodedBytes;
			}
			else
			{
				return null;
			}
		}
	}
}
