using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRUNStatsCommon.Migrations
{
    /// <inheritdoc />
    public partial class FIOTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedFIO",
                schema: "prun",
                table: "Companies",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedFIO",
                schema: "prun",
                table: "Companies");
        }
    }
}
