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
*Name:			OpenPOP.POP3
*Function:		exceptions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		3 May 2004 0200 GMT+5 by Hamid Qureshi
*Description:
*Changes:		2004/4/2 21:25 GMT+8 by Unruled Boy
*					1.added PopServerLockException
*				3 May 2004 0200 GMT+5 by Hamid Qureshi
*					1.Adding NDoc Comments
*/
using System;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Thrown when the POP3 Server sends an error (-ERR) during intial handshake (HELO)
	/// </summary>
	public class PopServerNotAvailableException:Exception
	{}

	/// <summary>
	/// Thrown when the specified POP3 Server can not be found or connected with
	/// </summary>	
	public class PopServerNotFoundException:Exception
	{}

	/// <summary>
	/// Thrown when the attachment is not in a format supported by OpenPOP.NET
	/// </summary>
	/// <remarks>Supported attachment encodings are Base64,Quoted Printable,MS TNEF</remarks>
	public class AttachmentEncodingNotSupportedException:Exception
	{}

	/// <summary>
	/// Thrown when the supplied login doesn't exist on the server
	/// </summary>
	/// <remarks>Should be used only when using USER/PASS Authentication Method</remarks>
	public class InvalidLoginException:Exception
	{}

	/// <summary>
	/// Thrown when the password supplied for the login is invalid
	/// </summary>	
	/// <remarks>Should be used only when using USER/PASS Authentication Method</remarks>
	public class InvalidPasswordException:Exception
	{}

	/// <summary>
	/// Thrown when either the login or the password is invalid on the POP3 Server
	/// </summary>
	/// /// <remarks>Should be used only when using APOP Authentication Method</remarks>
	public class InvalidLoginOrPasswordException:Exception
	{}

	/// <summary>
	/// Thrown when the user mailbox is in a locked state
	/// </summary>
	/// <remarks>The mail boxes are locked when an existing session is open on the mail server. Lock conditions are also met in case of aborted sessions</remarks>
	public class PopServerLockException:Exception
	{}

}

