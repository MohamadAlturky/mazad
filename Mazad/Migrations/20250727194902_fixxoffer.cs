using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazad.Migrations
{
    /// <inheritdoc />
    public partial class fixxoffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfViews",
                schema: "dbo",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfViews",
                schema: "dbo",
                table: "Offers");
        }
    }
}
