using System;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// TNEFAttachment
	/// </summary>
	public class TNEFAttachment
	{

		private string _fileName="";
		private long _fileLength=0;
		private string _subject="";
		private byte[] _fileContent=null;


		public string Subject
		{
			get{return _subject;}
			set{_subject=value;}
		}

		public long FileLength
		{
			get{return _fileLength;}
			set{_fileLength=value;}
		}

		public string FileName
		{
			get{return _fileName;}
			set{_fileName=value;}
		}

		public byte[] FileContent
		{
			get{return _fileContent;}
			set{_fileContent=value;}
		}

		public TNEFAttachment()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		~TNEFAttachment()
		{
			_fileContent=null;
		}
	}
}
