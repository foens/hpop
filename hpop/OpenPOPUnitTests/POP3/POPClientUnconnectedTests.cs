using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPOP.POP3;
using OpenPOP.POP3.Exceptions;

namespace OpenPOPUnitTests.POP3
{
	/// <summary>
	/// This TestFixture is testing that when the <see cref="POPClient"/> is in an unconnected state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
	class POPClientUnconnectedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";

		private POPClient Client;

		[SetUp]
		public void Init()
		{
			Client = new POPClient();
		}

		[Test]
		public void TestAuthenticate()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.Authenticate(RandomString, RandomString); });
		}

		[Test]
		public void TestDeleteAllMessages()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.DeleteAllMessages(); });
		}

		[Test]
		public void TestDeleteMessage()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.DeleteMessage(RandomMessageNumber); });
		}

		[Test]
		public void TestDisconnect()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.Disconnect(); });
		}

		[Test]
		public void TestGetMessage()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessage(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageCount()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageCount(); });
		}

		[Test]
		public void TestGetMessageHeaders()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageHeaders(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageSize()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageSize(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageSizes()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageSizes(); });
		}

		[Test]
		public void TestGetMessageUID()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageUID(RandomMessageNumber); });
		}

		[Test]
		public void TestGetMessageUIDs()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.GetMessageUIDs(); });
		}

		[Test]
		public void TestNOOP()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.NOOP(); });
		}

		[Test]
		public void TestQUIT()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.QUIT(); });
		}

		[Test]
		public void TestRSET()
		{
			Assert.Throws(typeof(InvalidUseException), delegate { Client.RSET(); });
		}

		[Test]
		public void TestConnectWorks()
		{
			StringReader reader = new StringReader("+OK"); // Welcome message
			StringWriter writer = new StringWriter(new StringBuilder());

			Assert.DoesNotThrow(delegate { Client.Connect(reader, writer); });
		}

		[Test]
		public void TestConnectWorksThrowsCorrectExceptionWhenServerDoesNotRespond()
		{
			StringReader reader = new StringReader(""); // No welcome message.
			StringWriter writer = new StringWriter(new StringBuilder());

			// There was a problem, where an InvalidUseException was being thrown, if the server
			// did not respond or the stream was closed. That was the wrong exception
			Assert.Throws(typeof(PopServerNotAvailableException), delegate { Client.Connect(reader, writer); });
		}
	}
}