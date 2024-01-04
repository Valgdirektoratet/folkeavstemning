using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folkeavstemning.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stemmegivninger",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    folkeavstemning_id = table.Column<string>(type: "text", nullable: false),
                    manntallsnummer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stemmegivninger", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stemmegivninger");
        }
    }
}
