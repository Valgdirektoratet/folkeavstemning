using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manntall.Backend.Database;
#pragma warning disable CS8603 // Possible null reference return.

public class PersonConfiguration : IEntityTypeConfiguration<PersonEntity>
{
    public void Configure(EntityTypeBuilder<PersonEntity> builder)
    {
        builder.ToTable("person");
        builder.HasKey(person => person.Id);

        builder.HasIndex(p => new { p.FolkeavstemningId, p.Identifikasjonsnummer })
            .IncludeProperties(entity => entity.Manntallsnummer);

        builder.HasIndex(p => new { p.Identifikasjonsnummer })
            .IncludeProperties(entity => new { entity.Manntallsnummer, entity.FolkeavstemningId, });

        builder.OwnsOne(x => x.Bostedsadresse);
        builder.OwnsOne(x => x.PostboksAdresse);
        builder.OwnsOne(x => x.VegadresseForPost);

        builder.OwnsOne(x => x.DigitalKontakt);
        builder.OwnsOne(x => x.Oppholdsadresse);
    }
}
#pragma warning restore CS8603 // Possible null reference return.
