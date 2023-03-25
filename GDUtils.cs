using System.Text.RegularExpressions;

namespace Axvemi.Commons;

public static class GDUtils
{
	public static string GetStrippedBbCode(string text) => Regex.Replace(text, "\\[.+?\\]", "");
}