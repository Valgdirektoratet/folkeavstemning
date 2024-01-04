using Microsoft.EntityFrameworkCore;

namespace Folkeavstemning.Api.Database;

public class FolkeavstemningContext : DbContext
{
    public FolkeavstemningContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Stemmegivning> Stemmegivninger { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(FolkeavstemningContext).Assembly);

}
