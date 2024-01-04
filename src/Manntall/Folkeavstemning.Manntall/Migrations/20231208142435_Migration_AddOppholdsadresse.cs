using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manntall.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Migration_AddOppholdsadresse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_adressegradering",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_adressenavn",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_adressenummer",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_adressetillegsnavn",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_bruksenhetsnummer",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_bruksenhetstype",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_kommunenummer",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_postnummer",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "oppholdsadresse_poststed",
                table: "person",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "oppholdsadresse_adressegradering",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_adressenavn",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_adressenummer",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_adressetillegsnavn",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_bruksenhetsnummer",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_bruksenhetstype",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_kommunenummer",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_postnummer",
                table: "person");

            migrationBuilder.DropColumn(
                name: "oppholdsadresse_poststed",
                table: "person");
        }
    }
}
