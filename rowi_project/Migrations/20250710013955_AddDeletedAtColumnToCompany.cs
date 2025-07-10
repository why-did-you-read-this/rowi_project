using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rowi_project.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedAtColumnToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Companies",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Companies");
        }
    }
}
