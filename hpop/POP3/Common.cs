/*
*Name:			OpenPOP.POP3
*Function:		Common definitions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3/29 12:28 GMT-8
*Description:
*				2004/3/30 09:15 GMT-8 by Unruled Boy
*					1.Adding ImportanceType
*/

using System;

namespace OpenPOP.POP3
{

	/// <summary>
	/// Possible responses when authenticating
	/// </summary>
	public enum  AuthenticationResponse
	{
		SUCCESS=0,INVALIDUSER=1,INVALIDPASSWORD=2,INVALIDUSERORPASSWORD=3
	}		

	/// <summary>
	/// Authentication method to use
	/// </summary>
	/// <remarks>TRYBOTH means code will first attempt by using APOP method as its more secure.
	///  In case of failure the code will fall back to USERPASS method.
	/// </remarks>
	public enum  AuthenticationMethod
	{
		USERPASS=0,APOP=1,TRYBOTH=2
	}

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