using NUnit.Framework;
using OpenPop.Mime.Decode;

namespace OpenPopUnitTests.Mime.Decode
{
	[TestFixture]
	public class UtilityTest
	{
		/// <summary>
		/// Tests the ability of the method to parse out the message header name when the content has been split to another line
		/// </summary>
		[Test]
		public void TestGetHeadersValueSplitLine()
		{
			const string expectedName = "MessageID";
			var header = Utility.GetHeadersValue( string.Format("{0}:",expectedName) );
			Assert.AreEqual( 2, header.Length, "Number of header items" );
			Assert.AreEqual( expectedName, header[0], "Header Name" );
			Assert.AreEqual( string.Empty, header[1], "Header Value" );
		}

		/// <summary>
		/// Tests the ability of the method to parse out the header and content from a single line
		/// </summary>
		[Test]
		public void TestGetHeadersValueSingleLine()
		{
			const string expectedName = "MessageID";
			const string expectedValue = "<1234567890>";
			var header = Utility.GetHeadersValue( string.Format( "{0}: {1}", expectedName, expectedValue ) );
			Assert.AreEqual( 2, header.Length, "Number of header items" );
			Assert.AreEqual( expectedName, header[0], "Header Name" );
			Assert.AreEqual( expectedValue, header[1], "Header Value" );
		}
	}
}