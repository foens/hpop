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
*Name:			OpenPOP.MIMEParser.DecodeQP
*Function:		Decoding Quoted-Printable text
*Author:		coollzh(coollzh@hotmail.com)
*Created:		2003/8
*Modified:		2004/3/29 12:33 GMT+8
*Description:
*Changes:		
*/
using System;
using System.Text;
using System.Globalization;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// Decoding Quoted-Printable text
	/// 
	/// </summary>
	public class DecodeQP
	{
		public DecodeQP()
		{
		}

		/// <summary>
		/// Decoding Quoted-Printable string
		/// </summary>
		/// <param name="Hexstring">Quoted-Printable encoded string</param>
		/// <param name="encode">encoding method</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexToString(string Hexstring,string Encoding)
		{
			try
			{return ConvertHexToString(Hexstring, System.Text.Encoding.GetEncoding(Encoding));}
			catch
			{return ConvertHexContent(Hexstring);}
		}

		/// <summary>
		/// Decoding Quoted-Printable string
		/// </summary>
		/// <param name="Hexstring">Quoted-Printable encoded string</param>
		/// <param name="encode">encoding method</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexToString(string Hexstring,Encoding encode)
		{			
			try
			{
				if(Hexstring==null||Hexstring.Equals("")) return "";

				if(Hexstring.StartsWith("=")) Hexstring=Hexstring.Substring(1);
			
				string[] aHex= Hexstring.Split(new char[1]{'='});
				byte[] abyte = new Byte[aHex.Length];
			
				for(int i=0;i<abyte.Length;i++)
				{
					//	Console.WriteLine(aHex[i]);
					abyte[i] =(byte) int.Parse(aHex[i],NumberStyles.HexNumber);
				}
				return encode.GetString(abyte);
			}
			catch
			{
				return Hexstring;
			}
		}

		/// <summary>
		/// Decoding Quoted-Printable string at a position
		/// </summary>
		/// <param name="Hexstring">Quoted-Printable encoded string</param>
		/// <param name="encode">encoding method, "Default" is suggested</param>
		/// <param name="nStart">position to start, normally 0</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexContent(string Hexstring,Encoding encode,long nStart)
		{			
			if(nStart>=Hexstring.Length) return Hexstring;

			//to hold string to be decoded
			StringBuilder sbHex = new  StringBuilder();
			sbHex.Append("");
			//to hold decoded string
			StringBuilder sbEncoded = new StringBuilder();
			sbEncoded.Append("");
			//wether we reach Quoted-Printable string
			bool isBegin = false;
			string temp;
			int i = (int)nStart;

			while(i<Hexstring.Length )
			{
				//init next loop
				sbHex.Remove(0,sbHex.Length);
				isBegin = false;
				int count=0;

				while(i<Hexstring.Length )
				{
					temp = Hexstring.Substring(i,1);//before reaching Quoted-Printable string, one char at a time
					if(temp.StartsWith("=")) 
					{
						temp = Hexstring.Substring(i,3);//get 3 chars
						if(temp.EndsWith("\r\n"))//return char
						{
							if(isBegin&& (count % 2==0))
								break;								
							//	sbEncoded.Append("");
							i=i+3;
						}
						else if(!temp.EndsWith("3D"))
						{
							sbHex.Append(temp);
							isBegin = true;//we reach Quoted-Printable string, put it into buffer
							i=i+3;
							count++;
						}
						else //if it ends with 3D, it is "="
						{
							if(isBegin&& (count % 2==0)) //wait until even items to handle all character sets
								break;	
							
							sbEncoded.Append("=");
							i=i+3;
						}

					}
					else
					{
						if(isBegin)//we have got the how Quoted-Printable string, break it
							break;	
						sbEncoded.Append(temp);//not Quoted-Printable string, put it into buffer
						i++;
					}

				}
				//decode Quoted-Printable string
				sbEncoded.Append(ConvertHexToString(sbHex.ToString(),encode)); 
			}
			
			return sbEncoded.ToString();		
		}


		/// <summary>
		/// Decoding Quoted-Printable string using default encoding and begin at 0
		/// </summary>
		/// <param name="Hexstring">Quoted-Printable encoded string</param>
		/// <returns>decoded string</returns>
		public static string ConvertHexContent(string Hexstring)
		{
			if(Hexstring==null || Hexstring.Equals("")) return Hexstring;

			return ConvertHexContent(Hexstring,Encoding.Default,0);
			
		}
	}
}
