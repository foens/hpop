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
*Name:			OpenPOP.MIMEParser.Attachment
*Function:		Attachment
*Author:		Hamid Qureshi
*Created:		2003/8
*Last Modified:	2004/5/28 10:19 GMT+8 by Unruled Boy
*Description:
*Changes:		
*				2004/5/28 10:19 GMT+8 by grandepuffo via Unruled Boy
*					1.Fixed a bug in parsing ContentFileName @ https://sourceforge.net/forum/message.php?msg_id=2589759
*				2004/5/17 14:20 GMT+8 by Unruled Boy
*					1.Fixed a bug in parsing FileName
*				2004/5/8 17:00 GMT+8 by Unruled Boy
*					1.Again, hopefully we have handled the NotAttachment property correctly
*				2004/5/1 14:13 GMT+8 by Unruled Boy
*					1.Adding three more constructors
*					2.Adding descriptions to every public functions/property/void
*				2004/4/29 19:05 GMT+8 by Unruled Boy
*					1.Hopefully we have handled the NotAttachment property correctly
*				2004/3/29 10:28 GMT+8 by Unruled Boy
*					1.removing bugs in decoding attachment
*				2004/3/29 17:32 GMT+8 by Unruled Boy
*					1.support for reply message using ms-tnef 
*					2.adding detail description for every function
*					3.cleaning up the codes
*/
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// Summary description for Attachment.
	/// </summary>
	public class Attachment : IComparable
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
		private string _defaultMSTNEFFileName="winmail.dat";
		private string _contentID=null;
		private long _contentLength=0;
		private string _rawAttachment=null;
		private bool _inBytes=false;
		private byte[] _rawBytes=null;
		#endregion


		#region Properties
		/// <summary>
		/// raw attachment content bytes
		/// </summary>
		public byte[] RawBytes
		{
			get{return _rawBytes;}
			set{_rawBytes=value;}
		}

		/// <summary>
		/// whether attachment is in bytes
		/// </summary>
		public bool InBytes
		{
			get{return _inBytes;}
			set{_inBytes=value;}
		}

		/// <summary>
		/// Content length
		/// </summary>
		public long ContentLength
		{
			get{return _contentLength;}
		}

		/// <summary>
		/// verify the attachment whether it is a real attachment or not
		/// </summary>
		/// <remarks>this is so far not comprehensive and needs more work to finish</remarks>
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
				if ((_contentType==null||_contentFileName=="") && _contentID==null)//&&_contentType.ToLower().IndexOf("text/")!=-1)
					return true;
				else
					return false;

			}
		}

		/// <summary>
		/// Content format
		/// </summary>
		public string ContentFormat
		{
			get{return _contentFormat;}
		}

		/// <summary>
		/// Content charset
		/// </summary>
		public string ContentCharset
		{
			get{return _contentCharset;}
		}

		/// <summary>
		/// default file name
		/// </summary>
		public string DefaultFileName
		{
			get{return _defaultFileName;}
			set{_defaultFileName=value;}
		}

		/// <summary>
		/// default file name 2
		/// </summary>
		public string DefaultFileName2
		{
			get{return _defaultFileName2;}
			set{_defaultFileName2=value;}
		}

		/// <summary>
		/// default report file name
		/// </summary>
		public string DefaultReportFileName
		{
			get{return _defaultReportFileName;}
			set{_defaultReportFileName=value;}
		}

		/// <summary>
		/// default MIME File Name
		/// </summary>
		public string DefaultMIMEFileName
		{
			get{return _defaultMIMEFileName;}
			set{_defaultMIMEFileName=value;}
		}

		/// <summary>
		/// Content Type
		/// </summary>
		public string ContentType
		{
			get{return _contentType;}
		}

		/// <summary>
		/// Content Transfer Encoding
		/// </summary>
		public string ContentTransferEncoding
		{
			get{return _contentTransferEncoding;}
		}

		/// <summary>
		/// Content Description
		/// </summary>
		public string ContentDescription
		{
			get{return _contentDescription;}
		}

		/// <summary>
		/// Content File Name
		/// </summary>
		public string ContentFileName
		{
			get{return _contentFileName;}
			set{_contentFileName=value;}
		}

		/// <summary>
		/// Content Disposition
		/// </summary>
		public string ContentDisposition
		{
			get{return _contentDisposition;}
		}

		/// <summary>
		/// Content ID
		/// </summary>
		public string ContentID
		{
			get{return _contentID;}
		}

		/// <summary>
		/// Raw Attachment
		/// </summary>
		public string RawAttachment
		{
			get{return _rawAttachment;}
		}

		/// <summary>
		/// decoded attachment in bytes
		/// </summary>
		public byte[] DecodedAttachment
		{
			get
			{
				return DecodedAsBytes();
			}
		}
		#endregion


		/// <summary>
		/// release all objects
		/// </summary>
		~Attachment()
		{
			_rawBytes=null;
			_rawAttachment=null;
		}

		/// <summary>
		/// New Attachment
		/// </summary>
		/// <param name="bytAttachment">attachment bytes content</param>
		/// <param name="lngFileLength">file length</param>
		/// <param name="strFileName">file name</param>
		/// <param name="strContentType">content type</param>
		public Attachment(byte[] bytAttachment, long lngFileLength, string strFileName, string strContentType)
		{
			_inBytes=true;
			_rawBytes=bytAttachment;
			_contentLength=lngFileLength;
			_contentFileName=strFileName;
			_contentType=strContentType;
		}

		/// <summary>
		/// New Attachment
		/// </summary>
		/// <param name="bytAttachment">attachment bytes content</param>
		/// <param name="strFileName">file name</param>
		/// <param name="strContentType">content type</param>
		public Attachment(byte[] bytAttachment, string strFileName, string strContentType)
		{
			_inBytes=true;
			_rawBytes=bytAttachment;
			_contentLength=bytAttachment.Length;
			_contentFileName=strFileName;
			_contentType=strContentType;
		}

		/// <summary>
		/// New Attachment
		/// </summary>
		/// <param name="strAttachment">attachment content</param>
		/// <param name="strContentType">content type</param>
		/// <param name="blnParseHeader">whether only parse the header or not</param>
		public Attachment(string strAttachment,string strContentType, bool blnParseHeader)
		{
			if(!blnParseHeader)
			{
				_contentFileName=_defaultMSTNEFFileName;
				_contentType=strContentType;
			}
			this.NewAttachment(strAttachment,blnParseHeader);
		}

		/// <summary>
		/// New Attachment
		/// </summary>
		/// <param name="strAttachment">attachment content</param>
		public Attachment(string strAttachment)
		{	
			this.NewAttachment(strAttachment,true);
		}

		/// <summary>
		/// create attachment
		/// </summary>
		/// <param name="strAttachment">raw attachment text</param>
		/// <param name="blnParseHeader">parse header</param>
		private void NewAttachment(string strAttachment, bool blnParseHeader)
		{
			_inBytes=false;

			if(strAttachment==null)
				throw new ArgumentNullException("strAttachment");

			StringReader srReader=new StringReader(strAttachment);

			if(blnParseHeader)
			{
				string strLine=srReader.ReadLine();
				while(Utility.IsNotNullTextEx(strLine))
				{
					ParseHeader(srReader,ref strLine);
					if(Utility.IsOrNullTextEx(strLine))
						break;
					else
						strLine=srReader.ReadLine();
				}
			}

			this._rawAttachment=srReader.ReadToEnd();
			_contentLength=this._rawAttachment.Length;
		}

		/// <summary>
		/// Parse header fields and set member variables
		/// </summary>
		/// <param name="srReader">string reader</param>
		/// <param name="strLine">header line</param>
		private void ParseHeader(StringReader srReader,ref string strLine)
		{
			string []array=Utility.GetHeadersValue(strLine);//Regex.Split(strLine,":");
			string []values=Regex.Split(array[1],";");//array[1].Split(';');
			string strRet=null;

			switch(array[0].ToUpper())
			{
				case "CONTENT-TYPE":
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
					_contentFileName=Utility.ParseFileName(strLine);
					if(_contentFileName=="")
					{
						strRet=srReader.ReadLine();
						if(strRet=="")
						{
							strLine="";
							break;
						}
						_contentFileName=Utility.ParseFileName(strLine);
						if(_contentFileName=="")
							ParseHeader(srReader,ref strRet);
					}
					break;
				case "CONTENT-TRANSFER-ENCODING":
					_contentTransferEncoding=Utility.SplitOnSemiColon(array[1])[0].Trim();
					break;
				case "CONTENT-DESCRIPTION":
					_contentDescription=Utility.DecodeText(Utility.SplitOnSemiColon(array[1])[0].Trim());
					break;
				case "CONTENT-DISPOSITION":
					if(values.Length>0)
						_contentDisposition=values[0].Trim();
					
					///<bug>reported by grandepuffo @ https://sourceforge.net/forum/message.php?msg_id=2589759
					//_contentFileName=values[1];
					if(values.Length>1) 
					{
						_contentFileName=values[1];
					} 
					else 
					{
						_contentFileName="";
					}
					
					if(_contentFileName=="")
						_contentFileName=srReader.ReadLine();

					_contentFileName=_contentFileName.Replace("\t","");
					_contentFileName=Utility.GetQuotedValue(_contentFileName,"=","filename");
					_contentFileName=Utility.DecodeText(_contentFileName);
					break;
				case "CONTENT-ID":
					_contentID=Utility.SplitOnSemiColon(array[1])[0].Trim('<').Trim('>');
					break;
			}
		}

		/// <summary>
		/// verify the encoding
		/// </summary>
		/// <param name="encoding">encoding to verify</param>
		/// <returns>true if encoding</returns>
		private bool IsEncoding(string encoding)
		{
			return _contentTransferEncoding.ToLower().IndexOf(encoding.ToLower())!=-1;
		}

		/// <summary>
		/// Decode the attachment to text
		/// </summary>
		/// <returns>Decoded attachment text</returns>
		public string DecodeAsText()
		{
			string decodedAttachment=null;

			try
			{
				if(_contentType.ToLower()=="message/rfc822".ToLower())
					decodedAttachment=Utility.DecodeText(_rawAttachment);
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
					}
				}
				else if(_contentCharset!=null)
					decodedAttachment=Utility.Change(_rawAttachment,_contentCharset);//Encoding.Default.GetString(Encoding.GetEncoding(_contentCharset).GetBytes(_rawAttachment));
				else
					decodedAttachment=_rawAttachment;
			}
			catch
			{
				decodedAttachment=_rawAttachment;
			}
			return decodedAttachment;
		}

		/// <summary>
		/// decode attachment to be a message object
		/// </summary>
		/// <returns>message</returns>
		public Message DecodeAsMessage()
		{
			bool blnRet=false;
			return new Message(ref blnRet,"",false ,_rawAttachment,false);
		}

		/// <summary>
		/// Decode the attachment to bytes
		/// </summary>
		/// <returns>Decoded attachment bytes</returns>
		public byte[] DecodedAsBytes()
		{
			if(_rawAttachment==null)
				return null;
			if(_contentFileName!="")
			{
				byte []decodedBytes=null;

				if(_contentType!=null && _contentType.ToLower()=="message/rfc822".ToLower())
					decodedBytes=Encoding.Default.GetBytes(Utility.DecodeText(_rawAttachment));
				else if(_contentTransferEncoding!=null)
				{
					string bytContent=_rawAttachment;

					if(!IsEncoding("7bit"))
					{
						if(IsEncoding("8bit")&&_contentCharset!=null&_contentCharset!="")
							bytContent=Utility.Change(bytContent,_contentCharset);

						if(Utility.IsQuotedPrintable(_contentTransferEncoding))
							decodedBytes=Encoding.Default.GetBytes(DecodeQP.ConvertHexContent(bytContent));
						else if(IsEncoding("8bit"))
							decodedBytes=Encoding.Default.GetBytes(bytContent);
						else
							decodedBytes=Convert.FromBase64String(Utility.RemoveNonB64(bytContent));
					}
					else
						decodedBytes=Encoding.Default.GetBytes(bytContent);
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

		public int CompareTo(object attachment)
		{
			return (this.RawAttachment.CompareTo(((Attachment)(attachment)).RawAttachment));
		}
	}
}

