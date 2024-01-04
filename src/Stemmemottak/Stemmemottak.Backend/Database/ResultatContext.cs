using Microsoft.EntityFrameworkCore;

namespace Resultat.Api.Database;

public class ResultatContext : DbContext
{
    public ResultatContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResultatContext).Assembly);

    public DbSet<KryptertStemme> Stemmer { get; set; } = default!;

}