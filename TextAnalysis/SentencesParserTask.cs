namespace TextAnalysis;

static class SentencesParserTask
{
    public static List<List<string>> ParseSentences(string text)
    {
        var sentencesList = new List<List<string>>();
        var siparetare = new [] { '.', '!', '?', ';', ':', '(', ')' };
        var splitText = text.Split(siparetare, StringSplitOptions.RemoveEmptyEntries);

        foreach (var sentence in splitText)
        {
            var wordsList = ParseOneSentence(sentence);
            if (wordsList.Count > 0)
                sentencesList.Add(wordsList);
        }
        return sentencesList;
    }

    private static List<string> ParseOneSentence(string sentence)
    {
        var word = "";
        var oneSentenceList = new List<string>();
        for (var i = 0; i < sentence.Length; i++)
        {
            if (char.IsLetter(sentence[i]) || sentence[i] == '\'')
                word += char.ToLower(sentence[i]);
            else if (word != "")
            {
                oneSentenceList.Add(word);
                word = "";
            }

            if (i == sentence.Length - 1 && word != "")
                oneSentenceList.Add(word);
        }
        return oneSentenceList;
    }
}