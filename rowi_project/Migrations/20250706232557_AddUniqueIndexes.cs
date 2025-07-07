using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rowi_project.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Companies_FullName",
                table: "Companies",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_RepEmail",
                table: "Companies",
                column: "RepEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_RepPhoneNumber",
                table: "Companies",
                column: "RepPhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Companies_FullName",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_RepEmail",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_RepPhoneNumber",
                table: "Companies");
        }
    }
}
