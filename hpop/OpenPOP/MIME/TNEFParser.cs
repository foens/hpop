using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenPOP.POP3;
using OpenPOP.Shared.Logging;

namespace OpenPOP.MIME
{
	/// <summary>
	/// Used to parse attachments that have the MIME-type Application/MS-TNEF
	/// TNEF stands for Transport Neutral Encapsulation Format, and is proprietary Microsoft attachment format.
	///
	/// Based on tnef.c from Thomas Boll.
	/// </summary>
	/// <remarks>
	/// See <a href="http://en.wikipedia.org/wiki/Transport_Neutral_Encapsulation_Format">http://en.wikipedia.org/wiki/Transport_Neutral_Encapsulation_Format</a> 
	/// for more details
	/// </remarks>
	internal class TNEFParser : Disposable
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
		private TNEFAttachment _attachment;

		private long fileLength;
		private string subject;
		#endregion

		#region Properties
		/// <summary>
		/// The file the parser is associated with
		/// </summary>
		public string TNEFFile { get; set; }

		/// <summary>
		/// Specifies whether to turn of verbose logging output
		/// </summary>
		public bool Verbose { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int SkipSignature { get; set; }

		/// <summary>
		/// The logging interface for the object
		/// </summary>
		private ILog Log { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// Used the set up default values
		/// </summary>
		/// <param name="logger">The logging interface to use</param>
		/// <exception cref="ArgumentNullException">If <paramref name="logger"/> is <see langword="null"/></exception>
		private TNEFParser(ILog logger)
		{
			if (logger == null)
				throw new ArgumentNullException("logger");

			Log = logger;
			Verbose = false;
			TNEFFile = string.Empty;
		}

		/// <summary>
		/// Create a TNEFParser which loads its contents from a file
		/// </summary>
		/// <param name="file">MS-TNEF file</param>
		/// <param name="logger">The logging interface to use</param>
		/// <exception cref="ArgumentNullException">If <paramref name="file"/> or <paramref name="logger"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If <paramref name="file"/> could not be opened</exception>
		public TNEFParser(FileInfo file, ILog logger)
			: this(logger)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			if (!OpenTNEFStream(file))
				throw new ArgumentException("Error opening the file", "file");
		}

		/// <summary>
		/// Create a TNEFParser which loads its contents from a byte array
		/// </summary>
		/// <param name="contents">MS-TNEF bytes</param>
		/// <param name="logger">The logging interface to use</param>
		/// <exception cref="ArgumentNullException">If <paramref name="contents"/> or <paramref name="logger"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If <paramref name="contents"/> could not be opened as a stream</exception>
		public TNEFParser(byte[] contents, ILog logger)
			: this(logger)
		{
			if(contents == null)
				throw new ArgumentNullException("contents");

			if (!OpenTNEFStream(contents))
				throw new ArgumentException("The contents is invalid", "contents");
		}
		#endregion

		private static int GETINT32(byte[] p)
		{
			return (p[0] + (p[1] << 8) + (p[2] << 16) + (p[3] << 24));
		}

		private static short GETINT16(byte[] p)
		{
			return (short) (p[0] + (p[1] << 8));
		}

		private int geti32()
		{
			byte[] buffer = new byte[4];

			if (StreamReadBytes(buffer, 4) != 1)
			{
				Log.LogError("geti32():unexpected end of input\n");
				return 1;
			}
			return GETINT32(buffer);
		}

		private int geti16()
		{
			byte[] buffer = new byte[2];

			if (StreamReadBytes(buffer, 2) != 1)
			{
				Log.LogError("geti16():unexpected end of input\n");
				return 1;
			}
			return GETINT16(buffer);
		}

		private int geti8()
		{
			byte[] buffer = new byte[1];

			if (StreamReadBytes(buffer, 1) != 1)
			{
				Log.LogError("geti8():unexpected end of input\n");
				return 1;
			}
			return buffer[0];
		}

		private int StreamReadBytes(byte[] buffer, int size)
		{
			try
			{
				if (fsTNEF.Position + size <= fileLength)
				{
					fsTNEF.Read(buffer, 0, size);
					return 1;
				}

				return 0;
			} catch (Exception e)
			{
				Log.LogError("StreamReadBytes():" + e.Message);
				return 0;
			}
		}

		private void CloseTNEFStream()
		{
			if (fsTNEF == null)
				return;
			try
			{
				Stream stream = fsTNEF;
				fsTNEF = null;
				stream.Close();
			} catch (Exception e)
			{
				Log.LogError("CloseTNEFStream():" + e.Message);
			}
		}

		/// <summary>
		/// Open the MS-TNEF stream from file
		/// </summary>
		/// <param name="file">MS-TNEF file</param>
		/// <returns></returns>
		private bool OpenTNEFStream(FileInfo file)
		{
			if(file == null)
				throw new ArgumentNullException("file");

			TNEFFile = file.FullName;
			try
			{
				fsTNEF = file.OpenRead();
				fileLength = file.Length;
				return true;
			} catch (Exception e)
			{
				fsTNEF = null;
				Log.LogError("OpenTNEFStream(File):" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Open the MS-TNEF stream from bytes
		/// </summary>
		/// <param name="contents">MS-TNEF bytes</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="contents"/> is <see langword="null"/></exception>
		private bool OpenTNEFStream(byte[] contents)
		{
			if(contents == null)
				throw new ArgumentNullException("contents");

			try
			{
				fsTNEF = new MemoryStream(contents);
				fileLength = contents.Length;
				return true;
			} catch (Exception e)
			{
				fsTNEF = null;
				Log.LogError("OpenTNEFStream(Bytes):" + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Find the MS-TNEF signature
		/// </summary>
		/// <returns>true if found, vice versa</returns>
		public bool FindSignature()
		{
			AssertDisposed();
			bool returner;
			long leftPosition = 0;

			try
			{
				for (leftPosition = 0; ; leftPosition++)
				{
					if (fsTNEF.Seek(leftPosition, SeekOrigin.Begin) == -1)
					{
						PrintResult("No signature found\n");
						return false;
					}

					int d = geti32();
					if (d == TNEF_SIGNATURE)
					{
						PrintResult("Signature found at {0}\n", leftPosition);
						break;
					}
				}
				returner = true;
			} catch (Exception e)
			{
				Log.LogError("FindSignature():" + e.Message);
				returner = false;
			}

			fsTNEF.Position = leftPosition;

			return returner;
		}

		private void decode_attribute(int d)
		{
			byte[] buffer = new byte[4000];
			int i;

			int length = geti32();

			switch (d & 0xffff0000)
			{
				case _BYTE:
					PrintResult("Attribute {0} =", d & 0xffff);
					for (i = 0; i < length; i++)
					{
						int v = geti8();

						if (i < 10) PrintResult(" {0}", v);
						else if (i == 10) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _WORD:
					PrintResult("Attribute {0} =", d & 0xffff);
					for (i = 0; i < length; i += 2)
					{
						int v = geti16();

						if (i < 6) PrintResult(" {0}", v);
						else if (i == 6) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _DWORD:
					PrintResult("Attribute {0} =", d & 0xffff);
					for (i = 0; i < length; i += 4)
					{
						int v = geti32();

						if (i < 4) PrintResult(" {0}", v);
						else if (i == 4) PrintResult("...");
					}
					PrintResult("\n");
					break;
				case _string:
					StreamReadBytes(buffer, length);

					PrintResult("Attribute {0} = {1}\n", d & 0xffff, Encoding.Default.GetString(buffer));
					break;
				default:
					StreamReadBytes(buffer, length);
					PrintResult("Attribute {0}\n", d);
					break;
			}

			geti16(); /* checksum */
		}

		private void decode_message()
		{
			int d = geti32();

			decode_attribute(d);
		}

		private void decode_attachment()
		{
			byte[] buffer = new byte[4096];
			int length;

			int d = geti32();

			switch (d)
			{
				case ASUBJECT:
					length = geti32();

					StreamReadBytes(buffer, length);

					byte[] _subjectBuffer = new byte[length - 1];

					Array.Copy(buffer, _subjectBuffer, (long) length - 1);

					subject = Encoding.Default.GetString(_subjectBuffer);

					PrintResult("Found subject: {0}", subject);

					geti16(); /* checksum */

					break;

				case AFILENAME:
					length = geti32();
					StreamReadBytes(buffer, length);
					//PrintResult("File-Name: {0}\n", buf);
					byte[] _fileNameBuffer = new byte[length - 1];
					Array.Copy(buffer, _fileNameBuffer, (long) length - 1);

					string strFileName = Encoding.Default.GetString(_fileNameBuffer);

					//new attachment found because attachment data goes before attachment name
					_attachment.FileName = strFileName;
					_attachment.Subject = subject;
					_attachments.Add(_attachment);

					geti16(); /* checksum */

					break;

				case ATTACHDATA:
					length = geti32();
					PrintResult("ATTACH-DATA: {0} bytes\n", length);

					_attachment = new TNEFAttachment();
					_attachment.Content = new byte[length];
					_attachment.Length = length;

					for (int i = 0; i < length;)
					{
						int chunk = length - i;
						if (chunk > buffer.Length) chunk = buffer.Length;

						StreamReadBytes(buffer, chunk);

						Array.Copy(buffer, 0, _attachment.Content, i, chunk);

						i += chunk;
					}

					geti16(); /* checksum */

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
			AssertDisposed();

			return _attachments;
		}

		/// <summary>
		/// save all decoded attachments to files
		/// </summary>
		/// <returns><see langword="true"/> if succeeded, <see langword="false"/> otherwise</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="pathToSaveTo"/> is <see langword="null"/></exception>
		public bool SaveAttachments(DirectoryInfo pathToSaveTo)
		{
			AssertDisposed();

			if(pathToSaveTo == null)
				throw new ArgumentNullException("pathToSaveTo");

			bool result = false;

			foreach (TNEFAttachment tnefAttachment in _attachments)
			{
				result = SaveAttachment(tnefAttachment, pathToSaveTo, Log);
			}

			return result;
		}

		/// <summary>
		/// save a decoded attachment to file
		/// </summary>
		/// <param name="attachment">decoded attachment</param>
		/// <param name="pathToSaveTo">Where to save the attachment to</param>
		/// <returns><see langword="true"/> if succeeded, <see langword="false"/> otherwise</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="attachment"/> or <paramref name="pathToSaveTo"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If <paramref name="pathToSaveTo"/> does not exist</exception>
		public static bool SaveAttachment(TNEFAttachment attachment, DirectoryInfo pathToSaveTo)
		{
			if(attachment == null)
				throw new ArgumentNullException("attachment");

			if(pathToSaveTo == null)
				throw new ArgumentNullException("pathToSaveTo");

			if(!pathToSaveTo.Exists)
				throw new ArgumentException("The path " + pathToSaveTo.FullName + " does not exist");

			return SaveAttachment(attachment, pathToSaveTo, DefaultLogger.CreateLogger());
		}

		/// <summary>
		/// save a decoded attachment to file
		/// </summary>
		/// <param name="attachment">decoded attachment</param>
		/// <param name="pathToSaveTo">Where to save the attachment to</param>
		/// <param name="logger">The object to use for logging output</param>
		/// <returns><see langword="true"/> if succeeded, <see langword="false"/> otherwise</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="attachment"/>, <paramref name="pathToSaveTo"/> or <paramref name="logger"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If <paramref name="pathToSaveTo"/> does not exist</exception>
		public static bool SaveAttachment(TNEFAttachment attachment, DirectoryInfo pathToSaveTo, ILog logger)
		{
			if (attachment == null)
				throw new ArgumentNullException("attachment");

			if (pathToSaveTo == null)
				throw new ArgumentNullException("pathToSaveTo");

			if (logger == null)
				throw new ArgumentNullException("logger");

			if (!pathToSaveTo.Exists)
				throw new ArgumentException("The path " + pathToSaveTo.FullName + " does not exist");
			
			try
			{
				FileInfo outFile = new FileInfo(Path.Combine(pathToSaveTo.FullName, attachment.FileName));

				if (outFile.Exists)
					outFile.Delete();

				using (FileStream fsData = outFile.Create())
				{
					fsData.Write(attachment.Content, 0, (int) attachment.Length);
				}

				return true;
			} catch (Exception e)
			{
				logger.LogError("SaveAttachment(): " + e.Message);
				return false;
			}
		}

		/// <summary>
		/// parse MS-TNEF stream
		/// </summary>
		/// <returns><see langword="true"/> if succeeded, <see langword="false"/> otherwise</returns>
		public bool Parse()
		{
			AssertDisposed();
			byte[] buffer = new byte[4];

			if (FindSignature())
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
					if (StreamReadBytes(buffer, 1) == 0)
						break;

					d = buffer[0];

					switch (d)
					{
						case LVL_MESSAGE:
							PrintResult("{0}: Decoding Message Attributes\n", fsTNEF.Position);
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

		private void PrintResult(string result, params object[] content)
		{
			if (Verbose)
			{
				Log.LogDebug(string.Format(result, content));
			}
		}

		/// <summary>
		/// Disposes of the managed resources within the object
		/// </summary>
		/// <param name="disposing">Specifies if managed resources are being disposed of</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				CloseTNEFStream();
				Log = null;
			}

			base.Dispose(disposing);
		}
	}
}