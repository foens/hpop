using System;
using System.Collections.Generic;
using System.Net.Mail;
using OpenPOP.Shared.Logging;
using OpenPOP.MIME.Decode;

namespace OpenPOP.MIME
{
	/// <summary>
	/// This class is used for RFC compiant email addresses
	/// </summary>
	/// <remarks>
	/// The <seealso cref="System.Net.Mail.MailAddress"/> does not cover all the possible formats 
	/// for <a href="http://tools.ietf.org/html/rfc5322#section-3.4">RFC5322</a> compliant email addresses.
	/// This class is used as an address wrapper to account for that deficiency.
	/// </remarks>
	public class RFCMailAddress
	{
		#region Properties

		/// <summary>
		/// The textual representation of the mail address
		/// </summary>
		public string Address { get; private set; }

		/// <summary>
		/// The <see cref="System.Net.Mail.MailAddress"/> associated with the <see cref="RFCMailAddress"/>. 
		/// </summary>
		/// <remarks>
		/// The value of this property can be <see lanword="null"/> in instances where the <see cref="System.Net.Mail.MailAddress"/> cannot represent the address properly.
		/// </remarks>
		public MailAddress MailAddress { get; private set; }

		/// <summary>
		/// Specifies if the object contains a valid <see cref="System.Net.Mail.MailAddress"/> reference.
		/// </summary>
		public bool HasValidMailAddress
		{
			get { return MailAddress != null; }
		}

		#endregion

		/// <summary>
		/// Constructs an <see cref="RFCMailAddress"/> object from a <see cref="System.Net.Mail.MailAddress"/> object
		/// </summary>
		/// <param name="address">The address to use</param>
		private RFCMailAddress(MailAddress address)
		{
			MailAddress = address;
			Address = address.ToString( );
		}

		/// <summary>
		/// Constructs an <see cref="RFCMailAddress"/> object from a <see cref="String"/>
		/// </summary>
		/// <param name="address">The address to use</param>
		private RFCMailAddress( string address )
		{
			Address = address;
		}

		/// <summary>
		/// Parse email address from a MIME header
		/// http://tools.ietf.org/html/rfc5322#section-3.4
		/// 
		/// Examples of input:
		/// Eksperten mailrobot &lt;noreply@mail.eksperten.dk&gt;
		/// "Eksperten mailrobot" &lt;noreply@mail.eksperten.dk&gt;
		/// &lt;noreply@mail.eksperten.dk&gt;
		/// noreply@mail.eksperten.dk
		/// Some name (will return <see langword="null"/> on this)
		/// 
		/// 
		/// It might also contain encoded text.
		/// <see cref="EncodedWord.Decode">For more information about encoded text</see>
		/// </summary>
		/// <param name="input">The value to parse out and email and/or a username</param>
		/// <returns>A valid <see cref="MailAddress"/> where the input has been parsed into or <see langword="null"/> if the input is not valid</returns>
		/// <exception cref="ArgumentNullException">Thrown if a <see langword="null"/> reference is passed via the <paramref name="input"/> parameter.</exception>
		public static RFCMailAddress ParseMailAddress(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			// Decode the value, if it was encoded
			input = EncodedWord.Decode(input.Trim());

			// Find the location of the email address
			int indexStartEmail = input.LastIndexOf("<");
			int indexEndEmail = input.LastIndexOf(">");

			try
			{
				if (indexStartEmail >= 0 && indexEndEmail >= 0)
				{
					string username;
					// Check if there is a username in front of the email address
					if (indexStartEmail > 0)
					{
						// Parse out the user
						username = input.Substring(0, indexStartEmail).Trim();
					}
					else
					{
						// There was no user
						username = string.Empty;
					}

					// Parse out the email address without the "<"  and ">"
					indexStartEmail = indexStartEmail + 1;
					int emailLength = indexEndEmail - indexStartEmail;
					string emailAddress = input.Substring(indexStartEmail, emailLength).Trim();

					// There has been cases where there was no emailaddress between the < and >
					if (!string.IsNullOrEmpty(emailAddress))
					{
						// If the username is quoted, MailAddress' constructor will remove them for us
						return new RFCMailAddress( new MailAddress(emailAddress, username) );
					}
				}

				// This might be on the form noreply@mail.eksperten.dk
				// Sometimes invalid emails are sent, like sqlmap-user@sourceforge.net. (last period is illigal)
				// if the MailAddress will take it so will we, otherwise it gets handled below
				return new RFCMailAddress( new MailAddress( input ) );
			}
			catch (FormatException)
			{
				DefaultLogger.CreateLogger().LogError("ParseMailAddress(): Improper mail address: " + input);
			}

			// It could be that the format used was simply a name
			// which is indeed valid according to the RFC
			// Example:
			// Eksperten mailrobot
			return new RFCMailAddress(input);
		}

		/// <summary>
		/// Parses input of the form
		/// Eksperten mailrobot &lt;noreply@mail.eksperten.dk&gt;, ...
		/// to a list of RFCMailAddresses
		/// </summary>
		/// <param name="input">The input that is a comma-separated list of EmailAddresses to parse</param>
		/// <returns>A List of <seealso cref="RFCMailAddress"/> objects extracted from the <paramref name="input"/> parameter.</returns>
		// TODO: This method does not handle display names that contain commas. "McDaniel, John" <john@mcdaniel.me>, "next" <next@next.com>
		public static List<RFCMailAddress> ParseMailAddresses(string input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			List<RFCMailAddress> returner = new List<RFCMailAddress>();

			// MailAddresses are split by commas
			string[] mailAddresses = input.Split(',');

			// Parse each of these
			foreach (string mailAddress in mailAddresses)
			{
				returner.Add(ParseMailAddress(mailAddress));
			}

			return returner;
		}

		/// <summary>
		/// Operator allows for implicit conversion from <see cref="RFCMailAddress"/> to <see cref="System.Net.Mail.MailAddress"/>
		/// </summary>
		/// <param name="address">The <see cref="RFCMailAddress"/> to convert.</param>
		/// <returns>The internal reference to the <see cref="System.Net.Mail.MailAddress"/> object from the <paramref name="address"/> object.</returns>
		public static implicit operator MailAddress( RFCMailAddress address )
		{
			return address.MailAddress;
		}

		/// <summary>
		/// The <see cref="Address"/> string associated with the object
		/// </summary>
		/// <returns>Returns the string representation for the object</returns>
		public override string ToString()
		{
			return Address;
		}

	}
}
