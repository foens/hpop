using System;

namespace OpenPOP
{
	/// <summary>
	/// Summary description for Utility.
	/// </summary>
	public class Utility
	{
		public Utility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Sepearte header name and header value
		/// </summary>
		/// <param name="RawHeader"></param>
		/// <returns></returns>
		public static string[] getHeadersValue(string RawHeader)
		{
			if(RawHeader==null)
				throw new ArgumentNullException("RawHeader","Argument was null");

			string []array=new String[2]{"",""};
			int indexOfColon=RawHeader.IndexOf(":");			

			try
			{
				array[0]=RawHeader.Substring(0,indexOfColon).Trim();
				array[1]=RawHeader.Substring(indexOfColon+1).Trim();
			}
			catch(Exception){}

			return array;
		}


		public static string[] splitOnSemiColon(string objString)
		{
			if(objString==null)
				throw new ArgumentNullException("RawHeader","Argument was null");

			string []array=null;
			int indexOfColon=objString.IndexOf(";");			

			if(indexOfColon<0)
			{
				array=new String[1];
				array[0]=objString;
				return array;
			}
			else
			{
				array=new String[2];
			}

			try
			{
				array[0]=objString.Substring(0,indexOfColon).Trim();
				array[1]=objString.Substring(indexOfColon+1).Trim();
			}
			catch(Exception){}

			return array;
		}

	}
}
