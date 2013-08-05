using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenPop.Async.Pop3;
using OpenPop.Pop3.Exceptions;
using OpenPopAsyncUnitTests.Utils;

namespace OpenPopAsyncUnitTests.Pop3
{
    /// <summary>
	/// This TestFixture is testing that when the <see cref="Pop3AsyncClient"/> is in an authenticated state, commands which may not
	/// be used in that state, throws <see cref="InvalidUseException"/>.
	/// 
	/// Also tests that commands which is can be used in this state, does not throw any exceptions
	/// </summary>
	[TestFixture]
    class PopAsyncClientAuthenticatedTests
	{
		private const int RandomMessageNumber = 5;
		private const string RandomString = "random";
		private const string RandomMessage = "Content-Type: text/plain; charset=iso-8859-1\r\n" +
			"Content-Transfer-Encoding: 7bit\r\n" +
			"\r\n" + // Headers end
			"Hello";

        private Pop3AsyncClient Client;

		[SetUp]
		public void Init()
		{
			Client = new Pop3AsyncClient();
		}

		/// <summary>
		/// Let the <see cref="Pop3AsyncClient"/> authenticate.
		/// </summary>
		/// <param name="extraReaderInput">Extra input that the server may read off the reader.  String is convert to bytes using ASCII encoding</param>
		private async Task Authenticate(string extraReaderInput = "")
		{
			// Always allow connect, which is the first ok
			// And always allow authenticate
			string readerInput = "+OK\r\n+OK\r\n+OK\r\n" + extraReaderInput;
			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes(readerInput));
			Stream outputStream = new MemoryStream();

			// Authenticate with the client
			await Client.ConnectAsync(new CombinedStream(inputStream, outputStream));
            await Client.AuthenticateAsync(RandomString, RandomString);
		}

		[Test]
		public async Task TestAuthenticate()
		{
			await Authenticate("+OK\r\n+OK\r\n"); // Allow username and password		    
            AsyncAssert.Throws<InvalidUseException>(Client.AuthenticateAsync(RandomString, RandomString));            
		}

		[Test]
        public async Task TestDeleteAllMessagesDoesNotThrow()
		{
			await Authenticate("+OK 0"); // First message count is asked for, we return 0
			Assert.IsNull(Client.DeleteAllMessagesAsync().Exception);
		}

		[Test]
		public async Task TestDeleteMessageDoesNotThrow()
		{
			await Authenticate("+OK"); // Message deleted succesfully
			Assert.IsNull(Client.DeleteMessageAsync(RandomMessageNumber).Exception);
		}
        
		[Test]
		public async Task TestDeleteMessageDoesThrowWhenWrongMessageNumberPassed()
		{
			await Authenticate("+OK"); // Message deleted succesfully
            AsyncAssert.Throws<InvalidUseException>(Client.DeleteMessageAsync(0));			
		}

		[Test]
		public async Task TestDisconnectDoesNotThrow()
		{
			await Authenticate("+OK"); // OK to quit command
			AsyncAssert.DoesNotThrow(Client.DisconnectAsync());
		}

	    [Test]
	    public async Task TestGetMessageDoesThrowWhenWrongMessageNumberPassed()
	    {
	        await Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
	        AsyncAssert.Throws<InvalidUseException>(Client.GetMessageAsync(-1));
	    }

	    [Test]
		public async Task TestGetMessageCountDoesNotThrow()
		{
			await Authenticate("+OK 0 0"); // Message count is 0
            AsyncAssert.DoesNotThrow(Client.GetMessageCountAsync());
		}

		[Test]
		public async Task TestGetMessageHeadersDoesNotThrow()
		{
			await Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
            AsyncAssert.DoesNotThrow(Client.GetMessageHeadersAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageHeadersDoesThrowWhenWrongMessageNumberPassed()
		{
			await Authenticate("+OK\r\n" + RandomMessage + "\r\n."); // We will send message // Message // . ends message
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageHeadersAsync(0));
		}

		[Test]
		public async Task TestGetMessageSizeDoesNotThrow()
		{
			await Authenticate("+OK 5 0"); // Message 5 has size is 0
            AsyncAssert.DoesNotThrow(Client.GetMessageSizeAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageSizeDoesThrowWhenWrongMessageNumberPassed()
		{
            await Authenticate("+OK 5 0"); // Message 5 has size is 0
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageSizeAsync(0));
		}

		[Test]
		public async Task TestGetMessageSizesDoesNotThrow()
		{
            await Authenticate("+OK\r\n1 2\r\n."); // LIST command accepted. // Message 1 has size 2 // . ends answer
            AsyncAssert.DoesNotThrow(Client.GetMessageSizesAsync());
		}

		[Test]
		public async Task TestGetMessageUIDDoesNotThrow()
		{
            await Authenticate("+OK 2 test"); // Message 2 has UID test
            AsyncAssert.DoesNotThrow(Client.GetMessageUidAsync(RandomMessageNumber));
		}

		[Test]
		public async Task TestGetMessageUIDDoesThrowWhenWrongMessageNumberPassed()
		{
            await Authenticate("+OK 2 test"); // Message 2 has UID test
            AsyncAssert.Throws<InvalidUseException>(Client.GetMessageUidAsync(0));
		}

		[Test]
		public async Task TestGetMessageUIDsDoesNotThrow()
		{
            await Authenticate("+OK\r\n1 test\r\n."); // UIDL command accepted. // Message 1 has UID test // . ends answer
            AsyncAssert.DoesNotThrow(Client.GetMessageUidsAsync());
		}

		[Test]
		public async Task TestNOOPDoesNotThrow()
		{
            await Authenticate("+OK"); // NOOP Accepted
            AsyncAssert.DoesNotThrow(Client.NoOperationAsync());
		}

		[Test]
		public async Task TestRSETDoesNotThrow()
		{
            await Authenticate("+OK"); // RSET accepted
            AsyncAssert.DoesNotThrow(Client.ResetAsync());
		}

		[Test]
		public async Task TestConnect()
		{
            await Authenticate();

			Stream inputStream = new MemoryStream(Encoding.ASCII.GetBytes("+OK")); // Welcome message
			Stream outputStream = new MemoryStream();

			// Try connect again
            AsyncAssert.Throws<InvalidUseException>(Client.ConnectAsync(new CombinedStream(inputStream, outputStream)));
		}
	}
}