using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRUNStatsCommon.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "prun");

            migrationBuilder.CreateTable(
                name: "Corporations",
                schema: "prun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorporationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CorporationCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FirstImportedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Planets",
                schema: "prun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NaturalId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstImportedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "prun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstImportedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "prun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CorporationId = table.Column<int>(type: "int", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Faction = table.Column<int>(type: "int", nullable: true),
                    FirstImportedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Corporations_CorporationId",
                        column: x => x.CorporationId,
                        principalSchema: "prun",
                        principalTable: "Corporations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Companies_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "prun",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bases",
                schema: "prun",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    FirstImportedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAtUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bases_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "prun",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bases_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalSchema: "prun",
                        principalTable: "Planets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bases_CompanyId",
                schema: "prun",
                table: "Bases",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Bases_PlanetId",
                schema: "prun",
                table: "Bases",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CorporationId",
                schema: "prun",
                table: "Companies",
                column: "CorporationId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserId",
                schema: "prun",
                table: "Companies",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bases",
                schema: "prun");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "prun");

            migrationBuilder.DropTable(
                name: "Planets",
                schema: "prun");

            migrationBuilder.DropTable(
                name: "Corporations",
                schema: "prun");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "prun");
        }
    }
}
