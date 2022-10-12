using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace FullTextSearchSamples.Services.Impl;

public class FullTextIndexAdvancedSearcher
{
    private Dictionary<string, HashSet<(int, int)>> _index = new Dictionary<string, HashSet<(int, int)>>();
    private List<string> _documents = new List<string>(100);


    public FullTextIndexAdvancedSearcher() { }


    public void AddStringToIndex(string document)
    {
        int documentId = _documents.Count;
        foreach (var tokenPos in GetTokenAndPos(document))
        {
            if (_index.TryGetValue(tokenPos.Item1, out HashSet<(int, int)> hashset))
            {
                hashset.Add((documentId, tokenPos.Item2));
            }
            else
            {
                _index.Add(tokenPos.Item1, new HashSet<(int, int)> { (documentId, tokenPos.Item2) });
            }
        }
        _documents.Add(document);
    }

    public IEnumerable<string> Search(string word)
    {
        var wordCoords = FindDocuments(word);

        foreach (var wordCoord in wordCoords)
        {
            yield return Prettify(_documents[wordCoord.Item1], wordCoord.Item2);
        }
    }


    private IEnumerable<(int, int)> FindDocuments(string word)
    {
        word = word.ToLowerInvariant();
        if (_index.TryGetValue(word, out HashSet<(int, int)> hashSet))
        {
            return hashSet;
        }
        return Enumerable.Empty<(int, int)>();
    }

    private string Prettify(string document, int pos)
    {
        var start = Math.Max(0, pos - 50);
        int end = Math.Min(start + 100, document.Length - 1);
        return $"{(start == 0 ? "" : "...")}{document.Substring(start, end - start)}{(end == document.Length - 1 ? "" : "...")}";
    }

    private IEnumerable<(string, int)> GetTokenAndPos(string text)
    {
        int end, start = -1;
        for (end = 0; end < text.Length; end++)
        {
            if (char.IsLetterOrDigit(text[end]))
            {
                if (start == -1)
                    start = end;
            }
            else
            {
                if (start >= 0)
                {
                    yield return GetToken(text, end, start);
                    start = -1;
                }
            }

        }
        if (start >= 0)
            yield return GetToken(text, end, start);
    }

    private (string, int) GetToken(string text, int end, int start)
    {
        var word = text.Substring(start, end - start).Normalize().ToLowerInvariant();
        return (word, start);
    }
}
