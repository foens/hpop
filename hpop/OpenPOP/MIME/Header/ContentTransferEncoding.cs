namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// Enum that describes the ContentTransferEncoding header field
	/// </summary>
	/// <see cref="http://www.ietf.org/rfc/rfc2045.txt">Section 6 or Section 2 for more details</see>
	public enum ContentTransferEncoding
	{
		SevenBit,
		EightBit,
		QuotedPrintable,
		Base64,
		Binary
	}
}