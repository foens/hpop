using System;
using NUnit.Framework;
using OpenPop.Mime.Header;

namespace OpenPopUnitTests.Mime.Header
{
	public class ReceivedTests
	{
		[Test]
		public void TestDate()
		{
			const string input = "; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			// -0600 we need to add 6 hours when in UTC
			DateTime expectedOutput = new DateTime(1997, 11, 21, 15, 55, 06, DateTimeKind.Utc);
			DateTime output = received.Date;

			Assert.AreEqual(expectedOutput, output);

			Assert.AreEqual(input, received.Raw);
		}

		[Test]
		public void TestDate2()
		{
			const string input = "; Tue, 1 Jul 2003 10:52:37 +0200";
			Received received = new Received(input);

			// +0200 we need to substract 2 hours when in UTC
			DateTime expectedOutput = new DateTime(2003, 7, 1, 8, 52, 37, DateTimeKind.Utc);
			DateTime output = received.Date;

			Assert.AreEqual(expectedOutput, output);

			Assert.AreEqual(input, received.Raw);
		}

		[Test]
		public void TestNull()
		{
			Assert.Throws<ArgumentNullException>(delegate { new Received(null); });
		}

		[Test]
		public void TestFrom()
		{
			const string input = "from foobar; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("foobar", received.Names["from"]);
		}

		[Test]
		public void TestFrom2()
		{
			const string input = "from testing.mail.com; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("testing.mail.com", received.Names["from"]);
		}

		[Test]
		public void TestBy()
		{
			const string input = "by fep32.mail.com; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("fep32.mail.com", received.Names["by"]);
		}

		[Test]
		public void TestBy2()
		{
			const string input = "by some.example.openpop.net; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("some.example.openpop.net", received.Names["by"]);
		}

		[Test]
		public void TestFromBy()
		{
			const string input = "from foo by bar; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("foo", received.Names["from"]);
			Assert.AreEqual("bar", received.Names["by"]);
		}

		[Test]
		public void TestFromBy2()
		{
			const string input = "from bar by foo; Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("bar", received.Names["from"]);
			Assert.AreEqual("foo", received.Names["by"]);
		}

		[Test]
		public void TestFromWithExtraInfo()
		{
			const string input = "from testing.mail.com ([216.34.181.88:10057] helo=lists.sourceforge.net); Fri, 21 Nov 1997 09:55:06 -0600";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("testing.mail.com ([216.34.181.88:10057] helo=lists.sourceforge.net)", received.Names["from"]);
		}

		[Test]
		public void TestFromWithExtraInfoWithSemicolon()
		{
			// This string is properly illegal
			const string input = "from foo (;bar); Fri, 21 Nov 1997 09:55:06 +0000";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("foo (;bar)", received.Names["from"]);
			Assert.AreEqual(new DateTime(1997, 11, 21, 9, 55, 06, DateTimeKind.Utc), received.Date);
		}

		[Test]
		public void TestFromWithExtraInfoWithSemicolonAndExtraDate()
		{
			// This string is properly illegal
			const string input = "from foo (;Tue, 1 Jul 2003 10:52:37 +0200); Fri, 21 Nov 1997 09:55:06 +0000";
			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("foo (;Tue, 1 Jul 2003 10:52:37 +0200)", received.Names["from"]);
			Assert.AreEqual(new DateTime(1997, 11, 21, 9, 55, 06, DateTimeKind.Utc), received.Date);
		}

		[Test]
		public void TestFullReceivedLine()
		{
			const string input =
				"from sog-mx-2.v43.ch3.sourceforge.com ([172.29.43.192] helo=mx.sourceforge.net) " +
				"by sfs-ml-3.v29.ch3.sourceforge.com " +
				"with esmtp (Exim 4.76) (envelope-from <thefeds@mail.dk>) " +
				"id 1Qcvg8-0004Kr-17 " +
				"for hpop-users@lists.sourceforge.net" +
				"; Sat, 02 Jul 2011 08:35:52 +0000";

			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("sog-mx-2.v43.ch3.sourceforge.com ([172.29.43.192] helo=mx.sourceforge.net)", received.Names["from"]);
			Assert.AreEqual("sfs-ml-3.v29.ch3.sourceforge.com", received.Names["by"]);
			Assert.AreEqual("esmtp (Exim 4.76) (envelope-from <thefeds@mail.dk>)", received.Names["with"]);
			Assert.AreEqual("1Qcvg8-0004Kr-17", received.Names["id"]);
			Assert.AreEqual("hpop-users@lists.sourceforge.net", received.Names["for"]);
			Assert.AreEqual(new DateTime(2011, 7, 2, 8, 35, 52, DateTimeKind.Utc), received.Date);
			Assert.AreEqual(input, received.Raw);
		}

		[Test]
		public void TestFullReceivedLine2()
		{
			const string input =
				"from smtp.nfit.au.dk ([10.19.9.11]) " +
				"by mbe1i (Cyrus v2.3.16-Invoca-RPM-2.3.16-3) " +
				"with LMTPA;" +
				"Tue, 05 Jul 2011 11:58:11 +0200";

			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("smtp.nfit.au.dk ([10.19.9.11])", received.Names["from"]);
			Assert.AreEqual("mbe1i (Cyrus v2.3.16-Invoca-RPM-2.3.16-3)", received.Names["by"]);
			Assert.AreEqual("LMTPA", received.Names["with"]);
			Assert.AreEqual(new DateTime(2011, 7, 5, 9, 58, 11, DateTimeKind.Utc), received.Date);
			Assert.AreEqual(input, received.Raw);
		}

		[Test]
		public void TestFullReceivedLine3()
		{
			const string input =
				"from ymir.adm.au.dk ([10.60.1.18]) " +
				"by ns2.au.dk (8.13.7+Sun/8.12.5) " +
				"with ESMTP " +
				"id p659boKa018808; " +
				"Tue, 5 Jul 2011 11:38:04 +0200 (MEST)";

			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("ymir.adm.au.dk ([10.60.1.18])", received.Names["from"]);
			Assert.AreEqual("ns2.au.dk (8.13.7+Sun/8.12.5)", received.Names["by"]);
			Assert.AreEqual("ESMTP", received.Names["with"]);
			Assert.AreEqual("p659boKa018808", received.Names["id"]);
			Assert.AreEqual(new DateTime(2011, 7, 5, 9, 38, 04, DateTimeKind.Utc), received.Date);
			Assert.AreEqual(input, received.Raw);
		}

		[Test]
		public void TestFullReceivedLine4()
		{
			const string input =
				"from fep29 ([80.160.76.233]) " +
				"by fep34.mail.dk (InterMail vM.8.01.04.07 201-2260-137-119-20110503) " +
				"with ESMTP " +
				"id <20110530134858.YAHN18594.fep34.mail.dk@fep29> " +
				"for <thefeds@mail.dk>; " + 
				"Mon, 30 May 2011 15:48:58 +0200";

			Received received = new Received(input);

			Assert.AreEqual(input, received.Raw);
			Assert.AreEqual("fep29 ([80.160.76.233])", received.Names["from"]);
			Assert.AreEqual("fep34.mail.dk (InterMail vM.8.01.04.07 201-2260-137-119-20110503)", received.Names["by"]);
			Assert.AreEqual("ESMTP", received.Names["with"]);
			Assert.AreEqual("<20110530134858.YAHN18594.fep34.mail.dk@fep29>", received.Names["id"]);
			Assert.AreEqual("<thefeds@mail.dk>", received.Names["for"]);
			Assert.AreEqual(new DateTime(2011, 5, 30, 13, 48, 58, DateTimeKind.Utc), received.Date);
			Assert.AreEqual(input, received.Raw);
		}
	}
}