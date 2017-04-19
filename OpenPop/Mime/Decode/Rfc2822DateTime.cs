﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using OpenPop.Common.Logging;

namespace OpenPop.Mime.Decode
{
	/// <summary>
	/// Class used to decode RFC 2822 Date header fields.
	/// </summary>
	public static class Rfc2822DateTime
	{
		//Constants
		private const string REGEX_OLD_TIMEZONE_FORMATS = @"UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|MSK|PDT|[A-I]|[K-Y]|Z"; //Timezone formats that aren't +-hhmm, e.g. UTC, or K. See MatchEvaluator method for conversions
		private const string REGEX_NEW_TIMEZONE_FORMATS = @"[\+-](?<hours>\d\d)(?<minutes>\d\d)"; //Matches any +=hhmm timezone offset, e.g. +0100

		/// <summary>
		/// Custom DateTime formats - will be tried if cannot parse the dateInput string using the default method
		///	 Specified using formats at http://msdn.microsoft.com/en-us/library/8kb3ddd4%28v=vs.110%29.aspx
		///	 One format per string in the array
		/// </summary>
		public static string[] CustomDateTimeFormats { private get; set; }

		/// <summary>
		/// Converts a string in RFC 2822 format into a <see cref="DateTime"/> object
		/// </summary>
		/// <param name="inputDate">The date to convert</param>
		/// <returns>
		/// A valid <see cref="DateTime"/> object, which represents the same time as the string that was converted. 
		/// If <paramref name="inputDate"/> is not a valid date representation, then <see cref="DateTime.MinValue"/> is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException">If <paramref name="inputDate"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If the <paramref name="inputDate"/> could not be parsed into a <see cref="DateTime"/> object</exception>
		public static DateTime StringToDate(string inputDate)
		{
			if(inputDate == null)
				throw new ArgumentNullException("inputDate");

			// Handle very wrong date time format: Tue Feb 18 10:23:30 2014 (MSK)
			inputDate = FixSpecialCases(inputDate);

			// Old date specification allows comments and a lot of whitespace
			inputDate = StripCommentsAndExcessWhitespace(inputDate);

			try
			{
				// Extract the DateTime
				DateTime dateTime = ExtractDateTime(inputDate);

				// Bail if we could not parse the date
				if (dateTime == DateTime.MinValue)
					return dateTime;

				// If a day-name is specified in the inputDate string, check if it fits with the date
				ValidateDayNameIfAny(dateTime, inputDate);

				// Convert the date into UTC
				dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);

				// Adjust according to the time zone
				dateTime = AdjustTimezone(dateTime, inputDate);
				
				// Return the parsed date
				return dateTime;
			}
			catch (FormatException e)	// Convert.ToDateTime() Failure
			{
				throw new ArgumentException("Could not parse date: " + e.Message + ". Input was: \"" + inputDate + "\"", e);
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Could not parse date: " + e.Message + ". Input was: \"" + inputDate + "\"", e);
			}
		}

		/// <summary>
		/// Adjust the <paramref name="dateTime"/> object given according to the timezone specified in the <paramref name="dateInput"/>.
		/// </summary>
		/// <param name="dateTime">The date to alter</param>
		/// <param name="dateInput">The input date, in which the timezone can be found</param>
		/// <returns>An date altered according to the timezone</returns>
		private static DateTime AdjustTimezone(DateTime dateTime, string dateInput)
		{
			// We know that the timezones are always in the last part of the date input
			string[] parts = dateInput.Split(' ');
			string lastPart = parts[parts.Length - 1];

			// Convert timezones in older formats to [+-]dddd format.
			lastPart = Regex.Replace(lastPart, REGEX_OLD_TIMEZONE_FORMATS, MatchEvaluator);

			// Find the timezone specification
			// Example: Fri, 21 Nov 1997 09:55:06 -0600
			// finds -0600
			Match match = Regex.Match(lastPart, REGEX_NEW_TIMEZONE_FORMATS);
			if (match.Success)
			{
				// We have found that the timezone is in +dddd or -dddd format
				// Add the number of hours and minutes to our found date
				int hours = int.Parse(match.Groups["hours"].Value);
				int minutes = int.Parse(match.Groups["minutes"].Value);

				int factor = match.Value[0] == '+' ? -1 : 1;

				dateTime = dateTime.AddHours(factor*hours);
				dateTime = dateTime.AddMinutes(factor*minutes);

				return dateTime;
			}

			DefaultLogger.Log.LogDebug("No timezone found in date: " + dateInput + ". Using -0000 as default.");

			// A timezone of -0000 is the same as doing nothing
			return dateTime;
		}

		/// <summary>
		/// Convert timezones in older formats to [+-]dddd format.
		/// </summary>
		/// <param name="match">The match that was found</param>
		/// <returns>The string to replace the matched string with</returns>
		private static string MatchEvaluator(Match match)
		{
			if (!match.Success)
			{
				throw new ArgumentException("Match success are always true");
			}

			switch (match.Value)
			{
				// "A" through "I"
				// are equivalent to "+0100" through "+0900" respectively
				case "A": return "+0100";
				case "B": return "+0200";
				case "C": return "+0300";
				case "MSK": // EU timezone MSK is semantically equivalent to +0400
				case "D": return "+0400";
				case "E": return "+0500";
				case "F": return "+0600";
				case "G": return "+0700";
				case "H": return "+0800";
				case "I": return "+0900";

				// "K", "L", and "M"
				// are equivalent to "+1000", "+1100", and "+1200" respectively
				case "K": return "+1000";
				case "L": return "+1100";
				case "M": return "+1200";

				// "N" through "Y"
				// are equivalent to "-0100" through "-1200" respectively
				case "N": return "-0100";
				case "O": return "-0200";
				case "P": return "-0300";
				case "EDT":  // EDT is semantically equivalent to -0400
				case "Q": 
					  return "-0400";
				case "EST": // EST,CDT is semantically equivalent to -0500
				case "CDT":
				case "R": 
					  return "-0500";
				case "CST": // CST,MDT is semantically equivalent to -0600
				case "MDT":
				case "S": 
					  return "-0600";
				case "MST": // US timezones MST,PDT is semantically equivalent to -0700
				case "PDT": 
				case "T": 
					  return "-0700";
				case "PST": // US timezone PST is semantically equivalent to -0800
				case "U": 
					  return "-0800";
				case "V": return "-0900";
				case "W": return "-1000";
				case "X": return "-1100";
				case "Y": return "-1200";

				// "Z", "UT" and "GMT"
				// is equivalent to "+0000"
				case "Z":
				case "UT":
				case "GMT":
					return "+0000";			

				default:
					throw new ArgumentException("Unexpected input");
			}
		}

		/// <summary>
		/// Extracts the date and time parts from the <paramref name="dateInput"/>
		/// </summary>
		/// <param name="dateInput">The date input string, from which to extract the date and time parts</param>
		/// <returns>The extracted date part or <see langword="DateTime.MinValue"/> if <paramref name="dateInput"/> is not recognized as a valid date.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="dateInput"/> is <see langword="null"/></exception>
		private static DateTime ExtractDateTime(string dateInput)
		{
			if (dateInput == null)
				throw new ArgumentNullException("dateInput");

			//If there are some custom formats, try and parse the dateInput string with these first
			if (CustomDateTimeFormats != null && CustomDateTimeFormats.Length > 0)
			{
				//If there is a timezone at the end, remove it
				string strDate = dateInput.Trim();
				if (strDate.Contains(" ")) //Check contains a space before getting the last part to prevent accessing index -1
				{
					string[] parts = strDate.Split(' ');
					string lastPart = parts[parts.Length - 1];

					// Convert timezones in older formats to [+-]dddd format.
					lastPart = Regex.Replace(lastPart, REGEX_OLD_TIMEZONE_FORMATS, MatchEvaluator);

					// Find the timezone specification
					// Example: Fri, 21 Nov 1997 09:55:06 -0600
					// finds -0600
					Match timezoneMatch = Regex.Match(lastPart, REGEX_NEW_TIMEZONE_FORMATS);
					if (timezoneMatch.Success)
					{
						//This last part is a timezone, remove it
						strDate = strDate.Substring(0, strDate.Length - parts[parts.Length - 1].Length).Trim(); //Use the length of the old last part
					}
				}

				//Try and parse it as one of the custom formats
				try
				{
					DateTime dateTime = DateTime.ParseExact(strDate, CustomDateTimeFormats, null, DateTimeStyles.None);
					DefaultLogger.Log.LogDebug(String.Format("Successfully parsed date input \"{0}\" using a custom format. Converted to date: {1}", dateInput, dateTime.ToString()));
					return dateTime;
				}
				catch (FormatException) {  }
			}

			// Matches the date and time part of a string
			// Given string example: Fri, 21 Nov 1997 09:55:06 -0600
			// Needs to find: 21 Nov 1997 09:55:06

			// Seconds does not need to be specified
			// Even though it is illigal, sometimes hours, minutes or seconds are only specified with one digit

			// Year with 2 or 4 digits (1922 or 22)
			const string year = @"(\d\d\d\d|\d\d)";

			// Time with one or two digits for hour and minute and optinal seconds (06:04:06 or 6:4:6 or 06:04 or 6:4)
			const string time = @"\d?\d:\d?\d(:\d?\d)?";

			// Correct format is 21 Nov 1997 09:55:06
			const string correctFormat = @"\d\d? .+ " + year + " " + time;

			// Some uses incorrect format: 2012-1-1 12:30
			const string incorrectFormat = year + @"-\d?\d-\d?\d " + time;

			// Some uses incorrect format: 08-May-2012 16:52:30 +0100
			const string correctFormatButWithDashes = @"\d\d?-[A-Za-z]{3}-" + year + " " + time;

			// We allow both correct and incorrect format
			const string joinedFormat = @"(" + correctFormat + ")|(" + incorrectFormat + ")|(" + correctFormatButWithDashes + ")";

			Match match = Regex.Match(dateInput, joinedFormat);
			if (match.Success)
			{
				try
				{
					return Convert.ToDateTime(match.Value, CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					DefaultLogger.Log.LogError("The given date appeared to be in a valid format, but could not be converted to a DateTime object: " + dateInput);
				}
			}
			else
			{
				DefaultLogger.Log.LogError("The given date does not appear to be in a valid format: " + dateInput);
			}

			return DateTime.MinValue;
		}

		/// <summary>
		/// Validates that the given <paramref name="dateTime"/> agrees with a day-name specified
		/// in <paramref name="dateInput"/>.
		/// </summary>
		/// <param name="dateTime">The time to check</param>
		/// <param name="dateInput">The date input to extract the day-name from</param>
		/// <exception cref="ArgumentException">If <paramref name="dateTime"/> and <paramref name="dateInput"/> does not agree on the day</exception>
		private static void ValidateDayNameIfAny(DateTime dateTime, string dateInput)
		{
			// Check if there is a day name in front of the date
			// Example: Fri, 21 Nov 1997 09:55:06 -0600
			if (dateInput.Length >= 4 && dateInput[3] == ',')
			{
				string dayName = dateInput.Substring(0, 3);

				// If a dayName was specified. Check that the dateTime and the dayName
				// agrees on which day it is
				// This is just a failure-check and could be left out
				if ((dateTime.DayOfWeek == DayOfWeek.Monday		&& !dayName.Equals("Mon")) ||
					(dateTime.DayOfWeek == DayOfWeek.Tuesday	&& !dayName.Equals("Tue")) ||
					(dateTime.DayOfWeek == DayOfWeek.Wednesday	&& !dayName.Equals("Wed")) ||
					(dateTime.DayOfWeek == DayOfWeek.Thursday	&& !dayName.Equals("Thu")) ||
					(dateTime.DayOfWeek == DayOfWeek.Friday		&& !dayName.Equals("Fri")) ||
					(dateTime.DayOfWeek == DayOfWeek.Saturday	&& !dayName.Equals("Sat")) ||
					(dateTime.DayOfWeek == DayOfWeek.Sunday		&& !dayName.Equals("Sun")))
				{
					DefaultLogger.Log.LogDebug("Day-name does not correspond to the weekday of the date: " + dateInput);					
				}
			}

			// If no day name was found no checks can be made
		}

		/// <summary>
		/// Strips and removes all comments and excessive whitespace from the string
		/// </summary>
		/// <param name="input">The input to strip from</param>
		/// <returns>The stripped string</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="input"/> is <see langword="null"/></exception>
		private static string StripCommentsAndExcessWhitespace(string input)
		{
			if(input == null)
				throw new ArgumentNullException("input");

			// Strip out comments
			// Also strips out nested comments
			input = Regex.Replace(input, @"(\((?>\((?<C>)|\)(?<-C>)|.?)*(?(C)(?!))\))", "");

			// Reduce any whitespace character to one space only
			input = Regex.Replace(input, @"\s+", " ");

			// Remove all initial whitespace
			input = Regex.Replace(input, @"^\s+", "");

			// Remove all ending whitespace
			input = Regex.Replace(input, @"\s+$", "");

			// Remove spaces at colons
			// Example: 22: 33 : 44 => 22:33:44
			input = Regex.Replace(input, @" ?: ?", ":");

			return input;
		}

		/// <summary>
		/// Converts date time string in very wrong date time format:
		/// Tue Feb 18 10:23:30 2014 (MSK)
		/// to
		/// Feb 18 2014 10:23:30 MSK
		/// </summary>
		/// <param name="inputDate">The date to convert</param>
		/// <returns>The corrected string</returns>
		private static string FixSpecialCases(string inputDate)
		{
			const string weekDayPattern = "(?<weekDay>Mon|Tue|Wed|Thu|Fri|Sat|Sun)";
			const string monthPattern = @"(?<month>[A-Za-z]+)";
			const string dayPattern = @"(?<day>\d?\d)";
			const string yearPattern = @"(?<year>\d\d\d\d)";
			const string timePattern = @"(?<time>\d?\d:\d?\d(:\d?\d)?)";
			const string timeZonePattern = @"(?<timeZone>[A-Z]{3})";

			string incorrectFormat = String.Format(@"{0} +{1} +{2} +{3} +{4} +\({5}\)", weekDayPattern, monthPattern, dayPattern, timePattern, yearPattern, timeZonePattern);

			Match match = Regex.Match(inputDate, incorrectFormat);
			if(match.Success)
			{
				var month = match.Groups["month"];
				var day = match.Groups["day"];
				var year = match.Groups["year"];
				var time = match.Groups["time"];
				var timeZone = match.Groups["timeZone"];
				return String.Format("{0} {1} {2} {3} {4}", day, month, year, time, timeZone);
			}

			return inputDate;
		}
	}
}
