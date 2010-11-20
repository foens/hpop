using System;

namespace OpenPop.Mime.Header
{
	/// <summary>
	/// <see cref="Enum"/> that describes the ContentTransferEncoding header field
	/// </summary>
	/// <remarks>See <a href="http://www.ietf.org/rfc/rfc2045.txt">RFC 2045</a> Section 6 or Section 2 for more details</remarks>
	public enum ContentTransferEncoding
	{
		/// <summary>
		/// 7 bit Encoding
		/// </summary>
		SevenBit,

		/// <summary>
		/// 8 bit Encoding
		/// </summary>
		EightBit,

		/// <summary>
		/// Quoted Printable Encoding
		/// </summary>
		QuotedPrintable,

		/// <summary>
		/// Base64 Encoding
		/// </summary>
		Base64,

		/// <summary>
		/// Binary Encoding
		/// </summary>
		Binary
	}
}