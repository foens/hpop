/*
*Name:			OpenPOP.POP3
*Function:		exceptions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/27 12:37 GMT-8
*Description	:
*/

using System;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Summary description for PopServerNotFoundException.
	/// </summary>
	public class PopServerNotAvailableException:Exception
	{}

	public class PopServerNotFoundException:Exception
	{}

	public class AttachmentEncodingNotSupportedException:Exception
	{}

	public class InvalidLoginException:Exception
	{}

	public class InvalidPasswordException:Exception
	{}

	public class InvalidLoginOrPasswordException:Exception
	{}
}
