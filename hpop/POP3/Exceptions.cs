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
