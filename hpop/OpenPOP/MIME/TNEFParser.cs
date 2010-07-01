using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenPOP.MIME
{
    /// <summary>
    /// Used to parse attachments that have the MIME-type Application/MS-TNEF
    /// TNEF stands for Transport Neutral Encapsulation Format, and is proprietary Microsoft attachment format.
    ///
    /// Based on tnef.c from Thomas Boll.
    /// </summary>
    /// <see cref="http://en.wikipedia.org/wiki/Transport_Neutral_Encapsulation_Format">For more details</see>
	public class TNEFParser
	{
		#region Member Variables
		private const int TNEF_SIGNATURE  = 0x223e9f78;
		private const int LVL_MESSAGE     = 0x01;
		private const int LVL_ATTACHMENT  = 0x02;
		private const int _string		  = 0x00010000;
		private const int _BYTE			  = 0x00060000;
		private const int _WORD			  = 0x00070000;
		private const int _DWORD		  = 0x00080000;

		private const int AVERSION      = (_DWORD  | 0x9006); // Unused?
        private const int AMCLASS       = (_WORD   | 0x8008); // Unused?
		private const int ASUBJECT      = (_DWORD  | 0x8004);
		private const int AFILENAME     = (_string | 0x8010);
		private const int ATTACHDATA    = (_BYTE   | 0x800f);

		private Stream fsTNEF;
		private readonly List<TNEFAttachment> _attachments = new List<TNEFAttachment>();
		private TNEFAttachment _attachment=null;

	    //private string _logFile="OpenPOP.TNEF.log";
	    private long _fileLength=0;
	    private string strSubject;
		#endregion

		#region Properties
		//		public string LogFilePath
		//		{
		//			get{return _logFile;}
		//			set{_logFile=value;}
		//		}

	    public string TNEFFile { get; set; }

	    public bool Verbose { get; set; }

	    public int SkipSignature { get; set; }

	    public bool SearchSignature { get; set; }

	    public long Offset { get; set; }

	    #endregion

        #region Constructors
        /// <summary>
        /// Used the set up default values
        /// </summary>
        private TNEFParser()
        {
            Verbose = false;
            TNEFFile = "";
        }

        /// <summary>
        /// Create a TNEFParser which loads its content from a file
        /// </summary>
        /// <param name="strFile">MS-TNEF file</param>
        public TNEFParser(string strFile)
            : this()
        {
            if (!OpenTNEFStream(strFile))
                throw new ArgumentException();
        }

        /// <summary>
        /// Create a TNEFParser which loads its content from a byte array
        /// </summary>
        /// <param name="bytContents">MS-TNEF bytes</param>
        public TNEFParser(byte[] bytContents)
            : this()
        {
            if (!OpenTNEFStream(bytContents))
                throw new ArgumentException();
        }

        ~TNEFParser()
        {
            CloseTNEFStream();
        }
        #endregion


		private static int GETINT32(byte[] p)
		{
			return (p[0]+(p[1]<<8)+(p[2]<<16)+(p[3]<<24));
		}

		private static short GETINT16(byte[] p)
		{
			return (short)(p[0]+(p[1]<<8));
		}

		private int geti32() 
		{
			byte[] buf=new byte[4];

			if(StreamReadBytes(buf,4)!=1)
			{
				Utility.LogError("geti32():unexpected end of input\n");
				return 1;
			}
			return GETINT32(buf);
		}

		private int geti16() 
		{
			byte[] buf=new byte[2];

			if(StreamReadBytes(buf,2)!=1)
			{
				Utility.LogError("geti16():unexpected end of input\n");
				return 1;
			}
			return GETINT16(buf);
		}

		private int geti8() 
		{
			byte[] buf=new byte[1];

			if(StreamReadBytes(buf,1)!=1)
			{
				Utility.LogError("geti8():unexpected end of input\n");
				return 1;
			}
			return buf[0];
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
		private bool OpenTNEFStream(string strFile)
		{
			TNEFFile=strFile;
			try
			{
				fsTNEF=new FileStream(strFile,FileMode.Open,FileAccess.Read);
				FileInfo fi=new FileInfo(strFile);
				_fileLength=fi.Length;
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
		private bool OpenTNEFStream(byte[] bytContents)
		{
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
			bool ret;
			long lpos=0;

		    try
			{
				for (lpos=0; ; lpos++) 
				{

					if (fsTNEF.Seek(lpos,SeekOrigin.Begin)==-1)
					{
						PrintResult("No signature found\n");
						return false;
					}

					int d = geti32();
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
		    int v;
			int i;

			int len = geti32();

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
		    int d = geti32();

		    decode_attribute(d);
		}

        private void decode_attachment() 
		{  
			byte[] buf=new byte[4096];
		    int len;

		    int d = geti32();

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

					string strFileName=Encoding.Default.GetString(_fileNameBuffer);

					//new attachment found because attachment data goes before attachment name
					_attachment.FileName=strFileName;
					_attachment.Subject=strSubject;
					_attachments.Add(_attachment);

					geti16();     /* checksum */ 

					break;

				case ATTACHDATA:
					len = geti32();
					PrintResult("ATTACH-DATA: {0} bytes\n", len);

					_attachment=new TNEFAttachment();
					_attachment.FileContent=new byte[len];
					_attachment.FileLength=len;

			        for (int i = 0; i < len; ) 
					{
						int chunk = len-i;
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
		public List<TNEFAttachment> Attachments()
		{
			return _attachments;
		}

		/// <summary>
		/// save all decoded attachments to files
		/// </summary>
		/// <returns>true is succeded, vice versa</returns>
		public bool SaveAttachments(string pathToSaveTo)
		{
			bool blnRet=false;

		    foreach (TNEFAttachment tnefAttachment in _attachments)
		    {
		        blnRet = SaveAttachment(tnefAttachment, pathToSaveTo);
		    }

			return blnRet;
		}

        /// <summary>
        /// save a decoded attachment to file
        /// </summary>
        /// <param name="attachment">decoded attachment</param>
        /// <param name="pathToSaveTo">Where to save the attachment to</param>
        /// <returns>true is succeded, vice versa</returns>
        public static bool SaveAttachment(TNEFAttachment attachment, string pathToSaveTo)
		{
			try
			{
                string strOutFile = pathToSaveTo + attachment.FileName;

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

		    if(FindSignature())
			{
			    int d;
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

					d = buf[0];

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
			
			return false;
		}

		private void PrintResult(string strResult, params object[] strContent)
		{
			string strRet=string.Format(strResult,strContent);
			if (Verbose) 
				Utility.LogError(strRet);
		}
	}
}