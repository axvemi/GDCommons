using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;

namespace Axvemi.Commons;

public static class GDUtils
{
	public static string GetStrippedBbCode(string text) => Regex.Replace(text, "\\[.+?\\]", "");

	public static List<Node> GetRecursiveChildren(Node node, bool includeRoot = false)
	{
		List<Node> allChildren = new();
		if (includeRoot)
		{
			allChildren.Add(node);
		}

		foreach (Node child in node.GetChildren())
		{
			allChildren.Add(child);
			allChildren.AddRange(GetRecursiveChildren(child));
		}
		
		return allChildren;
	}
}