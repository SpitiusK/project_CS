namespace Passwords;

public class CaseAlternatorTask
{
	//Тесты будут вызывать этот метод
	public static List<string> AlternateCharCases(string lowercaseWord)
	{
		var result = new List<string>();
		AlternateCharCases(lowercaseWord.ToCharArray(), 0, result);
		return result;
	}

	static void AlternateCharCases(char[] word, int startIndex, List<string> result)
	{
		FillingResultList(word, startIndex, result);
		result.Sort(StringComparer.Ordinal);
		result.Reverse();
	}

	static void FillingResultList(char[] word, int startIndex, List<string> result)
	{
		var stringWord = string.Join("", word);
		if (!result.Contains(stringWord))
				result.Add(stringWord);
		if (startIndex == word.Length) return;
		if (char.IsLetter(word[startIndex]))
		{
			if (char.IsLower(word[startIndex]))
			{
				word[startIndex] = char.ToUpperInvariant(word[startIndex]);
				FillingResultList(word, startIndex + 1, result);
			}

			if (char.IsUpper(word[startIndex]))
			{
				word[startIndex] = char.ToLower(word[startIndex]);
				FillingResultList(word, startIndex + 1, result);
			}
		}
		else
			FillingResultList(word, startIndex + 1, result);
	}
}