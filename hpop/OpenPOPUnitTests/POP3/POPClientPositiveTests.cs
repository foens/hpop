using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPOP.MIME.Header;
using OpenPOP.POP3;

namespace OpenPOPUnitTests.POP3
{
	[TestFixture]
	public class POPClientPositiveTests
	{
		/// <summary>
		/// This test comes from the RFC 1939 example located at 
		/// http://tools.ietf.org/html/rfc1939#page-16
		/// </summary>
		[Test]
		public void TestAPOPAuthentication()
		{
			const string welcomeMessage = "+OK POP3 server ready <1896.697170952@dbc.mtview.ca.us>";
			const string loginMessage = "+OK mrose's maildrop has 2 messages (320 octets)";
			const string serverResponses = welcomeMessage + "\r\n" + loginMessage + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);

			// The POPClient should now have seen, that the server supports APOP
			Assert.IsTrue(client.APOPSupported);

			client.Authenticate("mrose", "tanstaaf", AuthenticationMethod.APOP);

			const string expectedOutput = "APOP mrose c4c9334bac560ecc979e58001b3e22fb\r\n";
			string output = popClientCommands.ToString();

			// The correct APOP command should have been sent
			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-6
		/// </summary>
		[Test]
		public void TestGetMessageCount()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string statCommandResponse = "+OK 5 10"; // 5 Messages with total size of 10 octets
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + statCommandResponse + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			int numberOfMessages = client.GetMessageCount();

			// We expected 5 messages
			Assert.AreEqual(5, numberOfMessages);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-8
		/// </summary>
		[Test]
		public void TestDeleteMessage()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string DeleteResponse = "+OK"; // Message was deleted
			const string QuitAccepted = "+OK";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + DeleteResponse + "\r\n" + QuitAccepted + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			client.DeleteMessage(5);

			const string expectedOutput = "DELE 5";
			string output = getLastCommand(popClientCommands);

			// We expected that the last command is the delete command
			Assert.AreEqual(expectedOutput, output);

			client.Disconnect();

			const string expectedOutputAfterQuit = "QUIT";
			string outputAfterQuit = getLastCommand(popClientCommands);

			// We now expect that the client has sent the QUIT command
			Assert.AreEqual(expectedOutputAfterQuit, outputAfterQuit);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-8
		/// </summary>
		[Test]
		public void TestDeleteAllMessages()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageCountResponse = "+OK 2 5"; // 2 messages with total size of 5 octets
			const string DeleteResponse = "+OK"; // Message was deleted
			const string QuitAccepted = "+OK";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageCountResponse  + "\r\n" + DeleteResponse + "\r\n" + DeleteResponse + "\r\n" + QuitAccepted + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			// Delete all the messages
			client.DeleteAllMessages();

			// Check that message 1 and message 2 was deleted
			string[] commandsFired = getCommands(popClientCommands);

			bool message1Deleted = false;
			bool message2Deleted = false;
			foreach (string commandFired in commandsFired)
			{
				if (commandFired.Equals("DELE 1"))
					message1Deleted = true;

				if (commandFired.Equals("DELE 2"))
					message2Deleted = true;
			}

			// We expect message 1 to be deleted
			Assert.IsTrue(message1Deleted);

			// We expect message 2 to be deleted
			Assert.IsTrue(message2Deleted);

			// Quit and commit
			client.Disconnect();

			const string expectedOutputAfterQuit = "QUIT";
			string outputAfterQuit = getLastCommand(popClientCommands);

			// We now expect that the client has sent the QUIT command
			Assert.AreEqual(expectedOutputAfterQuit, outputAfterQuit);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-5
		/// </summary>
		[Test]
		public void TestQuit()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string quitOK = "+OK";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + quitOK + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			client.QUIT();

			// Get the last command issued by the client
			string output = getLastCommand(popClientCommands);

			// We expect it to be QUIT
			const string expectedOutput = "QUIT";

			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-9
		/// </summary>
		[Test]
		public void TestNoOperation()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string noopOK = "+OK";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + noopOK + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			client.NOOP();

			// Get the last command issued by the client
			string output = getLastCommand(popClientCommands);

			// We expect it to be NOOP
			const string expectedOutput = "NOOP";

			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-9
		/// </summary>
		[Test]
		public void TestReset()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string rsetOK = "+OK";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + rsetOK + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			client.RSET();

			// Get the last command issued by the client
			string output = getLastCommand(popClientCommands);

			// We expect it to be RSET
			const string expectedOutput = "RSET";

			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-12
		/// </summary>
		[Test]
		public void TestGetMessageUID()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageUidResponse = "+OK 2 psycho"; // Message 2 has UID psycho
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageUidResponse + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringBuilder popClientCommands = new StringBuilder();
			StringWriter writer = new StringWriter(popClientCommands);

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			const string expectedOutput = "psycho";

			// Delete all the messages
			string output = client.GetMessageUID(2);

			// We now expect that the client has given us the correct UID
			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-12
		/// </summary>
		[Test]
		public void TestGetMessageUIDs()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageUidAccepted = "+OK";
			const string messageUid1 = "1 psycho"; // Message 1 has UID psycho
			const string messageUid2 = "2 lord"; // Message 2 has UID lord
			const string uidListEnded = ".";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageUidAccepted + "\r\n" + messageUid1 + "\r\n" + messageUid2 + "\r\n" + uidListEnded + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			// Get the UIDs for all the messages in sorted order from 1 and upwards
			System.Collections.Generic.List<string> uids = client.GetMessageUIDs();

			// The list should have size 2
			Assert.AreEqual(2, uids.Count);

			// The first entry should have uid psycho
			Assert.AreEqual("psycho", uids[0]);

			// The second entry should have uid lord
			Assert.AreEqual("lord", uids[1]);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-7
		/// </summary>
		[Test]
		public void TestGetMessageSize()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageSize = "+OK 9 200"; // Message 9 has size 200 octets
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageSize + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			// Message 9 should have size 200
			const int expectedOutput = 200;
			int output = client.GetMessageSize(9);

			Assert.AreEqual(expectedOutput, output);
		}

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-7
		/// </summary>
		[Test]
		public void TestGetMessageSizes()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageListAccepted = "+OK 2 messages (320 octets)";
			const string messageSize1 = "1 120";
			const string messageSize2 = "2 200";
			const string messageListEnd = ".";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageListAccepted + "\r\n" + messageSize1 + "\r\n" + messageSize2 + "\r\n" + messageListEnd + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			// Message 9 should have size 200
			System.Collections.Generic.List<int> messageSizes = client.GetMessageSizes();

			// The list should have size 2
			Assert.AreEqual(2, messageSizes.Count);

			// The first entry should have size 120
			Assert.AreEqual(120, messageSizes[0]);

			// The second entry should have size 200
			Assert.AreEqual(200, messageSizes[1]);
		}

		// TODO Implement a test for GetMessage(int) here

		/// <summary>
		/// http://tools.ietf.org/html/rfc1939#page-11
		/// </summary>
		[Test]
		public void TestGetMessageHeaders()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string messageTopAccepted = "+OK";
			const string messageHeaders = "Subject: [Blinded by the lights] New Comment On: Comparison of .Net libraries for fetching emails via POP3\r\n"; // \r\n is for ending headers
			const string messageListingEnd = ".";
			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + messageTopAccepted + "\r\n" + messageHeaders + "\r\n" + messageListingEnd + "\r\n";
			StringReader reader = new StringReader(serverResponses);

			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("test", "test");

			// Fetch the header of message 7
			MessageHeader header = client.GetMessageHeaders(7);

			const string expectedSubject = "[Blinded by the lights] New Comment On: Comparison of .Net libraries for fetching emails via POP3";
			string subject = header.Subject;

			Assert.AreEqual(expectedSubject, subject);
		}

		/// <summary>
		/// Helper method to get the last line from a <see cref="StringBuilder"/>
		/// which is the last line that the client has sent.
		/// </summary>
		/// <param name="builder">The builder to get the last line from</param>
		/// <returns>A single line, which is the last one in the builder</returns>
		private static string getLastCommand(StringBuilder builder)
		{
			string[] commands = getCommands(builder);
			return commands[commands.Length - 1];
		}

		/// <summary>
		/// Helper method to get a string array of the commands issued by a client.
		/// </summary>
		/// <param name="builder">The builder to get the commands from</param>
		/// <returns>A string array where each entry is a command</returns>
		private static string[] getCommands(StringBuilder builder)
		{
			return builder.ToString().Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}