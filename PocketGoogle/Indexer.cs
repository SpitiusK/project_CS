using System;
using System.Collections.Generic;

namespace PocketGoogle;

public class Indexer : IIndexer
{
    private readonly Dictionary<string, Dictionary<int, List<int>>> wordIndex = new();
    private readonly Dictionary<int, HashSet<string>> documentWords = new();
    private readonly HashSet<char> separators = new(new[] { ' ', '.', ',', '!', '?', ':', '-', '\r', '\n' });

    public void Add(int id, string documentText)
    {
        var words = new HashSet<string>();
        var wordPositions = GetWordPositions(documentText);
        UpdateIndex(id, wordPositions, words);
        documentWords[id] = words;
    }


    private Dictionary<string, List<int>> GetWordPositions(string documentText)
    {
        var wordPositions = new Dictionary<string, List<int>>();
        int start = -1;

        for (int i = 0; i <= documentText.Length; i++)
        {
            bool isSeparator = i < documentText.Length 
                               && separators.Contains(documentText[i]);

            if (isSeparator || i == documentText.Length)
            {
                ProcessCurrentWord(documentText, i, ref start, wordPositions);
            }
            else if (start == -1)
            {
                start = i;
            }
        }

        return wordPositions;
    }

    private void ProcessCurrentWord(
        string text, 
        int currentIndex, 
        ref int wordStart, 
        Dictionary<string, List<int>> wordPositions)
    {
        if (wordStart == -1) return;

        var word = text.Substring(wordStart, currentIndex - wordStart);
        if (!wordPositions.ContainsKey(word))
            wordPositions[word] = new List<int>();
    
        wordPositions[word].Add(wordStart);
        wordStart = -1;
    }

    private void UpdateIndex(
        int documentId, 
        Dictionary<string, List<int>> wordPositions, 
        HashSet<string> words)
    {
        foreach (var (word, positions) in wordPositions)
        {
            if (!wordIndex.ContainsKey(word))
                wordIndex[word] = new Dictionary<int, List<int>>();
        
            wordIndex[word][documentId] = positions;
            words.Add(word);
        }
    }

    public List<int> GetIds(string word)
    {
        if (wordIndex.TryGetValue(word, out var docs))
            return new List<int>(docs.Keys);
        return new List<int>();
    }

    public List<int> GetPositions(int id, string word)
    {
        if (wordIndex.TryGetValue(word, out var docs) && docs.TryGetValue(id, out var positions))
            return new List<int>(positions);
        return new List<int>();
    }

    public void Remove(int id)
    {
        if (!documentWords.TryGetValue(id, out var words))
            return;

        foreach (var word in words)
        {
            if (wordIndex.TryGetValue(word, out var docs))
            {
                docs.Remove(id);
                if (docs.Count == 0)
                    wordIndex.Remove(word);
            }
        }

        documentWords.Remove(id);
    }
}