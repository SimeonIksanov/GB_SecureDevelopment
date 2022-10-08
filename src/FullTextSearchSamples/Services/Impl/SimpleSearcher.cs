using static System.Net.Mime.MediaTypeNames;

namespace FullTextSearchSamples.Services.Impl;

public class SimpleSearcher
{
    public IEnumerable<string> Search(string word, IEnumerable<string> documents)
    {
        foreach (var document in documents)
        {
            int pos = 0;
            while(true)
            {
                pos = document.IndexOf(word, pos);
                if (pos >= 0)
                {
                    yield return Prettify(document, pos);
                    pos++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public string Prettify(string document, int pos)
    {
        var start = Math.Max(0, pos - 50);
        int end = Math.Min(start + 100, document.Length - 1);
        return $"{(start == 0 ? "" : "...")}{document.Substring(start, end - start)}{(end == document.Length - 1 ? "" : "...")}";
    }
}
