using System;
using System.Security.Cryptography;
using System.Text;

namespace OpenPOP.POP3
{
	/// <summary>
	/// Wrapper class for computing MD5 hash
	/// </summary>
	public class MyMD5
	{
		/// <summary>
		/// Calculates the MD5 hash
		/// </summary>
		/// <param name="input">The string on which the hash is being computed</param>
		/// <returns>The MD5 Hash</returns>
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

		/// <summary>
		/// Calculates the MD5 hash in Hexadecimal format
		/// </summary>
		/// <param name="input">The string on which the hash is being computed</param>
		/// <returns>The MD5 Hash in Hexadecimal format</returns>
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
