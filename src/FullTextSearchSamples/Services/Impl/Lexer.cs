namespace FullTextSearchSamples.Services.Impl;

public class Lexer
{
    public IEnumerable<string> GetTokens(string text)
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
        if (start >=0)
            yield return GetToken(text, end, start);
    }

    private string GetToken(string text, int end, int start)
    {
        return text.Substring(start, end - start).Normalize().ToLowerInvariant();
    }
}
