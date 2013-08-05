using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenPop.Async.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPopAsyncUnitTests;
using OpenPopAsyncUnitTests.Utils;

// ReSharper disable CheckNamespace
namespace OpenPopUnitTests.Pop3
// ReSharper restore CheckNamespace
{
	/// <summary>
	/// This TestFixture is testing that when the <see cref="Pop3AsyncClient"/> is in an connected state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
	class PopAsyncClientConnectedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";

		private Pop3AsyncClient Client;

		[SetUp]
		public void Init()
		{
            Client = new Pop3AsyncClient();
		}

		/// <summary>
		/// Let the <see cref="Pop3Client"/> connect.
		/// </summary>
		/// <param name="extraReaderInput">Extra input that the server may read off the reader. String is convert to bytes using ASCII encoding</param>
		private async Task Connect(string extraReaderInput = "")
		{
			string readerInput = "+OK\r\n" + extraReaderInput; // Always allow connect, which is the first ok
			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(readerInput));
			Stream outputStream = new MemoryStream();

			// Connect with the client
			await Client.ConnectAsync(new CombinedStream(inputStream, outputStream));
		}

		[Test]
		public void TestAuthenticateDoesNotThrow()
		{
			Connect("+OK\r\n+OK\r\n"); // Allow username and password
			AsyncAssert.DoesNotThrow(Client.AuthenticateAsync(RandomString, RandomString));
		}

		[Test]
		public async Task TestDeleteAllMessages()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.DeleteAllMessagesAsync());
		}

		[Test]
		public async Task TestDeleteMessage()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.DeleteMessageAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestDisconnectDoesNotThrow()
		{
			await Connect("+OK"); // OK to quit command
            AsyncAssert.DoesNotThrow(Client.DisconnectAsync());
		}

		[Test]
		public async Task TestGetMessage()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageCount()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageCountAsync());
		}

		[Test]
		public async Task TestGetMessageHeaders()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageHeadersAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageSize()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageSizeAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageSizes()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageSizesAsync());
		}

		[Test]
		public async Task TestGetMessageUID()
		{
			await Connect();
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageUidAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageUIDs()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.GetMessageUidsAsync());
		}

		[Test]
		public async Task TestNOOP()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.NoOperationAsync());
		}

		[Test]
		public async Task TestRSET()
		{
			await Connect();
		    AsyncAssert.Throws<InvalidUseException>(Client.ResetAsync());
		}

		[Test]
		public async Task TestConnect()
		{
			await Connect();

			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes("+OK")); // Welcome message
			Stream writer = new MemoryStream();

			// Try connect again
		    AsyncAssert.Throws<InvalidUseException>(Client.ConnectAsync(new CombinedStream(inputStream, writer)));
		}
	}
}