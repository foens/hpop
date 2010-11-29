using System.Collections.Specialized;
using NUnit.Framework;
using OpenPop.Mime.Header;

namespace OpenPopUnitTests.Mime.Header
{
	[TestFixture]
	public class MessageHeaderTests
	{
		[Test]
		public void TestSingleInReplyTo()
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("In-Reply-To", "<test@test.com>");

			MessageHeader header = new MessageHeader(collection);

			Assert.IsNotEmpty(header.InReplyTo);
			Assert.AreEqual(1, header.InReplyTo.Count);
			Assert.AreEqual("test@test.com", header.InReplyTo[0]);
		}

		[Test]
		public void TestMultipleInReplyToWithWhiteSpaceSeparation()
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("In-Reply-To", "<test@test.com> <test2@test2.com>");

			MessageHeader header = new MessageHeader(collection);

			Assert.IsNotEmpty(header.InReplyTo);
			Assert.AreEqual(2, header.InReplyTo.Count);

			Assert.AreEqual("test@test.com", header.InReplyTo[0]);
			Assert.AreEqual("test2@test2.com", header.InReplyTo[1]);
		}

		[Test]
		public void TestMultipleInReplyToNoWithspaceSeparation()
		{
			NameValueCollection collection = new NameValueCollection();
			collection.Add("In-Reply-To", "<test@test.com><test2@test2.com>");

			MessageHeader header = new MessageHeader(collection);

			Assert.IsNotEmpty(header.InReplyTo);
			Assert.AreEqual(2, header.InReplyTo.Count);

			Assert.AreEqual("test@test.com", header.InReplyTo[0]);
			Assert.AreEqual("test2@test2.com", header.InReplyTo[1]);
		}
	}
}
