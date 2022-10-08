using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace FullTextSearchSamples.Services.Impl;

public class FullTextIndexSearcher
{
    private Dictionary<string, HashSet<int>> _index = new Dictionary<string, HashSet<int>>();
    private List<string> _documents = new List<string>(100);
    private Lexer _lexer = new Lexer();

    public FullTextIndexSearcher(){}

    public void AddStringToIndex(string document)
    {
        int documentId = _documents.Count;
        foreach (var token in _lexer.GetTokens(document))
        {
            if (_index.TryGetValue(token, out HashSet<int> hashset))
            {
                hashset.Add(documentId);
            }
            else
            {
                _index.Add(token, new HashSet<int> { documentId });
            }
        }
        _documents.Add(document);
    }

    public IEnumerable<string> Search(string word)
    {
        var foundDocumentIds = FindDocuments(word);

        foreach (var documentId in foundDocumentIds)
        {
            foreach(var match in FindInDocument(word, _documents[documentId]))
            {
                yield return match;
            }
        }
    }

    private IEnumerable<int> FindDocuments(string word)
    {
        word = word.ToLowerInvariant();
        if (_index.TryGetValue(word, out HashSet<int> hashSet))
        {
            return hashSet;
        }
        return Enumerable.Empty<int>();
    }

    private IEnumerable<string> FindInDocument(string word, string document)
    {
        int pos = 0;
        while (true)
        {
            pos = document.IndexOf(word, pos, StringComparison.InvariantCultureIgnoreCase);
            if (pos >= 0)
            {
                yield return Prettify(document, pos);
            }
            else
                break;
            pos++;
        }
    }

    private string Prettify(string document, int pos)
    {
        var start = Math.Max(0, pos - 50);
        int end = Math.Min(start + 100, document.Length - 1);
        return $"{(start == 0 ? "" : "...")}{document.Substring(start, end - start)}{(end == document.Length - 1 ? "" : "...")}";
    }
}
