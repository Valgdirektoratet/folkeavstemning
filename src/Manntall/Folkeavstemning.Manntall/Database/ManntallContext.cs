using Microsoft.EntityFrameworkCore;

namespace Manntall.Backend.Database;

public class ManntallContext : DbContext
{
    public ManntallContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManntallContext).Assembly);

    public DbSet<PersonEntity> Personer { get; set; } = default!;
}
