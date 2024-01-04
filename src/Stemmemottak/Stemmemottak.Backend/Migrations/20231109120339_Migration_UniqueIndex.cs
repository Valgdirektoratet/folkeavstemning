using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resultat.Api.Migrations
{
    /// <inheritdoc />
    public partial class Migration_UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_krypterte_stemmer_data_signatur",
                table: "krypterte_stemmer",
                columns: new[] { "data", "signatur" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_krypterte_stemmer_data_signatur",
                table: "krypterte_stemmer");
        }
    }
}
