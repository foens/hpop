using NUnit.Framework;
using OpenPop.Mime.Decode;

namespace OpenPopUnitTests.Mime.Decode
{
	[TestFixture]
	public class EncodedWordTests
	{
		[Test]
		public void TestEncodedWordOnSubjectWithSpaces()
		{
			const string subject = 
				"=?iso-8859-1?Q?Re: Yahoo! Mail (comment acc=E9der, restaurer, etc)?=";

			string result = EncodedWord.Decode(subject);

			Assert.AreEqual("Re: Yahoo! Mail (comment accéder, restaurer, etc)", result);
		}

		[Test]
		public void TestEncodedWordOnNonEncodedString()
		{
			const string notEncodedString =
				"test that this is not touched ?! ?=..?= =?...?=";

			string result = EncodedWord.Decode(notEncodedString);

			Assert.AreEqual(notEncodedString, result);
		}

		[Test]
		public void TestEncodedWordDanish()
		{
			const string subject = "=?iso-8859-1?Q?SV:_Ticket(13349550)_-_Sp=F8rgsm=E5l_omkring_CBB_privat?=";

			string result = EncodedWord.Decode(subject);

			Assert.AreEqual("SV: Ticket(13349550) - Spørgsmål omkring CBB privat", result);
		}

		[Test]
		public void TestEncodedWordLithuanian()
		{
			const string input = "=?ISO-8859-13?Q?Fwd=3A_Dvira=E8iai_vasar=E0_vagiami_da=FEniau=2C_bet_draust?=";

			string result = EncodedWord.Decode(input);
			Assert.AreEqual("Fwd: Dviračiai vasarą vagiami dažniau, bet draust", result);
		}
	}
}