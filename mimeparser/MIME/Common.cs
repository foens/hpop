/*
*Name:			COM.NET.MAIL.POP.MIMEParser
*Function:		Common definitions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:28 GMT+8
*Description:
*				2004/3/30 09:15 GMT+8 by Unruled Boy
*					1.Adding ImportanceType
*/
using System;

namespace COM.NET.MAIL.POP.MIMEParser
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
