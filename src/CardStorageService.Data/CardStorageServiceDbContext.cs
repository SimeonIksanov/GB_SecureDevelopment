﻿using Microsoft.EntityFrameworkCore;

namespace CardStorageService.Data;

public class CardStorageServiceDbContext : DbContext
{
    public CardStorageServiceDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Card> Cards { get; set; }
}