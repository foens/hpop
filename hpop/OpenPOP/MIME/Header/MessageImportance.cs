namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// 3 message importance types defined by RFC
	/// </summary>
	/// <see cref="http://tools.ietf.org/html/rfc1911#page-8">Under "Importance" for more info.</see>
	public enum MessageImportance
	{
		High = 5, Normal = 3, Low = 1
	}
}