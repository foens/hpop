/*********************************************************************
* Based on tnef.c from Thomas Boll 
**********************************************************************/

/*
*Name:			OpenPOP.MIMEParser.Message
*Function:		Message Parser
*Author:		Thomas Boll(c version), Unruled Boy(c# version)
*Created:		2004/3
*Modified:		2004/5/1 14:13 GMT+8 by Unruled Boy
*Description	:
*Changes:		
*				2004/5/1 14:13 GMT+8 by Unruled Boy
*					1.Adding descriptions to every public functions/property/void
*/
using System;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// TNEFAttachment
	/// </summary>
	public class TNEFAttachment
	{

		#region Member Variables
		private string _fileName="";
		private long _fileLength=0;
		private string _subject="";
		private byte[] _fileContent=null;
		#endregion


		#region Properties
		/// <summary>
		/// attachment subject
		/// </summary>
		public string Subject
		{
			get{return _subject;}
			set{_subject=value;}
		}

		/// <summary>
		/// attachment file length
		/// </summary>
		public long FileLength
		{
			get{return _fileLength;}
			set{_fileLength=value;}
		}

		/// <summary>
		/// attachment file name
		/// </summary>
		public string FileName
		{
			get{return _fileName;}
			set{_fileName=value;}
		}

		/// <summary>
		/// attachment file content
		/// </summary>
		public byte[] FileContent
		{
			get{return _fileContent;}
			set{_fileContent=value;}
		}
		#endregion


		public TNEFAttachment()
		{
		}

		~TNEFAttachment()
		{
			_fileContent=null;
		}
	}
}
