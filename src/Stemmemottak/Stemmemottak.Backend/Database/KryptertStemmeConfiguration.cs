using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Resultat.Api.Database;

public class KryptertStemmeConfiguration : IEntityTypeConfiguration<KryptertStemme>
{
    public void Configure(EntityTypeBuilder<KryptertStemme> builder)
    {
        builder.ToTable("krypterte_stemmer");
        builder.HasKey(e => e.Id);
        builder.HasIndex(x => x.Id);

        builder.HasIndex(x => x.FolkeavstemningId)
            .IncludeProperties(x=> new {x.Data, x.Signatur});

        builder.HasIndex(x => new { x.Data, x.Signatur }).IsUnique();
    }
}
