using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Autocomplete;

internal class AutocompleteTask
{
	/// <returns>
	/// Возвращает первую фразу словаря, начинающуюся с prefix.
	/// </returns>
	/// <remarks>
	/// Эта функция уже реализована, она заработает, 
	/// как только вы выполните задачу в файле LeftBorderTask
	/// </remarks>
	public static string FindFirstByPrefix(IReadOnlyList<string> phrases, string prefix)
	{
		var index = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
		if (index < phrases.Count && phrases[index].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
			return phrases[index];
            
		return null;
	}

	/// <returns>
	/// Возвращает первые в лексикографическом порядке count (или меньше, если их меньше count) 
	/// элементов словаря, начинающихся с prefix.
	/// </returns>
	/// <remarks>Эта функция должна работать за O(log(n) + count)</remarks>
	public static string[] GetTopByPrefix(IReadOnlyList<string> phrases, string prefix, int count)
	{																			
		// тут стоит использовать написанный ранее класс LeftBorderTask
		var leftBorderIndex = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count);
		var wordList = new List<string>();
		for (var i = 1; i <= count; i++)
		{
			if (leftBorderIndex + i < phrases.Count && phrases[leftBorderIndex + i].StartsWith(prefix))
			{
				wordList.Add(phrases[leftBorderIndex + i]);
			}
		}
		return wordList.ToArray(); 
	}

	/// <returns>
	/// Возвращает количество фраз, начинающихся с заданного префикса
	/// </returns>
	public static int GetCountByPrefix(IReadOnlyList<string> phrases, string prefix)
	{
		var leftBorderIndex = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count);
		var rightBorderIndex = RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, phrases.Count);
		
		return rightBorderIndex - leftBorderIndex - 1;
	}
}
[TestFixture]
public class AutocompleteTests
{
	[TestCase(new[] { "apple", "applet", "application", "banana" }, "app", 2, new[] { "apple", "applet" })]
	[TestCase(new[] { "apple", "applet", "application", "banana" }, "app", 5, new[] { "apple", "applet", "application" })]
	[TestCase(new[] { "banana", "cherry" }, "app", 3, new string[0])]
	[TestCase(new[] { "Applet", "apple", "application" }, "app", 3, new[] { "apple", "application" })]
	[TestCase(new[] { "app" }, "app", 1, new[] { "app" })]
	[TestCase(new string[0], "app", 1, new string[0])]
	public void GetTopByPrefix_ReturnsCorrectResults
		(IReadOnlyList<string> phrases, string prefix, int count, string[] expected)
	{
		var result = AutocompleteTask.GetTopByPrefix(phrases, prefix, count);
		CollectionAssert.AreEqual(expected, result);
	}

	[TestCase(new[] { "apple", "applet", "application" }, "app", 3)]
	[TestCase(new[] { "banana", "cherry" }, "app", 0)]
	[TestCase(new[] { "apple" }, "app", 1)]
	[TestCase(new[] { "a", "ab", "abc" }, "a", 3)]
	public void GetCountByPrefix_ReturnsCorrectCount
		(IReadOnlyList<string> phrases, string prefix, int expectedCount)
	{
		var result = AutocompleteTask.GetCountByPrefix(phrases, prefix);
		Assert.AreEqual(expectedCount, result);
	}
}

