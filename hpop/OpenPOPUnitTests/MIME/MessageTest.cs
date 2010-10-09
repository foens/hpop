using NUnit.Framework;
using OpenPOP.MIME;

namespace OpenPOPUnitTests.MIME
{
	[TestFixture]
	public class MessageTest
	{
		[Test]
		public void TestLineEndingsNotStrippedAwayAtEnd()
		{
			// This is a test which uses Quoted-Printable encoding to see if
			// the message content is stripped.
			// Quoted-Printable does need the last \r\n to decode correctly
			const string input = "Content-Type: text/plain; charset=iso-8859-1\r\n" +
			"Content-Transfer-Encoding: quoted-printable\r\n" +
			"\r\n" + // Headers end
			"Hello=\r\n"; // This is where the last \r\n should not be removed

			const string expectedOutput = "Hello";

			string output = new Message(false, input, false).MessageBody[0].Body;

			Assert.AreEqual(expectedOutput, output);
		}

		[Test]
		public void TestLineEndingsNotStrippedAwayAtStart()
		{
			const string input = "Content-Type: text/plain; charset=iso-8859-1\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"\r\nHello"; // This is where the first \r\n should not be removed

			const string expectedOutput = "\r\nHello";

			string output = new Message(false, input, false).MessageBody[0].Body;

			Assert.AreEqual(expectedOutput, output);
		}
	}
}
