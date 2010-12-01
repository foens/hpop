using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpenPop.Mime.Decode
{
	/// <summary>
	/// Portions Copyright (c) vendredi13@007.freesurf.fr<br/>
	/// All rights reserved.<br/>
	/// <br/>
	/// Redistribution and use in source and binary forms, with or without
	/// modification, are permitted.<br/>
	/// <br/>
	/// Bugs supplied by Ross Presser on codeProject.com has been incorporated.
	/// </summary>
	/// <remarks> 
	/// See <a href="http://www.codeproject.com/KB/recipes/rfc2822-date-parser.aspx">RFC 2822-date-parser</a> for original version.<br/>
	/// See <a href="http://www.codeproject.com/info/cpol10.aspx">CodeProject.com license</a> for license.
	/// </remarks>
	internal static class Rfc2822DateTime
	{
		/// <summary>
		/// Converts a string in RFC 2822 format into a <see cref="DateTime"/> object
		/// </summary>
		/// <param name="aDate">The date to convert</param>
		/// <returns>A valid <see cref="DateTime"/> object, which represents the same time as the string that was converted</returns>
		/// <exception cref="ArgumentNullException"><exception cref="ArgumentNullException">If <paramref name="aDate"/> is <see langword="null"/></exception></exception>
		/// <exception cref="FormatException">If the <paramref name="aDate"/> could not be parsed into a <see cref="DateTime"/> object</exception>
		public static DateTime StringToDate(string aDate)
		{
			if(aDate == null)
				throw new ArgumentNullException("aDate");

			// Strip out comments, handles nested comments as well
			string temp = Regex.Replace(aDate, @"(\((?>\((?<C>)|\)(?<-C>)|.?)*(?(C)(?!))\))", "");

			// strip extra white spaces
			temp = Regex.Replace(temp, @"\s+", " ");
			temp = Regex.Replace(temp, @"^\s*(.*?)\s*$", "$1");

			string dayName;
			// extract week name part
			string[] resp = temp.Split(new[] {','}, 2);
			if (resp.Length == 2)
			{
				// there's week name
				dayName = resp[0];
				temp = resp[1];
			} else
				dayName = "";

			try
			{
				// extract date and time
				int position = temp.LastIndexOf(' ');
				if (position < 1)
					throw new FormatException("Probably not a date");

				string dpart = temp.Substring(0, position);
				string timeZone = temp.Substring(position + 1);

				// Date parts should always be parsed as english, and represented as UTC
				DateTime dateTime = new DateTime(Convert.ToDateTime(dpart, CultureInfo.InvariantCulture).Ticks, DateTimeKind.Utc);

				// check weekDay name
				// this must be done before convert to GMT 
				if (!string.IsNullOrEmpty(dayName))
				{
					if ((dateTime.DayOfWeek == DayOfWeek.Friday && dayName != "Fri") ||
					    (dateTime.DayOfWeek == DayOfWeek.Monday && dayName != "Mon") ||
					    (dateTime.DayOfWeek == DayOfWeek.Saturday && dayName != "Sat") ||
					    (dateTime.DayOfWeek == DayOfWeek.Sunday && dayName != "Sun") ||
					    (dateTime.DayOfWeek == DayOfWeek.Thursday && dayName != "Thu") ||
					    (dateTime.DayOfWeek == DayOfWeek.Tuesday && dayName != "Tue") ||
					    (dateTime.DayOfWeek == DayOfWeek.Wednesday && dayName != "Wed")
						)
						throw new FormatException("Invalid week of day");
				}

				// adjust to localtime
				if (Regex.IsMatch(timeZone, @"[+\-][0-9][0-9][0-9][0-9]"))
				{
					// it's a modern ANSI style timezone
					int timezoneFactor;
					if (timeZone.Substring(0, 1) == "+")
						timezoneFactor = -1;
					else if (timeZone.Substring(0, 1) == "-")
						timezoneFactor = 1;
					else
						throw new FormatException("Incorrect time zone");
					string hour = timeZone.Substring(1, 2);
					string minute = timeZone.Substring(3, 2);
					dateTime = dateTime.AddHours(timezoneFactor*Convert.ToInt32(hour, CultureInfo.InvariantCulture));
					dateTime = dateTime.AddMinutes(timezoneFactor*Convert.ToInt32(minute, CultureInfo.InvariantCulture));
				} else
				{
					// it's a old style military time zone ?
					switch (timeZone)
					{
						case "A": dateTime = dateTime.AddHours(1); break;
						case "B": dateTime = dateTime.AddHours(2); break;
						case "C": dateTime = dateTime.AddHours(3); break;
						case "D": dateTime = dateTime.AddHours(4); break;
						case "E": dateTime = dateTime.AddHours(5); break;
						case "F": dateTime = dateTime.AddHours(6); break;
						case "G": dateTime = dateTime.AddHours(7); break;
						case "H": dateTime = dateTime.AddHours(8); break;
						case "I": dateTime = dateTime.AddHours(9); break;
						case "K": dateTime = dateTime.AddHours(10); break;
						case "L": dateTime = dateTime.AddHours(11); break;
						case "M": dateTime = dateTime.AddHours(12); break;
						case "N": dateTime = dateTime.AddHours(-1); break;
						case "O": dateTime = dateTime.AddHours(-2); break;
						case "P": dateTime = dateTime.AddHours(-3); break;
						case "Q": dateTime = dateTime.AddHours(-4); break;
						case "R": dateTime = dateTime.AddHours(-5); break;
						case "S": dateTime = dateTime.AddHours(-6); break;
						case "T": dateTime = dateTime.AddHours(-7); break;
						case "U": dateTime = dateTime.AddHours(-8); break;
						case "V": dateTime = dateTime.AddHours(-9); break;
						case "W": dateTime = dateTime.AddHours(-10); break;
						case "X": dateTime = dateTime.AddHours(-11); break;
						case "Y": dateTime = dateTime.AddHours(-12); break;
						case "Z":
						case "UT":
						case "GMT": break;    // It's UTC
						case "EST": dateTime = dateTime.AddHours(5); break;
						case "EDT": dateTime = dateTime.AddHours(4); break;
						case "CST": dateTime = dateTime.AddHours(6); break;
						case "CDT": dateTime = dateTime.AddHours(5); break;
						case "MST": dateTime = dateTime.AddHours(7); break;
						case "MDT": dateTime = dateTime.AddHours(6); break;
						case "PST": dateTime = dateTime.AddHours(8); break;
						case "PDT": dateTime = dateTime.AddHours(7); break;
						default:
							throw new FormatException("Invalid time zone");
					}
				}
				return dateTime;
			} catch (Exception e)
			{
				throw new FormatException("Invalid Date: " + e.Message + ". Input: \"" + aDate + "\"", e);
			}
		}
	}
}