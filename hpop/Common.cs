using System;

namespace OpenPOP
{

	public enum  AuthenticationResponse
	{
		SUCCESS=0,INVALIDUSER=1,INVALIDPASSWORD=2,INVALIDUSERORPASSWORD=3
	}		

	public enum  AuthenticationMethod
	{
		USERPASS=0,APOP=1,TRYBOTH=2
	}			
}