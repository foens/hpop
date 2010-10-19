using NUnit.Framework;
using OpenPOP.MIME;
using System.Collections.Generic;

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

	}
}
