/******************************************************************************
	Copyright 2003-2004 Hamid Qureshi and Unruled Boy 
	iOfficeMail.Net is free software; you can redistribute it and/or modify
	it under the terms of the Lesser GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	iOfficeMail.Net is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	Lesser GNU General Public License for more details.

	You should have received a copy of the Lesser GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/

/*
*Name:			iOfficeMail.MIMEParser.QuotedCoding
*Function:		Quoted Coding
*Author:		Hamid Qureshi
*Created:		2003/8
*Modified:		2004/3
*Description:
*/
using System;
using System.Text;

namespace iOfficeMail.MIMEParser
{
	/// <summary>
	/// Summary description for Coding.
	/// </summary>
	public class QuotedCoding
	{
		/// <summary>
		/// zwraca tablice bajtow
		/// zamienia 3 znaki np '=A9' na odp wartosc.
		/// zamienia '_' na znak 32
		/// </summary>
		/// <param name="s">Kupis_Pawe=B3</param>
		/// <returns>Kupis Pawe?/returns>
		public static byte[] GetByteArray(string s)
		{
			byte[] buffer=new byte[s.Length];

			int bufferPosition=0;
			if (s.Length>1)
			{
				for(int i=0;i<s.Length;i++)
				{
					if (s[i]=='=')
					{
						if (s[i+1]=='\r' && s[i+2]=='\n')
							bufferPosition--;
						else
							buffer[bufferPosition]=System.Convert.ToByte(s.Substring(i+1,2),16);
						i+=2;
					}
					else if (s[i]=='_')
						buffer[bufferPosition]=32;
					else
						buffer[bufferPosition]=(byte)s[i];
					bufferPosition++;
				}
			}
			else
			{
				buffer[bufferPosition]=32;
			}

			byte[] newArray=new byte[bufferPosition];
			Array.Copy(buffer,newArray,bufferPosition);
			return newArray;
		}

		/// <summary>
		/// Decoduje string "=?iso-8859-2?Q?Kupis_Pawe=B3?=" 
		/// lub zakodowany base64
		/// na poprawny
		/// </summary>
		/// <param name="s">"=?iso-8859-2?Q?Kupis_Pawe=B3?="</param>
		/// <returns>Kupis Pawe?/returns>
		public static string DecodeOne(string s)
		{
			char[] separator={'?'};
			string[] sArray=s.Split(separator);
			if (sArray[0].Equals("=")==false)
				return s;
			
			byte[] bArray;
			//rozpoznaj rodzj kodowania
			if (sArray[2].ToUpper()=="Q") //querystring
				bArray=GetByteArray(sArray[3]);
			else if (sArray[2].ToUpper()=="B")//base64
				bArray=Convert.FromBase64String(sArray[3]);
			else
				return s;
			//pobierz strone kodowa
			Encoding encoding=Encoding.GetEncoding(sArray[1]); 
			return encoding.GetString(bArray);
		}

		/// <summary>
		/// decoduje string zamienia wpisy (=?...?=) na odp wartosci
		/// </summary>
		/// <param name="s">"ala i =?iso-8859-2?Q?Kupis_Pawe=B3?= ma kota"</param>
		/// <returns>"ala i Pawe?Kupis ma kota"</returns>
		public static string Decode(string s)
		{
			StringBuilder retstring=new StringBuilder();
			int old=0,start=0,stop;
			for(;;)
			{
				start=s.IndexOf("=?",start);
				if (start==-1)
				{
					retstring.Append(s,old,s.Length-old);
					return retstring.ToString();
				}
				stop=s.IndexOf("?=",start+2);
				if (stop==-1) //blad w stringu
					return s;
				retstring.Append(s,old,start-old);
				retstring.Append(DecodeOne(s.Substring(start,stop-start+2)));
				start=stop+2;
				old=stop+2;
			}
		}

	}
}

