using System;
using System.IO;

namespace OpenPOP
{
	/// <summary>
	/// Summary description for Attachment.
	/// </summary>
	public class Attachment
	{
		#region Member Variables
		private string _contentType=null;
		private string _contentTransferEncoding=null;
		private string _contentDescription=null;
		private string _contentDisposition=null;
		private string _contentID=null;
		private string _rawAttachment=null;
		#endregion

		#region Properties

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


		public byte[] DecodedAttachment
		{
			get
			{
				return GetDecodedAttachment();
			}
		}


		#endregion

		public Attachment(string sAttachment)
		{	
			if(sAttachment==null)
				throw new ArgumentNullException("sAttachment");

			StringReader sr=new StringReader(sAttachment);

			string temp=null;
			while( (temp=sr.ReadLine())!=null & temp!="")
			{
				parseHeader(temp);
			}

			this._rawAttachment=sr.ReadToEnd();
		}


		/// <summary>
		/// Parse header fields and set member variables
		/// </summary>
		/// <param name="temp"></param>

		private void parseHeader(string temp)
		{
			string []array=Utility.getHeadersValue(temp);

			switch(array[0].ToUpper())
			{				
				case "CONTENT-TYPE":
					_contentType=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
				case "CONTENT-TRANSFER-ENCODING":
					_contentTransferEncoding=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
				case "CONTENT-DESCRIPTION":
					_contentDescription=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
				case "CONTENT-DISPOSITION":
					_contentDisposition=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
				case "Content-Id":
					_contentID=Utility.splitOnSemiColon(array[1])[0].Trim();
					break;
			}
		}


		public byte[] GetDecodedAttachment()
		{
			if(_rawAttachment==null)
				return null;

			return Convert.FromBase64String(_rawAttachment);			
		}
	}
}
