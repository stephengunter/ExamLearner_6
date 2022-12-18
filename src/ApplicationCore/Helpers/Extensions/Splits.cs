using System;

namespace ApplicationCore.Helpers;

public static class SplitHelpers
{
	public static List<string> SplitToList(this string str, char splitBy = ',')
	{
		if (String.IsNullOrEmpty(str)) return new List<string>();
		return str.Split(splitBy).ToList();
	}
	public static List<int> SplitToIntList(this string str, char splitBy = ',')
	{
		if (String.IsNullOrEmpty(str)) return new List<int>();
		return str.Split(splitBy).Select(s => s.ToInt()).ToList();
	}
	
	public static List<int> SplitToIds(this string str, char splitBy = ',')
	{
		var list = str.SplitToIntList(splitBy);

		if (list.HasItems()) list.RemoveAll(item => item == 0);
		return list;
	}

	public static string JoinToString(this IEnumerable<string>? list)
	{
		if (list.IsNullOrEmpty()) return string.Empty;
		return String.Join(",", list!);
	}

	public static string JoinToStringIntegers(this List<int> list, bool greaterThanZero = false)
	{
		if (greaterThanZero) list = list.Where(id => id > 0).ToList();
		return String.Join(",", list.Select(x => x.ToString()));
	}
}
