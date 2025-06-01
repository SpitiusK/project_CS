namespace TextAnalysis;

static class TextGeneratorTask
{
    public static string ContinuePhrase(
        Dictionary<string, string> nextWords,
        string phraseBeginning,
        int wordsCount)
    {
        var phraseBeginningList = phraseBeginning.Split(' ').ToList();
        for (var i = 0; i < wordsCount; i++)
        {
            if ((phraseBeginningList.Count > 1 && GetPhraseBeginning(nextWords, phraseBeginningList, 2)) 
                || (phraseBeginningList.Count > 0 && GetPhraseBeginning(nextWords, phraseBeginningList, 1)))
            {
            }
            else
            {
                break;
            }
        }
        var result = string.Join(" ", phraseBeginningList);
        return result;
    }

    private static bool GetPhraseBeginning(Dictionary<string, string> nextWords, 
                    List<string> phraseBeginningList, int wordsCount)
    {
        var phraseBeginningArray = phraseBeginningList.ToArray();
        var endPhraseBeginning = string.Join(" ", phraseBeginningArray, 
            phraseBeginningArray.Length - wordsCount, wordsCount);
        if (nextWords.TryGetValue(endPhraseBeginning, out var phraseBeginning))
        {
            phraseBeginningList.Add(phraseBeginning);
            return true;
        }
        return false;
    }
}