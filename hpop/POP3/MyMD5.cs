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
*Name:			OpenPOP.POP3.MyMD5
*Function:		MD5
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3
*Description	:
*/

using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Summary description for MyMD5.
	/// </summary>
	public class MyMD5
	{
		public static string GetMD5Hash(String input)
		{
			MD5 md5=new MD5CryptoServiceProvider();
			//the GetBytes method returns byte array equavalent of a string
			byte []res=md5.ComputeHash(Encoding.Default.GetBytes(input),0,input.Length);
			char []temp=new char[res.Length];
			//copy to a char array which can be passed to a String constructor
			System.Array.Copy(res,temp,res.Length);
			//return the result as a string
			return new String(temp);
		}

		public static string GetMD5HashHex(String input)
		{
			MD5 md5=new MD5CryptoServiceProvider();
			DES des=new DESCryptoServiceProvider();
			//the GetBytes method returns byte array equavalent of a string
			byte []res=md5.ComputeHash(Encoding.Default.GetBytes(input),0,input.Length);

			String returnThis="";

			for(int i=0;i<res.Length;i++)
			{
				returnThis+=System.Uri.HexEscape((char)res[i]);
			}
			returnThis=returnThis.Replace("%","");
			returnThis=returnThis.ToLower();

			return returnThis;


		}

	}
}
