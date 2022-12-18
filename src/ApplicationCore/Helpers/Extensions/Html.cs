using System.Text.RegularExpressions;

namespace ApplicationCore.Helpers;

public static class HtmlHelpers
{
	public static string ReplaceNewLine(this string text, string replacement = "<br>") => String.IsNullOrEmpty(text) ? text : Regex.Replace(text, @"\r\n?|\n", replacement);

	public static string ReplaceBrToNewLine(this string text) => String.IsNullOrEmpty(text) ? text : text.Replace("<br>", System.Environment.NewLine);

	public static string RemoveScriptTags(this string htmlString)
	{
		if (String.IsNullOrEmpty(htmlString)) return "";
		//移除  javascript code.
		htmlString = Regex.Replace(htmlString, @"<script[\d\D]*?>[\d\D]*?</script>", String.Empty);

		return htmlString;


	}
	public static string RemoveHtmlTags(this string htmlString)
	{
		//移除html tag.
		if (String.IsNullOrEmpty(htmlString)) return "";
		var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		htmlString = regexCss.Replace(htmlString, string.Empty);

		if (String.IsNullOrEmpty(htmlString)) return htmlString;
		string htmlTagPattern = @"<[^>]+>|&nbsp;";
		htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);

		if (String.IsNullOrEmpty(htmlString)) return htmlString;
		htmlString = Regex.Replace(htmlString, @"\s{2,}", " ");

		if (String.IsNullOrEmpty(htmlString)) return htmlString;
		htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);


		return htmlString;



		//htmlString = Regex.Replace(htmlString, @"<[^>]*>", String.Empty);

		//return htmlString;
	}
	public static string RemoveSciptAndHtmlTags(this string htmlString)
	{
		if (String.IsNullOrEmpty(htmlString)) return "";
		htmlString = RemoveScriptTags(htmlString);

		return RemoveHtmlTags(htmlString).Trim();
	}

	public static bool HasHtmlTag(this string text)
	{
		if (String.IsNullOrEmpty(text)) return false;

		var htmlRegex = new Regex(@"<(\s*[(\/?)\w+]*)");
		return htmlRegex.IsMatch(text);
	}
}
