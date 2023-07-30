using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class Schemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Library");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Security");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "Books",
                newSchema: "Library");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Authors",
                newSchema: "Library");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Security",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Books",
                schema: "Library",
                newName: "Books");

            migrationBuilder.RenameTable(
                name: "Authors",
                schema: "Library",
                newName: "Authors");
        }
    }
}
