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

namespace OpenPOP.MIME
{
	/// <summary>
	/// TNEFAttachment
	/// </summary>
	public class TNEFAttachment
	{
		#region Properties

	    /// <summary>
	    /// attachment subject
	    /// </summary>
	    public string Subject { get; set; }

	    /// <summary>
	    /// attachment file length
	    /// </summary>
	    public long FileLength { get; set; }

	    /// <summary>
	    /// attachment file name
	    /// </summary>
	    public string FileName { get; set; }

	    /// <summary>
	    /// attachment file content
	    /// </summary>
	    public byte[] FileContent { get; set; }

	    #endregion


		public TNEFAttachment()
		{
		    FileContent = null;
		    FileName = "";
		    Subject = "";
		}

	    ~TNEFAttachment()
		{
			FileContent=null;
		}
	}
}

