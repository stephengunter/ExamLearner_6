using System;

namespace ApplicationCore.Helpers;
public static class CommonHelpers
{
	public static string GetString(this string? text) => String.IsNullOrEmpty(text) ? "" : text.ToString();
	public static bool HasValue(this string text) => !String.IsNullOrEmpty(text);
	public static bool EqualTo(this string val, string other) => String.Compare(val, other, true) == 0;
	public static bool CaseInsensitiveContains(this string text, string value)
	{
		StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase;
		if (text == null) return false;
		if (value == null) return false;
		return text.IndexOf(value, stringComparison) >= 0;
	}
	public static IList<string> GetKeywords(this string input)
	{
		if (String.IsNullOrWhiteSpace(input) || String.IsNullOrEmpty(input)) return new List<string>();

		return input.Split(new string[] { "-", " ", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	public static string ReverseString(this string str)
		=> String.IsNullOrEmpty(str) ? string.Empty : new string(str.ToCharArray().Reverse().ToArray());
	public static int ToInt(this string str)
	{
		int value = 0;
		if (!int.TryParse(str, out value)) value = 0;

		return value;
	}
	public static decimal ToDecimal(this string str)
	{
		decimal value;
		if (!Decimal.TryParse(str, out value)) value = 0;

		return value;
	}

	public static bool ToBoolean(this string str)
	{
		if (String.IsNullOrEmpty(str)) return false;

		return (str.ToLower() == "true");
	}
	public static bool ToBoolean(this int val) => val > 0;

	public static int ToInt(this bool val) => val ? 1 : 0;
	
	public static bool HasDuplicate(this string[] vals) => vals.Length != vals.Distinct().Count();
}
