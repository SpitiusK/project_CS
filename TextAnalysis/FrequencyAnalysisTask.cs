namespace TextAnalysis;

static class FrequencyAnalysisTask
{
    public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
    {
        var result = new Dictionary<string, string>();
        var dictionaryFrequency = new Dictionary<string, int>();
        foreach (var sentece in text)
        {
            var wordsArray = sentece.ToArray();
            for (var i = 0; i < wordsArray.Length; i++)
            {
                if (i + 2 <= wordsArray.Length)
                {
                    var biGramsStringKey = string.Join(' ', wordsArray[i..(i + 2)]);
                    if (i + 3 <= wordsArray.Length)
                    {
                        var threeGramStringKey = string.Join(' ', wordsArray[i..(i + 3)]);
                        AddInFrequencyDictionary(wordsArray, dictionaryFrequency, threeGramStringKey);
                    }
                    AddInFrequencyDictionary(wordsArray, dictionaryFrequency, biGramsStringKey);
                }
                else break;
            }
        }
        AddFrequencyNGramInResultDictionary(dictionaryFrequency, result);
        return result;
    }
    
    private static void AddInFrequencyDictionary(string[] wordsArray, Dictionary<string, int> dictionaryFrequency, 
                                                string stringKey)
    {
                if (!dictionaryFrequency.TryAdd(stringKey, 1))
                    dictionaryFrequency[stringKey] += 1;
    }

    private static void AddFrequencyNGramInResultDictionary(Dictionary<string, int> dictionaryFrequency, 
                                                            Dictionary<string, string> result)
    {
        foreach (var nGram in dictionaryFrequency)
        {
            var nGramArray = nGram.Key.Split(' ');
            var resultKey = string.Join(' ', nGramArray, 0, nGramArray.Length - 1);
            var resultTryVelue = nGramArray[nGramArray.Length - 1];

            if (!result.TryAdd(resultKey, resultTryVelue))
                if (dictionaryFrequency[$"{resultKey} {result[resultKey]}"] < dictionaryFrequency[nGram.Key])
                    result[resultKey] = resultTryVelue;
                else if (dictionaryFrequency[$"{resultKey} {result[resultKey]}"] == dictionaryFrequency[nGram.Key]
                         && string.CompareOrdinal(result[resultKey], resultTryVelue) > 0)
                        result[resultKey] = resultTryVelue;
        }
    }
}