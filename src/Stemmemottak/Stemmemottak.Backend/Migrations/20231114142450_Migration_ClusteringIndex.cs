using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resultat.Api.Migrations
{
    /// <inheritdoc />
    public partial class Migration_ClusteringIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_krypterte_stemmer_id",
                table: "krypterte_stemmer",
                column: "id");

            migrationBuilder.Sql("cluster krypterte_stemmer using ix_krypterte_stemmer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_krypterte_stemmer_id",
                table: "krypterte_stemmer");
        }
    }
}
