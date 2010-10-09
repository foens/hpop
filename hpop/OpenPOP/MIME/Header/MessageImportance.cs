namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// 3 message importance types defined by RFC
	/// </summary>
	/// <remarks>See <a href="http://tools.ietf.org/html/rfc1911#page-8">http://tools.ietf.org/html/rfc1911#page-8</a> under "Importance" for more info.</remarks>
	public enum MessageImportance
	{
		/// <summary>
		/// High Importance
		/// </summary>
		High = 5,
		/// <summary>
		/// Normal Importance
		/// </summary>
		Normal = 3,
		/// <summary>
		/// Low Importance
		/// </summary>
		Low = 1
	}
}