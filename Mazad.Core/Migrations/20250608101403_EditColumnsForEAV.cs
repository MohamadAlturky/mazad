using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazad.Core.Migrations
{
    /// <inheritdoc />
    public partial class EditColumnsForEAV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeValueType",
                schema: "dbo",
                table: "CategoryAttributes");

            migrationBuilder.AddColumn<int>(
                name: "AttributeValueType",
                schema: "dbo",
                table: "DynamicAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeValueType",
                schema: "dbo",
                table: "DynamicAttributes");

            migrationBuilder.AddColumn<int>(
                name: "AttributeValueType",
                schema: "dbo",
                table: "CategoryAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
