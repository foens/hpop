using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPOP.POP3;

namespace OpenPOPUnitTests.POP3
{
	/// <summary>
	/// Tests that when the <see cref="POPClient"/> is in an connected state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
	class POPClientConnectedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";

		private POPClient Client;

		[SetUp]
		public void Init()
		{
			Client = new POPClient();
		}

		/// <summary>
		/// Let the <see cref="POPClient"/> connect.
		/// </summary>
		/// <param name="extraReaderInput">Extra input that the server may read off the reader</param>
		private void Connect(string extraReaderInput = "")
		{
			string readerInput = "+OK\r\n" + extraReaderInput; // Always allow connect, which is the first ok
			StringReader reader = new StringReader(readerInput);
			StringWriter writer = new StringWriter(new StringBuilder());

			// Connect with the client
			Client.Connect(reader, writer);
		}

		[Test]
		public void TestAuthenticateDoesNotThrow()
		{
			Connect("+OK\r\n+OK\r\n"); // Allow username and password
			Assert.DoesNotThrow(delegate { Client.Authenticate(RandomString, RandomString); });
		}

		[Test]
		public void TestDeleteAllMessages()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.DeleteAllMessages(); });
		}

		[Test]
		public void TestDeleteMessage()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.DeleteMessage(RandomMessageNumber); });
		}

		[Test]
		public void TestDisconnectDoesNotThrow()
		{
			Connect();
			Assert.DoesNotThrow(delegate { Client.Disconnect(); });
		}

		[Test]
		public void TestGetMessage()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessage(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageCount()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageCount(); });
		}

		[Test]
		public void TestGetMessageHeaders()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageHeaders(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageSize()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageSize(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageSizes()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageSizes(); });
		}

		[Test]
		public void TestGetMessageUID()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageUID(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageUIDs()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageUIDs(); });
		}

		[Test]
		public void TestNOOP()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.NOOP(); });
		}

		[Test]
		public void TestQUITDoesNotThrown()
		{
			Connect("+OK");
			Assert.DoesNotThrow(delegate { Client.QUIT(); });
		}

		[Test]
		public void TestRSET()
		{
			Connect();
			Assert.Throws(typeof(InvalidUseException), delegate { Client.RSET(); });
		}

		[Test]
		public void TestConnect()
		{
			Connect();

			StringReader reader = new StringReader("+OK"); // Welcome message
			StringWriter writer = new StringWriter(new StringBuilder());

			// Try connect again
			Assert.Throws(typeof(InvalidUseException), delegate { Client.Connect(reader, writer); });
		}
	}
}