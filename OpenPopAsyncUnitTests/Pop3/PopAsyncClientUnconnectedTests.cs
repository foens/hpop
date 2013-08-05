using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenPop.Async.Pop3;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPopAsyncUnitTests.Utils;

namespace OpenPopAsyncUnitTests.Pop3
{
	/// <summary>
	/// This TestFixture is testing that when the <see cref="Pop3Client"/> is in an unconnected state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
	class PopAsyncClientUnconnectedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";

        private Pop3AsyncClient Client;

		[SetUp]
		public void init()
		{
			Client = new Pop3AsyncClient();
		}

		[Test]
		public async Task  TestAuthenticate()
		{
			AsyncAssert.Throws<InvalidUseException>(Client.AuthenticateAsync(RandomString, RandomString));
		}

		[Test]
		public async Task  TestDeleteAllMessages()
		{
			AsyncAssert.Throws<InvalidUseException>(Client.DeleteAllMessagesAsync());
		}

		[Test]
		public async Task  TestDeleteMessage()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.DeleteMessageAsync(RandomMessageNumber));
		}

		[Test]
		public async Task  TestDisconnect()
		{
			AsyncAssert.Throws<InvalidUseException>(Client.DisconnectAsync());
		}

		[Test]
		public async Task  TestGetMessage()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageAsync(RandomMessageNumber));
		}

		[Test]
		public async Task  TestGetMessageCount()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageCountAsync());
		}

		[Test]
		public async Task  TestGetMessageHeaders()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageHeadersAsync(RandomMessageNumber));
		}

		[Test]
		public async Task  TestGetMessageSize()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageSizeAsync(RandomMessageNumber));
		}

		[Test]
		public async Task  TestGetMessageSizes()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageSizesAsync());
		}

		[Test]
		public async Task  TestGetMessageUid()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageUidAsync(RandomMessageNumber));
		}

		[Test]
		public async Task  TestGetMessageUids()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageUidsAsync());
		}

		[Test]
		public async Task  TestNoOperation()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.NoOperationAsync());
		}

		[Test]
		public async Task  TestReset()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.ResetAsync());
		}

		[Test]
		public async Task  TestConnectWorks()
		{
			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes("+OK")); // Welcome message
			Stream outputStream = new MemoryStream();

            AsyncAssert.DoesNotThrow(Client.ConnectAsync(new CombinedStream(inputStream, outputStream)));
		}

		[Test]
		public async Task  TestConnectWorksThrowsCorrectExceptionWhenServerDoesNotRespond()
		{
			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes("")); // No welcome message
			Stream outputStream = new MemoryStream();

			// There was a problem, where an InvalidUseException was being thrown, if the server
			// did not respond or the stream was closed. That was the wrong exception
            AsyncAssert.Throws<PopServerNotAvailableException>(Client.ConnectAsync(new CombinedStream(inputStream, outputStream)));
		}

		[Test]
		public async Task  TestCapability()
		{
            AsyncAssert.Throws<InvalidUseException>(Client.CapabilitiesAsync());
		}

		[Test]
		public async Task  TestCapabilityDiposed()
		{
			Client.Dispose();
            AsyncAssert.Throws<ObjectDisposedException>(Client.CapabilitiesAsync());
		}
	}
}