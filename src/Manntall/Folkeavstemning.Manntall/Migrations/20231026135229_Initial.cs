using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Manntall.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "person",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    manntallsnummer = table.Column<string>(type: "text", nullable: true),
                    folkeavstemning_id = table.Column<string>(type: "text", nullable: false),
                    identifikasjonsnummer = table.Column<string>(type: "text", nullable: false),
                    fornavn = table.Column<string>(type: "text", nullable: false),
                    mellomnavn = table.Column<string>(type: "text", nullable: true),
                    etternavn = table.Column<string>(type: "text", nullable: false),
                    kjønn = table.Column<int>(type: "integer", nullable: false),
                    fødselsdato = table.Column<LocalDate>(type: "date", nullable: false),
                    folkeregister_status = table.Column<int>(type: "integer", nullable: false),
                    bostedsadresse_adressenavn = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_adressenummer = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_adressetillegsnavn = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_postnummer = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_poststed = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_kommunenummer = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_bruksenhetsnummer = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_bruksenhetstype = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_flyttedato = table.Column<LocalDate>(type: "date", nullable: true),
                    bostedsadresse_adressegradering = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_grunnkrets = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_kirkekrets = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_skolekrets = table.Column<string>(type: "text", nullable: true),
                    bostedsadresse_stemmekrets = table.Column<string>(type: "text", nullable: true),
                    postboks_adresse_adressegradering = table.Column<string>(type: "text", nullable: true),
                    postboks_adresse_postboks = table.Column<string>(type: "text", nullable: true),
                    postboks_adresse_eier = table.Column<string>(type: "text", nullable: true),
                    postboks_adresse_postnummer = table.Column<string>(type: "text", nullable: true),
                    postboks_adresse_poststed = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_adressegradering = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_adressenavn = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_adressenummer = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_bruksenhetsnummer = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_co_adressenavn = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_postnummer = table.Column<string>(type: "text", nullable: true),
                    vegadresse_for_post_poststed = table.Column<string>(type: "text", nullable: true),
                    digital_kontakt_språk = table.Column<string>(type: "text", nullable: true),
                    digital_kontakt_mobiltelefonnummer = table.Column<string>(type: "text", nullable: true),
                    digital_kontakt_epostadresse = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_person_folkeavstemning_id_identifikasjonsnummer",
                table: "person",
                columns: new[] { "folkeavstemning_id", "identifikasjonsnummer" })
                .Annotation("Npgsql:IndexInclude", new[] { "manntallsnummer" });

            migrationBuilder.CreateIndex(
                name: "ix_person_identifikasjonsnummer",
                table: "person",
                column: "identifikasjonsnummer")
                .Annotation("Npgsql:IndexInclude", new[] { "manntallsnummer", "folkeavstemning_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "person");
        }
    }
}
