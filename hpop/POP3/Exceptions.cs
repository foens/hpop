/*
*Name:			OpenPOP.POP3
*Function:		exceptions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/4/2 21:25 GMT-8 by Unruled Boy
*Description	:
*Changes:		2004/4/2 21:25 GMT-8 by Unruled Boy
*					1.added PopServerLockException
*/

using System;

namespace OpenPOP.POP3
{
	/// <summary>
	/// PopServerNotAvailableException.
	/// </summary>
	public class PopServerNotAvailableException:Exception
	{}

	/// <summary>
	/// PopServerNotFoundException
	/// </summary>
	public class PopServerNotFoundException:Exception
	{}

	/// <summary>
	/// AttachmentEncodingNotSupportedException
	/// </summary>
	public class AttachmentEncodingNotSupportedException:Exception
	{}

	/// <summary>
	/// InvalidLoginException
	/// </summary>
	public class InvalidLoginException:Exception
	{}

	/// <summary>
	/// InvalidPasswordException
	/// </summary>
	public class InvalidPasswordException:Exception
	{}

	/// <summary>
	/// InvalidLoginOrPasswordException
	/// </summary>
	public class InvalidLoginOrPasswordException:Exception
	{}

	/// <summary>
	/// PopServerLockException
	/// </summary>
	public class PopServerLockException:Exception
	{}

}
