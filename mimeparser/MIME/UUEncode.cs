using System;

namespace OpenPOP.UUEncode
{
	/// <summary>
	/// UUEncode
	/// </summary>
	public class UUEncode
	{
		public UUEncode()
		{
		}

		public string uuDecode(string sBuffer) 
		{ 
			string str1=String.Empty; 

			int j = sBuffer.Length ; 
			for (int i = 1; i <= j; i += 4) 
			{ 
				str1 = String.Concat(str1, Convert.ToString((char)(((int)Convert.ToChar( sBuffer.Substring ( i-1, 1)) - 32) * 4 + ((int)Convert.ToChar( sBuffer.Substring( i , 1)) - 32) / 16))); 

				str1 = String.Concat(str1, Convert.ToString((char)(((int)Convert.ToChar( sBuffer.Substring( i , 1)) % 16 * 16) + ((int)Convert.ToChar(sBuffer.Substring( i + 1, 1)) - 32) / 4))); 
				str1 = String.Concat(str1, Convert.ToString((char)(((int)Convert.ToChar(sBuffer.Substring ( i + 1, 1)) % 4 * 64) + (int)Convert.ToChar( sBuffer.Substring( i + 2, 1)) - 32))); 
			} 
			return str1; 
		} 

		public string uuEncode(string sBuffer) 
		{ 
			string str1=String.Empty; 

			if (sBuffer.Length % 3 != 0) 
			{ 
				string stuff =new String(' ',3 - sBuffer.Length % 3); 
				sBuffer = String.Concat(sBuffer, stuff); 
			} 
			int j = sBuffer.Length; 
			for (int i = 1; i <= j; i += 3) 
			{ 
				str1 = String.Concat(str1, Convert.ToString( (char)( (int)Convert.ToChar(sBuffer.Substring( i-1, 1)) / 4 + 32))); 
				str1 = String.Concat(str1, Convert.ToString((char)((int)Convert.ToChar(sBuffer.Substring( i-1, 1)) % 4 * 16 + (int)Convert.ToChar(sBuffer.Substring ( i , 1)) / 16 + 32))); 
				str1 = String.Concat(str1, Convert.ToString((char)((int)Convert.ToChar( sBuffer.Substring ( i , 1)) % 16 * 4 + (int)Convert.ToChar(sBuffer.Substring ( i + 1, 1)) / 64 + 32))); 
				str1 = String.Concat(str1, Convert.ToString((char)((int)Convert.ToChar( sBuffer.Substring ( i + 1, 1)) % 64 + 32))); 
			} 
			return str1; 
		} 
	}
}

