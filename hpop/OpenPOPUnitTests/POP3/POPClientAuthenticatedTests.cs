using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPOP.POP3;

namespace OpenPOPUnitTests.POP3
{
	/// <summary>
	/// This TestFixture is testing that when the <see cref="POPClient"/> is in an authenticated state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
	class POPClientAuthenticatedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";
		private const string RandomMessage = "Content-Type: text/plain; charset=iso-8859-1\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"Hello";

		private POPClient Client;

		[SetUp]
		public void Init()
		{
			Client = new POPClient();
		}

		/// <summary>
		/// Let the <see cref="POPClient"/> authenticate.
		/// </summary>
		/// <param name="extraReaderInput">Extra input that the server may read off the reader</param>
		private void Authenticate(string extraReaderInput = "")
		{
			// Always allow connect, which is the first ok
			// And always allow authenticate
			string readerInput = "+OK\r\n+OK\r\n+OK\r\n" + extraReaderInput;
			StringReader reader = new StringReader(readerInput);
			StringWriter writer = new StringWriter(new StringBuilder());

			// Authenticate with the client
			Client.Connect(reader, writer);
			Client.Authenticate(RandomString, RandomString);
		}

		[Test]
		public void TestAuthenticate()
		{
			Authenticate("+OK\r\n+OK\r\n"); // Allow username and password
			Assert.Throws(typeof(InvalidUseException), delegate { Client.Authenticate(RandomString, RandomString); });
		}

		[Test]
		public void TestDeleteAllMessagesDoesNotThrow()
		{
			Authenticate("+OK 0"); // First message count is asked for, we return 0
			Assert.DoesNotThrow(delegate { Client.DeleteAllMessages(); });
		}

		[Test]
		public void TestDeleteMessageDoesNotThrow()
		{
			Authenticate("+OK"); // Message deleted succesfully
			Assert.DoesNotThrow(delegate { Client.DeleteMessage(RandomMessageNumber); });
		}

		[Test]
		public void TestDeleteMessageDoesThrowWhenWrongMessageNumberPassed()
		{
			Authenticate("+OK"); // Message deleted succesfully
			Assert.Throws(typeof(InvalidUseException), delegate { Client.DeleteMessage(0); });
		}

		[Test]
		public void TestDisconnectDoesNotThrow()
		{
			Authenticate();
			Assert.DoesNotThrow(delegate { Client.Disconnect(); });
		}

		[Test]
		public void TestGetMessageDoesThrowWhenWrongMessageNumberPassed()
		{
			Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessage(-1); });
		}

		[Test]
		public void TestGetMessageCountDoesNotThrow()
		{
			Authenticate("+OK 0 0"); // Message count is 0
			Assert.DoesNotThrow(delegate { Client.GetMessageCount(); });
		}

		[Test]
		public void TestGetMessageHeadersDoesNotThrow()
		{
			Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
			Assert.DoesNotThrow(delegate { Client.GetMessageHeaders(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageHeadersDoesThrowWhenWrongMessageNumberPassed()
		{
			Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageHeaders(0); });
		}

		[Test]
		public void TestGetMessageSizeDoesNotThrow()
		{
			Authenticate("+OK 5 0"); // Message 5 has size is 0
			Assert.DoesNotThrow(delegate { Client.GetMessageSize(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageSizeDoesThrowWhenWrongMessageNumberPassed()
		{
			Authenticate("+OK 5 0"); // Message 5 has size is 0
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageSize(0); });
		}

		[Test]
		public void TestGetMessageSizesDoesNotThrow()
		{
			Authenticate("+OK\r\n1 2\r\n."); // LIST command accepted. // Message 1 has size 2 // . ends answer
			Assert.DoesNotThrow(delegate { Client.GetMessageSizes(); });
		}

		[Test]
		public void TestGetMessageUIDDoesNotThrow()
		{
			Authenticate("+OK 2 test"); // Message 2 has UID test
			Assert.DoesNotThrow(delegate { Client.GetMessageUID(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageUIDDoesThrowWhenWrongMessageNumberPassed()
		{
			Authenticate("+OK 2 test"); // Message 2 has UID test
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageUID(0); });
		}

		[Test]
		public void TestGetMessageUIDsDoesNotThrow()
		{
			Authenticate("+OK\r\n1 test\r\n."); // UIDL command accepted. // Message 1 has UID test // . ends answer
			Assert.DoesNotThrow(delegate { Client.GetMessageUIDs(); });
		}

		[Test]
		public void TestNOOPDoesNotThrow()
		{
			Authenticate("+OK"); // NOOP Accepted
			Assert.DoesNotThrow(delegate { Client.NOOP(); });
		}

		[Test]
		public void TestQUITDoesNotThrown()
		{
			Authenticate("+OK");
			Assert.DoesNotThrow(delegate { Client.QUIT(); });
		}

		[Test]
		public void TestRSETDoesNotThrow()
		{
			Authenticate("+OK"); // RSET accepted
			Assert.DoesNotThrow(delegate { Client.RSET(); });
		}

		[Test]
		public void TestConnect()
		{
			Authenticate();

			StringReader reader = new StringReader("+OK"); // Welcome message
			StringWriter writer = new StringWriter(new StringBuilder());

			// Try connect again
			Assert.Throws(typeof(InvalidUseException), delegate { Client.Connect(reader, writer); });
		}
	}
}