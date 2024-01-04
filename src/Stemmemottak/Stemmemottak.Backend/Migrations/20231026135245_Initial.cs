using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resultat.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "krypterte_stemmer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folkeavstemning_id = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    signatur = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_krypterte_stemmer", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_krypterte_stemmer_folkeavstemning_id",
                table: "krypterte_stemmer",
                column: "folkeavstemning_id")
                .Annotation("Npgsql:IndexInclude", new[] { "data", "signatur" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "krypterte_stemmer");
        }
    }
}
