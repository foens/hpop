namespace OpenPOP.MIME
{
	/// <summary>
	/// TNEFAttachment
	/// 
    /// Based on tnef.c from Thomas Boll 
	/// </summary>
	public class TNEFAttachment
	{
		#region Properties

	    /// <summary>
	    /// attachment subject
	    /// </summary>
	    public string Subject { get; set; }

	    /// <summary>
	    /// attachment file length
	    /// </summary>
	    public long FileLength { get; set; }

	    /// <summary>
	    /// attachment file name
	    /// </summary>
	    public string FileName { get; set; }

	    /// <summary>
	    /// attachment file content
	    /// </summary>
	    public byte[] FileContent { get; set; }

	    #endregion


		public TNEFAttachment()
		{
		    FileContent = null;
		    FileName = "";
		    Subject = "";
		}

	    ~TNEFAttachment()
		{
			FileContent=null;
		}
	}
}

