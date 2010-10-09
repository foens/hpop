namespace OpenPOP.MIME
{
	/// <summary>
	/// TNEFAttachment
	/// 
	/// Based on tnef.c from Thomas Boll 
	/// </summary>
	internal class TNEFAttachment
	{
		#region Properties
		/// <summary>
		/// attachment subject
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// attachment file length
		/// </summary>
		public long Length { get; set; }

		/// <summary>
		/// attachment file name
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// attachment file content
		/// </summary>
		public byte[] Content { get; set; }
		#endregion

		public TNEFAttachment()
		{
			Content = null;
			FileName = "";
			Subject = "";
		}
	}
}