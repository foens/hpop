using System;
using System.Text.RegularExpressions;

namespace OpenPOP.MIME.Header
{
	/// <summary>
	/// Copyright (c)  vendredi13@007.freesurf.fr
	/// All rights reserved.
	/// 
	/// Redistribution and use in source and binary forms, with or without
	/// modification, are permitted.
	/// 
	/// Bugs supplied by Ross Presser on codeproject.com has been incorporated.
	/// </summary>
	/// <see cref="http://www.codeproject.com/KB/recipes/rfc2822-date-parser.aspx">for original version</see>
	/// <see cref="http://www.codeproject.com/info/cpol10.aspx">for license</see>
	public static class RFC2822DateTime
	{
		/// <summary>
		/// Converts a string in RFC 2822 format into a <see cref="DateTime"/> object
		/// </summary>
		/// <param name="adate">The date to convert</param>
		/// <returns>A valid <see cref="DateTime"/> object, which represents the same time as the string that was converted</returns>
		public static DateTime StringToDate(string adate)
		{
			string dayName;
			DateTime dt;

			// FIXME : how to handle nested comments ?
			string tmp = Regex.Replace(adate, "(\\([^(].*\\))", "");

			// strip extra white spaces
			tmp = Regex.Replace(tmp, "\\s+", " ");
			tmp = Regex.Replace(tmp, "^\\s+", "");
			tmp = Regex.Replace(tmp, "\\s+$", "");

			// extract week name part
			string[] resp = tmp.Split(new[] { ',' }, 2);
			if (resp.Length == 2)
			{
				// there's week name
				dayName = resp[0];
				tmp = resp[1];
			}
			else dayName = "";

			try
			{
				// extract date and time
				int pos = tmp.LastIndexOf(" ");
				if (pos < 1) throw new FormatException("probably not a date");
				string dpart = tmp.Substring(0, pos);
				string timeZone = tmp.Substring(pos + 1);
				dt = Convert.ToDateTime(dpart);

				// check weekDay name
				// this must be done before convert to GMT 
				if (dayName != string.Empty)
				{
					if ((dt.DayOfWeek == DayOfWeek.Friday && dayName != "Fri") ||
						(dt.DayOfWeek == DayOfWeek.Monday && dayName != "Mon") ||
						(dt.DayOfWeek == DayOfWeek.Saturday && dayName != "Sat") ||
						(dt.DayOfWeek == DayOfWeek.Sunday && dayName != "Sun") ||
						(dt.DayOfWeek == DayOfWeek.Thursday && dayName != "Thu") ||
						(dt.DayOfWeek == DayOfWeek.Tuesday && dayName != "Tue") ||
						(dt.DayOfWeek == DayOfWeek.Wednesday && dayName != "Wed")
						)
						throw new FormatException("invalid week of day");
				}

				// adjust to localtime
				if (Regex.IsMatch(timeZone, "[+\\-][0-9][0-9][0-9][0-9]"))
				{
					// it's a modern ANSI style timezone
					int factor;
					string hour = timeZone.Substring(1, 2);
					string minute = timeZone.Substring(3, 2);
					if (timeZone.Substring(0, 1) == "+") factor = -1;
					else if (timeZone.Substring(0, 1) == "-") factor = 1;
					else throw new FormatException("incorrect time zone");
					dt = dt.AddHours(factor * Convert.ToInt32(hour));
					dt = dt.AddMinutes(factor * Convert.ToInt32(minute));
				}
				else
				{
					// it's a old style military time zone ?
					switch (timeZone)
					{
						case "A": dt = dt.AddHours(1); break;
						case "B": dt = dt.AddHours(2); break;
						case "C": dt = dt.AddHours(3); break;
						case "D": dt = dt.AddHours(4); break;
						case "E": dt = dt.AddHours(5); break;
						case "F": dt = dt.AddHours(6); break;
						case "G": dt = dt.AddHours(7); break;
						case "H": dt = dt.AddHours(8); break;
						case "I": dt = dt.AddHours(9); break;
						case "K": dt = dt.AddHours(10); break;
						case "L": dt = dt.AddHours(11); break;
						case "M": dt = dt.AddHours(12); break;
						case "N": dt = dt.AddHours(-1); break;
						case "O": dt = dt.AddHours(-2); break;
						case "P": dt = dt.AddHours(-3); break;
						case "Q": dt = dt.AddHours(-4); break;
						case "R": dt = dt.AddHours(-5); break;
						case "S": dt = dt.AddHours(-6); break;
						case "T": dt = dt.AddHours(-7); break;
						case "U": dt = dt.AddHours(-8); break;
						case "V": dt = dt.AddHours(-9); break;
						case "W": dt = dt.AddHours(-10); break;
						case "X": dt = dt.AddHours(-11); break;
						case "Y": dt = dt.AddHours(-12); break;
						case "Z":
						case "UT":
						case "GMT": break;    // It's UTC
						case "EST": dt = dt.AddHours(5); break;
						case "EDT": dt = dt.AddHours(4); break;
						case "CST": dt = dt.AddHours(6); break;
						case "CDT": dt = dt.AddHours(5); break;
						case "MST": dt = dt.AddHours(7); break;
						case "MDT": dt = dt.AddHours(6); break;
						case "PST": dt = dt.AddHours(8); break;
						case "PDT": dt = dt.AddHours(7); break;
						default: throw new FormatException("invalid time zone");
					}
				}
			}
			catch (Exception e)
			{
				throw new FormatException(string.Format("Invalid date:{0}:{1}", e.Message, adate));
			}
			return dt;
		}
	}
}