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
*Name:			OpenPOP.MIMEParser.Common
*Function:		Common definitions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:28 GMT+8
*Description:
*				2004/3/30 09:15 GMT+8 by Unruled Boy
*					1.Adding ImportanceType
*/

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// 3 message importance types defined by RFC
	/// </summary>
	/// <remarks>
	/// </remarks>
	public enum MessageImportanceType
	{
		HIGH=5,NORMAL=3,LOW=1
	}

}
