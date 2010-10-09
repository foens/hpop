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

		[Test]
		public void TestISO8859_9CharacterSet()
		{
			const string input = "Content-Type: text/plain; charset=iso-8859-9\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"\x00D0\x00DD\x00DE\x00F0\x00FD\x00FE";

			const string expectedOutput = "ĞİŞğış"; // not þ which is 8859-1. See http://en.wikipedia.org/wiki/ISO/IEC_8859-9

			string output = new Message(false, input, false).MessageBody[0].Body;

			Assert.AreEqual(expectedOutput, output);
		}

		[Test]
		public void TestWindows1254CharacterSet()
		{
			const string input = "Content-Type: text/plain; charset=windows-1254\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"\x00D0\x00DD\x00DE\x00F0\x00FD\x00FE";

			// Windows-1254 is compatible with iso 8859-9
			// http://en.wikipedia.org/wiki/Windows-1254
			const string expectedOutput = "ĞİŞğış"; // not þ which is 8859-1. See http://en.wikipedia.org/wiki/ISO/IEC_8859-9


			string output = new Message(false, input, false).MessageBody[0].Body;

			Assert.AreEqual(expectedOutput, output);
		}

		[Test]
		public void TestISO8859_1CharacterSet()
		{
			const string input = "Content-Type: text/plain; charset=iso-8859-1\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"\x00FE\x00E6";

			// Windows-1254 is compatible with iso 8859-9
			// http://en.wikipedia.org/wiki/Windows-1254
			const string expectedOutput = "þæ"; // http://da.wikipedia.org/wiki/ISO_8859-1


			string output = new Message(false, input, false).MessageBody[0].Body;

			Assert.AreEqual(expectedOutput, output);
		}
	}
}
