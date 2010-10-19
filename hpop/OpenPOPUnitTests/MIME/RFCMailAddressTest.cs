using System.Net.Mail;
using NUnit.Framework;
using OpenPOP.MIME;
using System.Collections.Generic;

namespace OpenPOPUnitTests.MIME
{
	[TestFixture]
	public class RFCMailAddressTest
	{
		/// <summary>
		/// Test that we can parse a fully qualified mail address
		/// </summary>
		[Test]
		public void ParsingMailAddressFull()
		{
			const string address = "\"John McDaniel\" <jmcdaniel@spam.teltronics.com>";
			const string expectedAddress = address;
			const string expectedMailAddress = "jmcdaniel@spam.teltronics.com";
			const string expectedMailName = "John McDaniel";
			RFCMailAddress rfcAddress = RFCMailAddress.ParseMailAddress( address );
			Assert.AreEqual( expectedAddress, rfcAddress.Address, "Full Name" );
			Assert.AreEqual( expectedMailAddress, rfcAddress.MailAddress.Address, "MailAddress" );
			Assert.AreEqual( expectedMailName, rfcAddress.MailAddress.DisplayName, "MailAddress Display Name" );
		}

		/// <summary>
		/// Test that we can parse an address which only contains a display name
		/// </summary>
		[Test]
		public void ParsingMailAddressNameOnly()
		{
			const string address = "Ralph Wiggins";
			const string expectedAddress = address;
			RFCMailAddress rfcAddress = RFCMailAddress.ParseMailAddress( address );
			Assert.AreEqual( expectedAddress, rfcAddress.Address, "Display Name" );
			Assert.IsFalse( rfcAddress.HasValidMailAddress, "HasValidMailAddress" );
			Assert.IsNull( rfcAddress.MailAddress, "MailAddress" );
		}

		/// <summary>
		/// Test that we can parse an email address which is not surrounded by brackets
		/// </summary>
		[Test]
		public void ParsingMailAddressNoBrackets()
		{
			const string address = "snoopy@peanuts.com";
			const string expectedAddress = address;
			RFCMailAddress rfcAddress = RFCMailAddress.ParseMailAddress( address );
			Assert.IsTrue( rfcAddress.HasValidMailAddress, "HasValidMailAddress" );
			Assert.AreEqual( expectedAddress, rfcAddress.Address, "Display Name" );
			Assert.AreEqual( expectedAddress, rfcAddress.MailAddress.Address, "MailAddress" );
			Assert.IsEmpty( rfcAddress.MailAddress.DisplayName, "MailAddress Display Name" );
		}

		/// <summary>
		/// Test that we can parse a list of email addresses separated by commas
		/// </summary>
		[Test]
		public void ParsingMailAddressList()
		{
			const string addressList = "\"John McDaniel\" <jmcdaniel@nospam.teltronics.com>, snoopy@peanuts.com, <bob@builder.org>";
			string[] expectedAddress = { "jmcdaniel@nospam.teltronics.com", "snoopy@peanuts.com", "bob@builder.org" };
			string[] expectedDisplay = { "\"John McDaniel\" <jmcdaniel@nospam.teltronics.com>", "snoopy@peanuts.com", "bob@builder.org" };
			List<RFCMailAddress> list = RFCMailAddress.ParseMailAddresses( addressList );
			Assert.AreEqual( expectedAddress.Length, list.Count, "Number of items parsed" );
			for( int i = 0; i < list.Count; i++ )
			{
				Assert.IsTrue( list[i].HasValidMailAddress, string.Format( "HasValidMailAddress: {0}", i ) );
				Assert.AreEqual( expectedAddress[i], list[i].MailAddress.Address, string.Format( "Email Address: {0}", i ) );
				Assert.AreEqual( expectedDisplay[i], list[i].Address, string.Format( "Email Display: {0}", i ) );
			}
		}

		/// <summary>
		/// Test that we can implicitly convert from a RFCMailAddress to a Net.Mail.MailAddress object
		/// </summary>
		[Test]
		public void ImplicitMailAddressConversion()
		{
			const string address = "\"John McDaniel\" <jmcdaniel@spam.teltronics.com>";
			RFCMailAddress rfcAddress = RFCMailAddress.ParseMailAddress( address );
			MailAddress mailAddress = rfcAddress;
			Assert.AreSame( rfcAddress.MailAddress, mailAddress);
		}

	}
}
