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
*Function:		Common definitions
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		3rd May 1600 GMT+5
*Description:
*Changes:		3rd May 1600 GMT+5 by Hamid Qureshi
*					1.Adding NDoc Comments
*/
using System;

namespace OpenPOP.POP3
{

	/// <summary>
	/// Possible responses received from the server when performing an Authentication
	/// </summary>
	public enum  AuthenticationResponse
	{
		/// <summary>
		/// Authentication succeeded
		/// </summary>
		SUCCESS=0,
		/// <summary>
		/// Login doesn't exist on the POP3 server
		/// </summary>		
		INVALIDUSER=1,
		/// <summary>
		/// Password is invalid for the give login
		/// </summary>
		INVALIDPASSWORD=2,
		/// <summary>
		/// Invalid login and/or password
		/// </summary>
		INVALIDUSERORPASSWORD=3
	}		

	/// <summary>
	/// Authentication method to use
	/// </summary>
	/// <remarks>TRYBOTH means code will first attempt by using APOP method as its more secure.
	///  In case of failure the code will fall back to USERPASS method.
	/// </remarks>
	public enum  AuthenticationMethod
	{
		/// <summary>
		/// Authenticate using the USER/PASS method. USER/PASS is secure but all POP3 servers may not support this method
		/// </summary>
		USERPASS=0,
		/// <summary>
		/// Authenticate using the APOP method
		/// </summary>
		APOP=1,
		/// <summary>
		/// Authenticate using USER/PASS. In case USER/PASS fails then revert to APOP
		/// </summary>
		TRYBOTH=2
	}

}
