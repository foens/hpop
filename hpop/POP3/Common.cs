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
}