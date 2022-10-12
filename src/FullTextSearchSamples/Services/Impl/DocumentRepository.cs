using FullTextSearchSamples.Entity;

namespace FullTextSearchSamples.Services.Impl;

public class DocumentRepository : IDocumentRepository
{
    private readonly DocumentDbContext _context;

    public DocumentRepository(DocumentDbContext context)
    {
        _context = context;
    }

    public void LoadDocuments()
    {
        using var streamReader = new StreamReader(Path.Combine(AppContext.BaseDirectory, "data.txt"));
        while(!streamReader.EndOfStream)
        {
            var document = streamReader
                .ReadLine()
                .Split('\t');
            if (document.Length > 1 && int.TryParse(document[0], out int id))
            {
                _context.Documents.Add(new Document
                {
                    Id = id,
                    Content = document[1]
                });
                _context.SaveChanges();
            }
        }
    }
}
