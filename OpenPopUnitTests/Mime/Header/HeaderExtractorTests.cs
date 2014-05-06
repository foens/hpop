using System.Collections.Generic;
using NUnit.Framework;
using OpenPop.Mime.Header;
using System.Collections.Specialized;

namespace OpenPopUnitTests.Mime.Header
{
	[TestFixture]
	public class HeaderExtractorTests
	{
		/// <summary>
		/// Tests the ability of the method to parse out the message header name when the content has been split to another line
		/// </summary>
		[Test]
		public void TestGetHeadersValueSplitLine()
		{
			const string expectedName = "MessageID";
			KeyValuePair<string, string> header = HeaderExtractor.SeparateHeaderNameAndValue( string.Format("{0}:",expectedName) );
			Assert.AreEqual( expectedName, header.Key, "Header Name" );
			Assert.AreEqual( string.Empty, header.Value, "Header Value" );
		}

		/// <summary>
		/// Tests the ability of the method to parse out the header and content from a single line
		/// </summary>
		[Test]
		public void TestGetHeadersValueSingleLine()
		{
			const string expectedName = "MessageID";
			const string expectedValue = "<1234567890>";
			KeyValuePair<string, string> header = HeaderExtractor.SeparateHeaderNameAndValue(string.Format("{0}: {1}", expectedName, expectedValue));
			Assert.AreEqual( expectedName, header.Key, "Header Name" );
			Assert.AreEqual( expectedValue, header.Value, "Header Value" );
		}

		[Test]
		public void TestTabInBase64HeaderValue()
		{
			string base64Header = "Disposition-Notification-To: =?windows-1251?B?ZWFzdXJlLg\r\n"
				+ "\t==?=\r\n"
				+ "\t<user@server.domain>\r\n"
				;

			string expectedName = "easure.";
			string expectedAddress = "user@server.domain";

			NameValueCollection col = HeaderExtractor.ExtractHeaders(base64Header);
			Assert.AreEqual(1, col.Count);

			MessageHeader header = new MessageHeader(col);
			Assert.AreEqual(1, header.DispositionNotificationTo.Count);

			RfcMailAddress address = header.DispositionNotificationTo[0];
			Assert.IsNotNull(address.MailAddress);
			Assert.AreEqual(expectedName, address.MailAddress.DisplayName);
			Assert.AreEqual(expectedAddress, address.MailAddress.Address);
		}

		[Test]
		public void TestSpaceInBase64HeaderValue()
		{
			string base64Header = "Disposition-Notification-To: =?windows-1251?B?ZWFzdXJlLg\r\n"
				+ " ==?=\r\n"
				+ "\t<user@server.domain>\r\n"
				;

			string expectedName = "easure.";
			string expectedAddress = "user@server.domain";

			NameValueCollection col = HeaderExtractor.ExtractHeaders(base64Header);
			Assert.AreEqual(1, col.Count);

			MessageHeader header = new MessageHeader(col);
			Assert.AreEqual(1, header.DispositionNotificationTo.Count);

			RfcMailAddress address = header.DispositionNotificationTo[0];
			Assert.IsNotNull(address.MailAddress);
			Assert.AreEqual(expectedName, address.MailAddress.DisplayName);
			Assert.AreEqual(expectedAddress, address.MailAddress.Address);
		}


		[Test]
		public void TestInvalidParameterCharsetInContentDisposition()
		{
			string base64Header = "Content-Disposition: attachment; charset=windows-1251; filename=\"image.jpg\"";
			string expectedName = "image.jpg";

			NameValueCollection col = HeaderExtractor.ExtractHeaders(base64Header);
			Assert.AreEqual(1, col.Count);

			MessageHeader header = new MessageHeader(col);
			Assert.IsNotNull(header.ContentDisposition);
			Assert.AreEqual(expectedName, header.ContentDisposition.FileName);
		}
	}
}