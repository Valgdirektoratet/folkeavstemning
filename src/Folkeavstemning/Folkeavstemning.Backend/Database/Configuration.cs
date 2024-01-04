using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Folkeavstemning.Api.Database;

public class StemmegivningConfig : IEntityTypeConfiguration<Stemmegivning>
{
    public void Configure(EntityTypeBuilder<Stemmegivning> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.FolkeavstemningId).IncludeProperties(x => x.Manntallsnummer);
        builder.HasIndex(x => new { x.FolkeavstemningId, x.Manntallsnummer }).IsUnique();
    }
}
