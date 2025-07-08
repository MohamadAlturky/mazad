using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazad.Migrations
{
    /// <inheritdoc />
    public partial class addlocationForTheOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "dbo",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Offers_RegionId",
                schema: "dbo",
                table: "Offers",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Regions_RegionId",
                schema: "dbo",
                table: "Offers",
                column: "RegionId",
                principalSchema: "dbo",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Regions_RegionId",
                schema: "dbo",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_RegionId",
                schema: "dbo",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "dbo",
                table: "Offers");
        }
    }
}
