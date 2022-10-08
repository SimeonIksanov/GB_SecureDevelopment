using FullTextSearchSamples.Entity;
using Microsoft.EntityFrameworkCore;

namespace FullTextSearchSamples;

public class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }
}
