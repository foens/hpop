/*
*Name:			OpenPOP.POP3.MyMD5
*Function:		MD5
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		3 May 2004 0200 GMT+5 by Hamid Qureshi
*Description:
*Changes:		3 May 2004 0200 GMT+5 by Hamid Qureshi
*					1.Adding NDoc Comments
*Description:
*/
using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Implements wrapper for MD5CryptoServiceProvider
	/// </summary>
	public class MyMD5
	{
		/// <summary>
		/// Get the MD5 hash of a string 
		/// </summary>
		/// <param name="input">The string for which the MD5 hash is required</param>
		/// <returns>The MD5 hash of the input string</returns>
		public static string GetMD5Hash(string input)
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

		/// <summary>
		/// Get the MD5 hash of a string in hexadecimal format
		/// </summary>
		/// <param name="input">The string for which the MD5 hash in hexadecimal is required</param>
		/// <returns>The MD5 hash in hexadecimal fromat of the input string</returns>
		public static string GetMD5HashHex(string input)
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
