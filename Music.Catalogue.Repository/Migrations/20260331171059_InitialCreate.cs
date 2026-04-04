using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music.Catalogue.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Songs",
                columns: table => new
                {
                    SpotifyId = table.Column<string>(type: "text", nullable: false),
                    Titolo = table.Column<string>(type: "text", nullable: false),
                    Artista = table.Column<string>(type: "text", nullable: false),
                    Album = table.Column<string>(type: "text", nullable: false),
                    DataUscita = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Durata = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.SpotifyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Songs");
        }
    }
}
