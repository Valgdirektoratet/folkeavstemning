using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folkeavstemning.Api.Migrations
{
    /// <inheritdoc />
    public partial class Migration_UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_stemmegivninger_folkeavstemning_id",
                table: "stemmegivninger",
                column: "folkeavstemning_id")
                .Annotation("Npgsql:IndexInclude", new[] { "manntallsnummer" });

            migrationBuilder.CreateIndex(
                name: "ix_stemmegivninger_folkeavstemning_id_manntallsnummer",
                table: "stemmegivninger",
                columns: new[] { "folkeavstemning_id", "manntallsnummer" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stemmegivninger_folkeavstemning_id",
                table: "stemmegivninger");

            migrationBuilder.DropIndex(
                name: "ix_stemmegivninger_folkeavstemning_id_manntallsnummer",
                table: "stemmegivninger");
        }
    }
}
