/*
*Name:			COM.NET.MAIL.POP.POP3
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

namespace COM.NET.MAIL.POP.POP3
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
	/// Thrown when the attachment is not in a format supported by COM.NET.MAIL.POP.NET
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

