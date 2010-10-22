using System;
using System.Collections.Generic;
using System.Net.Mail;
using OpenPOP.Shared.Logging;
using OpenPOP.MIME.Decode;

namespace OpenPOP.MIME
{
	/// <summary>
	/// This class is used for RFC compliant email addresses
	/// </summary>
	/// <remarks>
	/// The <seealso cref="MailAddress"/> does not cover all the possible formats 
	/// for <a href="http://tools.ietf.org/html/rfc5322#section-3.4">RFC5322</a> compliant email addresses.
	/// This class is used as an address wrapper to account for that deficiency.
	/// </remarks>
	public class RFCMailAddress
	{
		#region Properties
		///<summary>
		/// The email address of this <see cref="RFCMailAddress"/>
		/// It is possibly string.Empty since RFC mail addresses
		/// does not require an email address specified.
		///</summary>
		///<example>
		/// Example header with email address:
		/// To: Test test@mail.com
		/// Address will be test@mail.com
		///</example>
		///<example>
		/// Example header without email address:
		/// To: Test
		/// Address will be string.Empty
		///</example>
		public string Address { get; private set; }

		///<summary>
		/// The display name of this <see cref="RFCMailAddress"/>
		/// It is possibly string.Empty since RFC mail addresses
		/// does not require a display name to be specified.
		///</summary>
		///<example>
		/// Example header with display name:
		/// To: Test test@mail.com
		/// DisplayName will be Test
		///</example>
		///<example>
		/// Example header without display name:
		/// To: test@test.com
		/// DisplayName will be string.Empty
		///</example>
		public string DisplayName { get; private set; }

		public string Raw { get; set; }

		/// <summary>
		/// The <see cref="MailAddress"/> associated with the <see cref="RFCMailAddress"/>. 
		/// </summary>
		/// <remarks>
		/// The value of this property can be <see lanword="null"/> in instances where the <see cref="MailAddress"/> cannot represent the address properly.
		/// </remarks>
		public MailAddress MailAddress { get; private set; }

		/// <summary>
		/// Specifies if the object contains a valid <see cref="MailAddress"/> reference.
		/// </summary>
		public bool HasValidMailAddress
		{
			get { return MailAddress != null; }
		}
		#endregion

		# region Constructors
		/// <summary>
		/// Constructs an <see cref="RFCMailAddress"/> object from a <see cref="MailAddress"/> object.
		/// This constructor is used when we were able to construct a <see cref="MailAddress"/> from a string.
		/// </summary>
		/// <param name="mailAddress">The address that <paramref name="raw"/> was parsed into</param>
		/// <param name="raw">The raw unparsed input which was parsed into the <paramref name="mailAddress"/></param>
		/// <exception cref="ArgumentNullException">If <paramref name="mailAddress"/> or <paramref name="raw"/> is <see langword="null"/></exception>
		private RFCMailAddress(MailAddress mailAddress, string raw)
		{
			if (mailAddress == null)
				throw new ArgumentNullException("mailAddress");

			if(raw == null)
				throw new ArgumentNullException("raw");

			MailAddress = mailAddress;
			Address = mailAddress.Address;
			DisplayName = mailAddress.DisplayName;
			Raw = raw;
		}

		/// <summary>
		/// When we were unable to parse a string into a <see cref="MailAddress"/>, this constructor can be
		/// used. The Raw string is then used as the <see cref="DisplayName"/>.
		/// </summary>
		/// <param name="raw">The raw unparsed input which could not be parsed</param>
		/// <exception cref="ArgumentNullException">If <paramref name="raw"/> is <see langword="null"/></exception>
		private RFCMailAddress(string raw)
		{
			if(raw == null)
				throw new ArgumentNullException("raw");

			MailAddress = null;
			Address = string.Empty;
			DisplayName = raw;
			Raw = raw;
		}
		#endregion

		#region Parsing
		/// <summary>
		/// Parse email address from a MIME header
		/// http://tools.ietf.org/html/rfc5322#section-3.4
		/// 
		/// Examples of input:
		/// Eksperten mailrobot &lt;noreply@mail.eksperten.dk&gt;
		/// "Eksperten mailrobot" &lt;noreply@mail.eksperten.dk&gt;
		/// &lt;noreply@mail.eksperten.dk&gt;
		/// noreply@mail.eksperten.dk
		/// 
		/// It might also contain encoded text.
		/// <see cref="EncodedWord.Decode">For more information about encoded text</see>
		/// </summary>
		/// <param name="input">The value to parse out and email and/or a username</param>
		/// <returns>A <see cref="RFCMailAddress"/></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
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
						return new RFCMailAddress(new MailAddress(emailAddress, username), input);
					}
				}

				// This might be on the form noreply@mail.eksperten.dk
				// Check if there is an email, if notm there is no need to try
				if(input.Contains("@"))
					return new RFCMailAddress(new MailAddress(input), input);
			}
			catch (FormatException)
			{
				// Sometimes invalid emails are sent, like sqlmap-user@sourceforge.net. (last period is illigal)
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
		/// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
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
		#endregion

		/*
		 * TODO: Discuss with John if this should be kept or deleted
		/// <summary>
		/// Operator allows for implicit conversion from <see cref="RFCMailAddress"/> to <see cref="System.Net.Mail.MailAddress"/>
		/// </summary>
		/// <param name="address">The <see cref="RFCMailAddress"/> to convert.</param>
		/// <returns>The internal reference to the <see cref="System.Net.Mail.MailAddress"/> object from the <paramref name="address"/> object.</returns>
		public static implicit operator MailAddress(RFCMailAddress address)
		{
			return address.MailAddress;
		}
		 */
		
		/// <summary>
		/// The <see cref="Address"/> string associated with the object
		/// </summary>
		/// <returns>Returns the string representation for the object</returns>
		public override string ToString()
		{
			if (HasValidMailAddress)
				return MailAddress.ToString();

			return Raw;
		}

	}
}
