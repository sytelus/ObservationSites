using System;
using System.Text.RegularExpressions;

namespace ObservationSites
{
	/// <summary>
	/// Summary description for CommonFunctions.
	/// </summary>
	public class CommonFunctions
	{
		public static String ReplaceString(String SourceString, String SearchString, String ReplaceString, bool IsCaseInsensetive)
		{
			return Regex.Replace (SourceString, Regex.Escape(SearchString), ReplaceString, (IsCaseInsensetive==true)?RegexOptions.IgnoreCase: RegexOptions.None);  
		}
		public static String[] SplitString(String SourceString, String SearchString, bool IsCaseInsensetive)
		{
			return Regex.Split (SourceString, Regex.Escape(SearchString), (IsCaseInsensetive==true)?RegexOptions.IgnoreCase: RegexOptions.None);  
		}
		public static  bool IsSubStringExist(String StringToSearch, String SubStringToLookFor, bool IsCaseInsensitive)
		{
			return Regex.IsMatch(StringToSearch, Regex.Escape(SubStringToLookFor), (IsCaseInsensitive==true)?RegexOptions.IgnoreCase: RegexOptions.None);
		}
	}
}
