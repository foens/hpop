/*	 /--==###################==--\
 *	 |	Bram's Ini File Handler  |
 *	 \--==###################==--/
 * 
 * This handles Ini files and all their content.
 * 
 * Some explanation:
 * Categories are in fact sections, but i didn't think
 * "sections" so i wrote "categories". Sorry.
 * 
 * comment lines in ini files begin with #, ; or //
 * multi-line are not supported (Because I've never seen such)
 * 
 * It ignores comments on reading but can write them
 * 
 * How it works:
 * All data is saved in one System.Collections.SortedList which
 * contains the category names as keys, and all key-value pairs
 * as values, saved as SortedList too:
 * 
 * explanation sheet
 * 
 * SortedList Categories
 * {
 *		{"Category1", {Key1, value1}
 *					  {Key2, value2}
 *					  ...
 *								   }
 *		{"Category2", {Key1, value1}
 *					  {Key2, value2}
 *					  ...
 *								   }
 *		...
 * }
 * 
 * that behaves like an array in an array (array[][]), but with dynamic bounds
 * and strings as indexers.
 * 
 * I hope you did understand this, it would have been easier to explain 
 * in French or German...
 * 
 * You can make with it what you want, but I would be pleased to
 * hear some feedback. (Ok, I admit: I would be pleased only if it's
 * positive feedback...)
 * 
 * Send me an email! kratchkov@inbox.lv
 * 
 * Thanks
 * 
 * DISCLAIMER:
 * THIS CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE.  THE  ENTIRE RISK  AS TO THE  QUALITY AND PERFORMANCE OF THIS
 * CODE IS WITH YOU. SHOULD THIS CODE PROVE DEFECTIVE, YOU ASSUME
 * THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION .
 * You take full responsibility for the use of this code and any
 * consequences thereof. I can not accept liability for damages
 * or failures arising from the use of this code, or parts of this code.
 */

using System;
using System.IO;
using System.Text;
using System.Collections;

namespace MailMonitor
{
	/// <summary>
	/// Handles Ini categories, keys and their associated values, static methods implemented for file
	/// handling (saving and reading)
	/// </summary>
	public class IniStructure
	{
		#region Ini structure code
		private SortedList Categories = new SortedList();

		/// <summary>
		/// Initialies a new IniStructure
		/// </summary>
		public IniStructure()
		{
			return; // There's nothing to do...
		}

		/// <summary>
		/// Adds a category to the IniStructure
		/// </summary>
		/// <param name="Name">Name of the new category</param>
		public bool AddCategory(string Name)
		{
			if (Name == "" | Categories.ContainsKey(Name))
				return false;
			if (Name.IndexOf('=') != -1
				| Name.IndexOf('[') != -1
				| Name.IndexOf(']') != -1) // these characters are not allowed in a category name
				return false;

			Categories.Add(Name, new SortedList());
			return true;
		}

		/// <summary>
		/// Deletes a category and its contents
		/// </summary>
		/// <param name="Name">category to delete</param>
		public bool DeleteCategory(string Name)
		{
			if (Name == "" | !Categories.ContainsKey(Name))
				return false;
			Categories.Remove(Name);
			return true;
		}

		/// <summary>
		/// Renames a category
		/// </summary>
		/// <param name="Name">Category to rename</param>
		/// <param name="NewName">New name</param>
		public bool RenameCategory(string Name, string NewName)
		{ //		Or rather moves a category to a new name
			if (Name == "" | !Categories.ContainsKey(Name) | NewName == "")
				return false;

			if (NewName.IndexOf('=') != -1
				| NewName.IndexOf('[') != -1
				| NewName.IndexOf(']') != -1) // these characters are not allowed in a category name
				return false;

			SortedList Category = (SortedList)(Categories[Name]);
			Categories.Add(NewName, Category);
			this.DeleteCategory(Name);
			return true;
		}

		/// <summary>
		/// Returns the names of all categories
		/// </summary>
		/// <returns></returns>
		public string[] GetCategories()
		{
			string[] CatNames = new string[Categories.Count];
			IList KeyList = Categories.GetKeyList();
			int KeyCount = Categories.Count;
			for (int i = 0; i < KeyCount; i++)
			{
				CatNames[i] = KeyList[i].ToString();
			}
			return CatNames;
		}

		/// <summary>
		/// Returns the name of a category by specifying the index.
		/// Useful to enumerate through all categories.
		/// </summary>
		/// <param name="Index">The category index</param>
		/// <returns></returns>
		public string GetCategoryName(int Index)
		{
			if (Index < 0 | Index >= Categories.Count)
				return null;
			return Categories.GetKey(Index).ToString();
		}

		/// <summary>
		/// Adds a key-value pair to a specified category
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <param name="Key">New name of the key</param>
		/// <param name="Value">Associated value</param>
		public bool AddValue(string CategoryName, string Key, string Value)
		{
			if (CategoryName == "" | Key == "")
				return false;
			if (Key.IndexOf('=') != -1
				| Key.IndexOf('[') != -1
				| Key.IndexOf(']') != -1	// these chars are not allowed for keynames
				| Key.IndexOf(';') != -1
				| Key.IndexOf('#') != -1
				)
				return false;
			if (!Categories.ContainsKey(CategoryName))
				return false;
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if (Category.ContainsKey(Key))
				return false;
			Category.Add(Key, Value);
			return true;
		}

		/// <summary>
		/// Returns the value of a key-value pair in a specified category by specifying the key
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <param name="Key">Name of the Key</param>
		/// <returns></returns>
		public string GetValue(string CategoryName, string Key)
		{
			if (CategoryName == "" | Key == "")
				return null;
			if (!Categories.ContainsKey(CategoryName))
				return null;
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if (!Category.ContainsKey(Key))
				return null;
			return Category[Key].ToString();
		}

		/// <summary>
		/// Returns the key-value pair in a specified category by specifying the index
		/// </summary>
		/// <param name="CategoryName">Index of the category</param>
		/// <param name="Key">Index of the Key</param>
		/// <returns></returns>
		public string GetValue(int CatIndex, int KeyIndex)
		{
			if (CatIndex < 0 | KeyIndex < 0
				|CatIndex >= Categories.Count)
				return null;
			SortedList Category = (SortedList)(Categories.GetByIndex(CatIndex));
			if (KeyIndex >= Category.Count)
				return null;
			return Category.GetByIndex(KeyIndex).ToString();
		}

		/// <summary>
		/// Returns the name of the key in a key-value pair in a specified category by specifying the index
		/// </summary>
		/// <param name="CatIndex">Index of the category</param>
		/// <param name="KeyIndex">Index of the key</param>
		/// <returns></returns>
		public string GetKeyName(int CatIndex, int KeyIndex)
		{
			if (CatIndex < 0 | KeyIndex < 0
				|CatIndex >= Categories.Count)
				return null;
			SortedList Category = (SortedList)(Categories.GetByIndex(CatIndex));
			if (KeyIndex >= Category.Count)
				return null;
			return Category.GetKey(KeyIndex).ToString();
		}


		/// <summary>
		/// Deletes a key-value pair
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <param name="Key">Name of the Key</param>
		public bool DeleteValue(string CategoryName, string Key)
		{
			if (CategoryName == "" | Key == "")
				return false;
			if (!Categories.ContainsKey(CategoryName))
				return false;
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if (!Category.ContainsKey(Key))
				return false;
			Category.Remove(Key);
			return true;
		}

		/// <summary>
		/// Renames the keyname in a key-value pair
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <param name="KeyName">Name of the Key</param>
		/// <param name="NewKeyName">New name of the Key</param>
		public bool RenameKey(string CategoryName, string KeyName, string NewKeyName)
		{
			if (CategoryName == "" | KeyName == "" | NewKeyName == "")
				return false;
			if (!Categories.ContainsKey(CategoryName))
				return false;
			if (NewKeyName.IndexOf('=') != -1
				| NewKeyName.IndexOf('[') != -1
				| NewKeyName.IndexOf(']') != -1	// these chars are not allowed for keynames
				| NewKeyName.IndexOf(';') != -1
				| NewKeyName.IndexOf('#') != -1
				)
				return false;
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if ( !Category.ContainsKey(KeyName))
				return false;
			
			object value = Category[KeyName];
			Category.Remove(KeyName);
			Category.Add(NewKeyName, value);
			return true;
		}

		/// <summary>
		/// Modifies the value in a key-value pair
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <param name="KeyName">Name of the Key</param>
		/// <param name="NewValue">New name of the Key</param>
		public bool ModifyValue(string CategoryName, string KeyName, string NewValue)
		{
			if (CategoryName == "" | KeyName == "")
				return false;
			if (!Categories.ContainsKey(CategoryName))
				return false;
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if ( !Category.ContainsKey(KeyName))
				return false;
			
			Category[KeyName] = NewValue;
			return true;
		}

		/// <summary>
		/// Returns all keys in a category
		/// </summary>
		/// <param name="CategoryName">Name of the category</param>
		/// <returns></returns>
		public string[] GetKeys(string CategoryName)
		{
			SortedList Category = (SortedList)(Categories[CategoryName]);
			if (Category == null)
				return null;
			int KeyCount = Category.Count;
			string[] KeyNames = new string[KeyCount];
			IList KeyList = Category.GetKeyList();
			for (int i = 0; i < KeyCount; i++)
			{
				KeyNames[i] = KeyList[i].ToString();
			}
			return KeyNames;
		}

		#endregion

		#region Ini writing code
		/// <summary>
		/// Writes an IniStructure to a file with a comment.
		/// </summary>
		/// <param name="IniData">The contents to write</param>
		/// <param name="Filename">The complete path and name of the file</param>
		/// <param name="comment">Comment to add</param>
		/// <returns></returns>
		public static bool WriteIni(IniStructure IniData, string Filename, string comment)
		{
			string DataToWrite = CreateData(IniData, BuildComment(comment));
			return WriteFile(Filename, DataToWrite);
		}

		/// <summary>
		/// Writes an IniStructure to a file without a comment.
		/// </summary>
		/// <param name="IniData">The contents to write</param>
		/// <param name="Filename">The complete path and name of the file</param>
		/// <returns></returns>
		public static bool WriteIni(IniStructure IniData, string Filename)
		{
			string DataToWrite = CreateData(IniData);
			return WriteFile(Filename, DataToWrite);
		}

		private static bool WriteFile(string Filename, string Data)
		{	// Writes a string to a file
			try
			{
				FileStream IniStream = new FileStream(Filename,FileMode.Create);
				if (!IniStream.CanWrite)
				{
					IniStream.Close();
					return false;
				}
				StreamWriter writer = new StreamWriter(IniStream);
				writer.Write(Data);
				writer.Flush();
				writer.Close();
				IniStream.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static string BuildComment(string comment)
		{ // Adds a # at the beginning of each line
			if (comment == "")
				return "";
			string[] Lines = DivideToLines(comment);
			string temp = "";
			foreach (string line in Lines)
			{
				temp += "# " + line + "\r\n";
			}
			return temp;
		}

		private static string CreateData(IniStructure IniData)
		{
			return CreateData(IniData,"");
		}

		private static string CreateData(IniStructure IniData, string comment)
		{	//Iterates through all categories and keys and appends all data to Data
			int CategoryCount = IniData.GetCategories().Length;
			int[] KeyCountPerCategory = new int[CategoryCount];
			string Data = comment;
			string[] temp = new string[2]; // will contain key-value pair
			
			for (int i = 0; i < CategoryCount; i++) // Gets keycount per category
			{
				string CategoryName = IniData.GetCategories()[i];
				KeyCountPerCategory[i] = IniData.GetKeys(CategoryName).Length;
			}

			for (int catcounter = 0; catcounter < CategoryCount; catcounter++)
			{
				Data += "\r\n[" + IniData.GetCategoryName(catcounter) + "]\r\n"; 
				// writes [Category] to Data
				for (int keycounter = 0; keycounter < KeyCountPerCategory[catcounter]; keycounter++)
				{
					temp[0] = IniData.GetKeyName(catcounter, keycounter);
					temp[1] = IniData.GetValue(catcounter, keycounter);
					Data += temp[0] + "=" + temp[1] + "\r\n";
					// writes the key-value pair to Data
				}
			}
			return Data;
		}
		#endregion

		#region Ini reading code

		/// <summary>
		/// Reads an ini file and returns the content as an IniStructure. Returns null if an error occurred.
		/// </summary>
		/// <param name="Filename">The filename to read</param>
		/// <returns></returns>
		public static IniStructure ReadIni(string Filename)
		{
			string Data = ReadFile(Filename);
			if (Data == null)
				return null;

			IniStructure data = InterpretIni(Data);
			
			return data;
		}

		public static IniStructure InterpretIni(string Data)
		{
			IniStructure IniData = new IniStructure();
			string[] Lines = RemoveAndVerifyIni(DivideToLines(Data));
			// Divides the Data in lines, removes comments and empty lines
			// and verifies if the ini is not corrupted
			// Returns null if it is.
			if (Lines == null)
				return null;

			if (IsLineACategoryDef(Lines[0]) != LineType.Category)
			{
				return null;
				// Ini is faulty - does not begin with a categorydef
			}
			string CurrentCategory = "";
			foreach (string line in Lines)
			{
				switch (IsLineACategoryDef(line))
				{
					case LineType.Category:	// the line is a correct category definition
						string NewCat = line.Substring(1,line.Length - 2);
						IniData.AddCategory(NewCat); // adds the category to the IniData
						CurrentCategory = NewCat;
						break;
					case LineType.NotACategory: // the line is not a category definition
						string[] keyvalue = GetDataFromLine(line);
						IniData.AddValue(CurrentCategory, keyvalue[0], keyvalue[1]);
						// Adds the key-value to the current category
						break;
					case LineType.Faulty: // the line is faulty
						return null;
				}
			}
			return IniData;
		}

		private static string ReadFile(string filename)
		{		// Reads a file to a string.
			if (!File.Exists(filename))
				return null;
			StringBuilder IniData;
			try
			{
				FileStream IniStream = new FileStream(filename,FileMode.Open,FileAccess.Read);
				if (!IniStream.CanRead)
				{
					IniStream.Close();
					return null;
				}
				StreamReader reader = new StreamReader(IniStream);
				IniData = new StringBuilder();
				IniData.Append(reader.ReadToEnd());
				reader.Close();
				IniStream.Close();
				return IniData.ToString();
			}
			catch
			{
				return null;
			}
		}
		
		private static string[] GetDataFromLine(string Line)
		{
			// returns the key and the value of a key-value pair in "key=value" format.
			int EqualPos = 0;
			EqualPos = Line.IndexOf("=", 0);
			if (EqualPos < 1)
			{
				return null;
			}
			string LeftKey = Line.Substring(0, EqualPos);
			string RightValue = Line.Substring(EqualPos + 1);
			
			string[] ToReturn = {LeftKey, RightValue};
			return ToReturn;
		}

		private enum LineType // return type for IsLineACategoryDef and LineVerify
		{
			NotACategory,
			Category,
			Faulty,
			Comment,
			Empty,
			Ok
		}

		private static LineType IsLineACategoryDef(string Line)
		{
			if (Line.Length < 3)
				return LineType.NotACategory; // must be a short keyname like "k="
            
			if (Line.Substring(0,1) == "[" & Line.Substring(Line.Length - 1, 1) == "]")
				// seems to be a categorydef
			{
				if (Line.IndexOf("=") != -1) 
					//  '=' found -> is incorrect for category def
					return LineType.Faulty;
				if (ContainsMoreThanOne(Line,'[') | ContainsMoreThanOne(Line, ']'))
					// two or more '[' or ']' found -> incorrect
					return LineType.Faulty;
				return LineType.Category;
			}
			return LineType.NotACategory;
		}

		private static string[] DivideToLines(string Data)
		{		// Divides a string into lines
			string[] Lines = new string[Data.Length];
			int oldnewlinepos = 0;
			int LineCounter = 0;
			for (int i = 0; i < Data.Length; i++)
			{
				if (Data.ToCharArray(i,1)[0] == '\n')
				{
					Lines[LineCounter] = Data.Substring(oldnewlinepos, i - oldnewlinepos - 1);
					oldnewlinepos = i + 1;
					LineCounter++;
				}
			}

			// Lines[] array is too big: needs to be trimmed
			
			Lines[LineCounter] = Data.Substring(oldnewlinepos, Data.Length - oldnewlinepos);
			string[] LinesTrimmed = new string[LineCounter + 1];
			for (int i = 0; i < LineCounter + 1; i++)
			{
				LinesTrimmed[i] = Lines[i];
			}
			return LinesTrimmed;
		}

		private static bool ContainsMoreThanOne(string Data, char verify)
		{	// returns true if Data contains verify more than once
			char[] data = Data.ToCharArray();
			int count = 0;
			foreach (char c in data)
			{
				if (c == verify)
					count++;
			}
			if (count > 1)
				return true;
			return false;
		}

		private static LineType LineVerify(string line)
		{		// Verifies a line of an ini
			if (line == "")
				return LineType.Empty;

			if (line.IndexOf(";") == 0 | line.IndexOf("#") == 0 | line.IndexOf("//") == 0)
			{
				return LineType.Comment; // line is a comment: ignore
			}

			int equalindex = line.IndexOf('=');
			if (equalindex == 0)
				return LineType.Faulty; // an '=' cannot be on first place

			if (equalindex != -1) // if = is found in line
			{
				// Verify: no '[' , ']' ,';' or '#' before the '='
				if (line.IndexOf('[', 0, equalindex) != -1
					| line.IndexOf(']', 0, equalindex) != -1
					| line.IndexOf(';', 0, equalindex) != -1
                    | line.IndexOf('#', 0, equalindex) != -1)
					return LineType.Faulty;
       		}

			return LineType.Ok;
		}

		private static string[] RemoveAndVerifyIni(string[] Lines)
		{
			// removes empty lines and comments, and verifies every line
			string[] temp = new string[Lines.Length];
			int TempCounter = 0; // number of lines to return
			foreach (string line in Lines)
			{
				switch (LineVerify(line))
				{
					case LineType.Faulty: // line is faulty
						return null;
					case LineType.Comment:	//	line is a comment
						continue;
					case LineType.Ok:	// line is ok
						temp[TempCounter] = line;
						TempCounter++;
						break;
					case LineType.Empty: // line is empty
						continue;
				}
			}
			// the temp[] array is too big: needs to be trimmed.
			string[] OKLines = new string[TempCounter];
			for (int i = 0; i < TempCounter; i++)
			{
				OKLines[i] = temp[i];
			}
			return OKLines;
		}
		#endregion
	}
}