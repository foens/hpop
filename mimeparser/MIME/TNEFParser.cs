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

/*********************************************************************
* Based on tnef.c from Thomas Boll 
**********************************************************************/

/*
*Name:			OpenPOP.MIMEParser.TNEFParser
*Function:		MS TNEF Parser
*Author:		Thomas Boll(c version), Unruled Boy(c# version)
*Created:		2004/3
*Modified:		2004/5/1 14:13 GMT+8 by Unruled Boy
*Description:
*Changes:		
*				2004/5/1 14:13 GMT+8 by Unruled Boy
*					1.Adding descriptions to every public functions/property/void
*/
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace OpenPOP.MIMEParser
{
	/// <summary>
	/// OpenPOP.MIMEParser.TNEFParser
	/// </summary>
	public class TNEFParser
	{
		#region Member Variables
		private const int TNEF_SIGNATURE  =0x223e9f78;
		private const int LVL_MESSAGE     =0x01;
		private const int LVL_ATTACHMENT  =0x02;
		private const int _string			=0x00010000;
		private const int _BYTE			=0x00060000;
		private const int _WORD			=0x00070000;
		private const int _DWORD			=0x00080000;

		private const int AVERSION      =(_DWORD|0x9006);
		private const int AMCLASS       =(_WORD|0x8008);
		private const int ASUBJECT      =(_DWORD|0x8004);
		private const int AFILENAME     =(_string|0x8010);
		private const int ATTACHDATA    =(_BYTE|0x800f);

		private Stream fsTNEF;
		private Hashtable _attachments=new Hashtable();
		private TNEFAttachment _attachment=null;

		private bool _verbose = false;
		//private string _logFile="OpenPOP.TNEF.log";
		private string _basePath=null;
		private int _skipSignature = 0;
		private bool _searchSignature = false;
		private long _offset = 0;
		private long _fileLength=0;
		private string _tnefFile="";
		private string strSubject;
		#endregion

		#region Properties
		//		public string LogFilePath
		//		{
		//			get{return _logFile;}
		//			set{_logFile=value;}
		//		}

		public string TNEFFile
		{
			get{return _tnefFile;}
			set{_tnefFile=value;}
		}

		public bool Verbose
		{
			get{return _verbose;}
			set{_verbose=value;}
		}

		public string BasePath
		{
			get{return _basePath;}
			set
			{
				try
				{
					if(value.EndsWith("\\"))
						_basePath=value;
					else
						_basePath=value+"\\";
				}
				catch
				{
				}
			}
		}

		public int SkipSignature
		{
			get{return _skipSignature;}
			set{_skipSignature=value;}
		}

		public bool SearchSignature
		{
			get{return _searchSignature;}
			set{_searchSignature=value;}
		}

		public long Offset
		{
			get{return _offset;}
			set{_offset=value;}
		}
		#endregion


		private int GETINT32(byte[] p)
		{
			return (p[0]+(p[1]<<8)+(p[2]<<16)+(p[3]<<24));
		}

		private short GETINT16(byte[] p)
		{
			return (short)(p[0]+(p[1]<<8));
		}

		private int geti32 () 
		{
			byte[] buf=new byte[4];

			if(StreamReadBytes(buf,4)!=1)
			{
				Utility.LogError("geti32():unexpected end of input\n");
				return 1;
			}
			return GETINT32(buf);
		}

		private int geti16 () 
		{
			byte[] buf=new byte[2];

			if(StreamReadBytes(buf,2)!=1)
			{
				Utility.LogError("geti16():unexpected end of input\n");
				return 1;
			}
			return GETINT16(buf);
		}

		private int geti8 () 
		{
			byte[] buf=new byte[1];

			if(StreamReadBytes(buf,1)!=1)
			{
				Utility.LogError("geti8():unexpected end of input\n");
				return 1;
			}
			return (int)buf[0];
		}

		private int StreamReadBytes(byte[] buffer, int size)
		{
			try
			{
				if(fsTNEF.Position+size<=_fileLength)					
				{
					fsTNEF.Read(buffer,0,size);
					return 1;
				}
				else
					return 0;
			}
			catch(Exception e)				
			{				
				Utility.LogError("StreamReadBytes():"+e.Message);
				return 0;
			}
		}

		private void CloseTNEFStream()
		{
			try
			{
				fsTNEF.Close();
			}
			catch(Exception e)
			{
				Utility.LogError("CloseTNEFStream():"+e.Message);
			}
		}

		/// <summary>
		/// Open the MS-TNEF stream from file
		/// </summary>
		/// <param name="strFile">MS-TNEF file</param>
		/// <returns></returns>
		public bool OpenTNEFStream(string strFile)
		{
			//Utility.LogFilePath=LogFilePath;

			TNEFFile=strFile;
			try
			{
				fsTNEF=new FileStream(strFile,FileMode.Open,FileAccess.Read);
				FileInfo fi=new FileInfo(strFile);
				_fileLength=fi.Length;
				fi=null;
				return true;
			}
			catch(Exception e)
			{
				Utility.LogError("OpenTNEFStream(File):"+e.Message);
				return false;
			}
		}

		/// <summary>
		/// Open the MS-TNEF stream from bytes
		/// </summary>
		/// <param name="bytContents">MS-TNEF bytes</param>
		/// <returns></returns>
		public bool OpenTNEFStream(byte[] bytContents)
		{
			//Utility.LogFilePath=LogFilePath;

			try
			{
				fsTNEF=new MemoryStream(bytContents);
				_fileLength=bytContents.Length;
				return true;
			}
			catch(Exception e)
			{
				Utility.LogError("OpenTNEFStream(Bytes):"+e.Message);
				return false;
			}
		}

		/// <summary>
		/// Find the MS-TNEF signature
		/// </summary>
		/// <returns>true if found, vice versa</returns>
		public bool FindSignature()
		{
			bool ret=false;
			long lpos=0;

			int d;

			try
			{
				for (lpos=0; ; lpos++) 
				{

					if (fsTNEF.Seek(lpos,SeekOrigin.Begin)==-1)
					{
						PrintResult("No signature found\n");
						return false;
					}

					d = geti32();
					if (d == TNEF_SIGNATURE) 
					{
						PrintResult("Signature found at {0}\n", lpos);
						break;
					}
				}
				ret=true;
			}
			catch(Exception e)
			{
				Utility.LogError("FindSignature():"+e.Message);
				ret=false;
			}

			fsTNEF.Position=lpos;

			return ret;
		}

		private void decode_attribute (int d) 
		{
			byte[] buf=new byte[4000];
			int len;
			int v;
			int i;

			len = geti32();   /* data length */

			switch(d&0xffff0000)
			{
				case _BYTE:
					PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=1) 
					{
						v = geti8();

						if (i< 10) PrintResult(" {0}", v);
						else if (i==10) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _WORD:
					PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=2) 
					{
						v = geti16();

						if (i < 6) PrintResult(" {0}", v);
						else if (i==6) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _DWORD:
					PrintResult("Attribute {0} =", d&0xffff);
					for (i=0; i < len; i+=4) 
					{
						v = geti32();

						if (i < 4) PrintResult(" {0}", v);
						else if (i==4) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _string:
					StreamReadBytes(buf, len);

					PrintResult("Attribute {0} = {1}\n", d&0xffff, Encoding.Default.GetString(buf));
					break;
				default:
					StreamReadBytes(buf, len);
					PrintResult("Attribute {0}\n", d);
					break;
			}

			geti16();     /* checksum */
		}

		private void decode_message() 
		{  
			int d;

			d = geti32();

			decode_attribute(d);
		}

		private void decode_attachment() 
		{  
			byte[] buf=new byte[4096];
			int d;
			int len;
			int i,chunk;
    
			d = geti32();

			switch (d) 
			{
				case ASUBJECT:
					len = geti32();

					StreamReadBytes(buf,len);

					byte[] _subjectBuffer=new byte[len-1];

					Array.Copy(buf,_subjectBuffer,(long)len-1);

					strSubject=Encoding.Default.GetString(_subjectBuffer);

					PrintResult("Found subject: {0}", strSubject);

					geti16();     /* checksum */ 

					break;

				case AFILENAME:
					len = geti32();
					StreamReadBytes(buf,len);
					//PrintResult("File-Name: {0}\n", buf);
					byte[] _fileNameBuffer=new byte[len-1];
					Array.Copy(buf,_fileNameBuffer,(long)len-1);

					if (_fileNameBuffer == null) _fileNameBuffer = Encoding.Default.GetBytes("tnef.dat");
					string strFileName=Encoding.Default.GetString(_fileNameBuffer);

					PrintResult("{0}: WRITING {1}\n", BasePath, strFileName);

					//new attachment found because attachment data goes before attachment name
					_attachment.FileName=strFileName;
					_attachment.Subject=strSubject;
					_attachments.Add(_attachment.FileName,_attachment);

					geti16();     /* checksum */ 

					break;

				case ATTACHDATA:
					len = geti32();
					PrintResult("ATTACH-DATA: {0} bytes\n", len);

					_attachment=new TNEFAttachment();
					_attachment.FileContent=new byte[len];
					_attachment.FileLength=len;

					for (i = 0; i < len; ) 
					{
						chunk = len-i;
						if (chunk > buf.Length) chunk = buf.Length;

						StreamReadBytes(buf,chunk);

						Array.Copy(buf,0,_attachment.FileContent,i,chunk);

						i += chunk;
					}

					geti16();     /* checksum */ 
		
					break;
		  
				default:
					decode_attribute(d);
					break;
			}
		}

		/// <summary>
		/// decoded attachments
		/// </summary>
		/// <returns>attachment array</returns>
		public Hashtable Attachments()
		{
			return _attachments;
		}

		/// <summary>
		/// save all decoded attachments to files
		/// </summary>
		/// <returns>true is succeded, vice versa</returns>
		public bool SaveAttachments()
		{
			bool blnRet=false;
			IDictionaryEnumerator ideAttachments=_attachments.GetEnumerator();

			while(ideAttachments.MoveNext())
			{
				blnRet=SaveAttachment((TNEFAttachment)ideAttachments.Value);
			}

			return blnRet;
		}

		/// <summary>
		/// save a decoded attachment to file
		/// </summary>
		/// <param name="attachment">decoded attachment</param>
		/// <returns>true is succeded, vice versa</returns>
		public bool SaveAttachment(TNEFAttachment attachment)
		{
			try
			{
				string strOutFile=BasePath+attachment.FileName;

				if(File.Exists(strOutFile))
					File.Delete(strOutFile);
				FileStream fsData=new FileStream(strOutFile,FileMode.CreateNew,FileAccess.Write);

				fsData.Write(attachment.FileContent,0,(int)attachment.FileLength);

				fsData.Close();

				return true;
			}
			catch(Exception e)
			{
				Utility.LogError("SaveAttachment():"+e.Message);
				return false;
			}
		}

		/// <summary>
		/// parse MS-TNEF stream
		/// </summary>
		/// <returns>true is succeded, vice versa</returns>
		public bool Parse()
		{
			byte[] buf=new byte[4];
			int d;

			if(FindSignature())
			{
				if (SkipSignature < 2) 
				{
					d = geti32();
					if (SkipSignature < 1) 
					{
						if (d != TNEF_SIGNATURE) 
						{
							PrintResult("Seems not to be a TNEF file\n");
							return false;
						}
					}
				}

				d = geti16();
				PrintResult("TNEF Key is: {0}\n", d);
				for (;;) 
				{
					if(StreamReadBytes(buf,1)==0) 
						break;

					d = (int)buf[0];

					switch (d) 
					{
						case LVL_MESSAGE:
							PrintResult("{0}: Decoding Message Attributes\n",fsTNEF.Position);
							decode_message();
							break;
						case LVL_ATTACHMENT:
							PrintResult("Decoding Attachment\n");
							decode_attachment();
							break;
						default:
							PrintResult("Coding Error in TNEF file\n");
							return false;
					}
				}
				return true;
			}
			else
				return false;
		}

		private void PrintResult(string strResult, params object[] strContent)
		{
			string strRet=string.Format(strResult,strContent);
			if (Verbose) 
				Utility.LogError(strRet);
		}

		~TNEFParser()
		{
			_attachments=null;
			CloseTNEFStream();
		}

		public TNEFParser()
		{
		}

		/// <summary>
		/// open MS-TNEF stream from a file
		/// </summary>
		/// <param name="strFile">MS-TNEF file</param>
		public TNEFParser(string strFile)
		{
			OpenTNEFStream(strFile);
		}	

		/// <summary>
		/// open MS-TNEF stream from bytes
		/// </summary>
		/// <param name="bytContents">MS-TNEF bytes</param>
		public TNEFParser(byte[] bytContents)
		{
			OpenTNEFStream(bytContents);
		}
	}
}

