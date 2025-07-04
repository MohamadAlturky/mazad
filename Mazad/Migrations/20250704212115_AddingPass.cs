using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazad.Migrations
{
    /// <inheritdoc />
    public partial class AddingPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                schema: "dbo",
                table: "Users");
        }
    }
}
