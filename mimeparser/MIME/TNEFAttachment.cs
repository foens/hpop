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

/*********************************************************************
* Based on tnef.c from Thomas Boll 
**********************************************************************/

/*
*Name:			OpenPOP.MIMEParser.TNEFAttachment
*Function:		TNEFAttachment
*Author:		Thomas Boll(c version), Unruled Boy(c# version)
*Created:		2004/3
*Modified:		2004/5/1 14:13 GMT+8 by Unruled Boy
*Description:
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

