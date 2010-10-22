using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenPOP.MIME;
using System.Collections.Generic;
using OpenPOP.POP3;

namespace OpenPOPUnitTests
{
	/// <summary>
	/// This class is to be used for creating test cases for issues which have not yet been fixed
	/// and for which there is no formal Test class.
	/// </summary>
	[TestFixture]
	public class UnfixedIssuesTests
	{
		/// <summary>
		/// Test that we can parse an email address that contains a comma in the display name
		/// </summary>
		[Test]
		public void ParsingMailAddressListDisplayNameHasComma()
		{
			const string address = "\"McDaniel, John\" <jmcdaniel@spam.teltronics.com>";
			const string expectedAddress = address;
			const string expectedMailAddress = "jmcdaniel@spam.teltronics.com";
			const string expectedMailName = "McDaniel, John";
			List<RFCMailAddress> list = RFCMailAddress.ParseMailAddresses( address );
			Assert.AreEqual( 1, list.Count, "Number of items parsed" );
			Assert.AreEqual( expectedAddress, list[0].Address, "Full Name" );
			Assert.AreEqual( expectedMailAddress, list[0].MailAddress.Address, "MailAddress" );
			Assert.AreEqual( expectedMailName, list[0].MailAddress.DisplayName, "MailAddress Display Name" );
		}

		/// <summary>
		/// Tests a ISO 88591 email which has special characters in the body
		/// TODO: Remember to uncomment address checks
		/// </summary>
		[Test]
		public void TestGetMessageIso88591()
		{
			const string welcomeMessage = "+OK";
			const string okUsername = "+OK";
			const string okPassword = "+OK";
			const string okMessageFetch = "+OK";
			const string messageHeaders = "Return-Path: <thefeds@spam.mail.dk>\r\nReceived: from fep28 ([80.160.76.232]) by fep30.mail.dk\r\n          (InterMail vM.7.09.02.02 201-2219-117-103-20090326) with ESMTP\r\n          id <20101017101437.WEXY2819.fep30.mail.dk@fep28>\r\n          for <thefeds@spam.mail.dk>; Sun, 17 Oct 2010 12:14:37 +0200\r\nReceived: from [195.41.46.142] ([195.41.46.142:41886] helo=fep41.mail.dk)\r\n\tby fep28 (envelope-from <thefeds@spam.mail.dk>)\r\n\t(ecelerity 2.2.2.45 r()) with ESMTP\r\n\tid 88/D0-14647-D8CCABC4; Sun, 17 Oct 2010 12:14:37 +0200\r\nReceived: from [87.48.47.215] ([87.48.47.215:49596] helo=[192.168.0.234])\r\n\tby fep41.mail.dk (envelope-from <thefeds@spam.mail.dk>)\r\n\t(ecelerity 2.2.2.45 r()) with ESMTPA\r\n\tid 7F/2E-18479-B8CCABC4; Sun, 17 Oct 2010 12:14:35 +0200\r\nMessage-ID: <4CBACC87.8080600@spam.mail.dk>\r\nDate: Sun, 17 Oct 2010 12:14:31 +0200\r\nFrom: =?ISO-8859-1?Q?Kasper_F=F8ns?= <thefeds@spam.mail.dk>\r\nUser-Agent: Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.9) Gecko/20100915 Thunderbird/3.1.4\r\nMIME-Version: 1.0\r\nTo: =?ISO-8859-1?Q?Kasper_F=F8ns?= <thefeds@spam.mail.dk>\r\nSubject: Test =?ISO-8859-1?Q?=E6=F8=E5=C6=D8=C5?=\r\nContent-Type: text/plain; charset=ISO-8859-1; format=flowed\r\nContent-Transfer-Encoding: 8bit";
			const string messageHeaderToBodyDelimiter = "";
			const string messageBody = "This is a test message. It contains some ISO-8859-1 characters like:\r\nÆØÅæøå\r\nWhich is Danish characters\r\n\r\nRegards\r\nKasper Føns\r\n";
			const string messageEnd = ".";

			const string serverResponses = welcomeMessage + "\r\n" + okUsername + "\r\n" + okPassword + "\r\n" + okMessageFetch + "\r\n" + messageHeaders + "\r\n" + messageHeaderToBodyDelimiter + "\r\n" + messageBody + "\r\n" + messageEnd + "\r\n";

			// We want to use a normal stream instead of a StringReader.
			// Therefore we convert our message and responses into a byte array.
			// We can use the encoding ISO-8859-1 on all the strings since
			// all the parts that are compaitible with ASCII can be decoded using ASCII
			// But the message body's special characters will be encoding using ISO-8859-1, which
			// is the encoding mentioned in the content-type charset property.
			byte[] serverResponsesInBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(serverResponses);

			StreamReader reader = new StreamReader(new MemoryStream(serverResponsesInBytes));
			StringWriter writer = new StringWriter(new StringBuilder());

			POPClient client = new POPClient();
			client.Connect(reader, writer);
			client.Authenticate("user", "password");

			Message message = client.GetMessage(132);

			Assert.NotNull(message);
			Assert.NotNull(message.Headers);

			Assert.AreEqual("Test æøåÆØÅ", message.Headers.Subject);
			Assert.AreEqual("4CBACC87.8080600@spam.mail.dk", message.Headers.MessageID);

			Assert.AreEqual("1.0", message.Headers.MimeVersion);

			Assert.NotNull(message.Headers.ContentType);
			Assert.NotNull(message.Headers.ContentType.CharSet);
			Assert.AreEqual("ISO-8859-1", message.Headers.ContentType.CharSet);
			Assert.NotNull(message.Headers.ContentType.MediaType);
			Assert.AreEqual("text/plain", message.Headers.ContentType.MediaType);

			// The Date header was:
			// Sun, 17 Oct 2010 12:14:31 +0200
			// The +0200 is the same as substracting 2 hours in UTC
			Assert.AreEqual(new DateTime(2010, 10, 17, 10, 14, 31, DateTimeKind.Utc), message.Headers.DateSent);
			Assert.AreEqual("Sun, 17 Oct 2010 12:14:31 +0200", message.Headers.Date);

			Assert.NotNull(message.Headers.From);
			Assert.AreEqual("thefeds@spam.mail.dk", message.Headers.From.Address);
			Assert.AreEqual("Kasper Føns", message.Headers.From.DisplayName);

			// There should only be one receiver
			Assert.NotNull(message.Headers.To);
			Assert.AreEqual(1, message.Headers.To.Count);
			Assert.AreEqual("thefeds@spam.mail.dk", message.Headers.To[0].Address);
			Assert.AreEqual("Kasper Føns", message.Headers.To[0].DisplayName);

			Assert.NotNull(message.Headers.ReturnPath);
			Assert.AreEqual("thefeds@spam.mail.dk", message.Headers.ReturnPath.Address);
			Assert.IsEmpty(message.Headers.ReturnPath.DisplayName);

			// There should only be one body
			Assert.NotNull(message.MessageBody);
			Assert.AreEqual(1, message.MessageBody.Count);
			Assert.AreEqual(messageBody, message.MessageBody[0].Body);
			Assert.AreEqual("text/plain", message.MessageBody[0].Type);
		}
	}
}
