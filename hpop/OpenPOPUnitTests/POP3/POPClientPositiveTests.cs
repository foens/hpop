using System.IO;
using System.Text;
using NUnit.Framework;
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
	}
}
