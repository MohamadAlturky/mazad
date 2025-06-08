using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mazad.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddingAVEForCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameArabic = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NameEnglish = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAttributes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    DynamicAttributeId = table.Column<int>(type: "int", nullable: false),
                    AttributeValueType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "dbo",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryAttributes_DynamicAttributes_DynamicAttributeId",
                        column: x => x.DynamicAttributeId,
                        principalSchema: "dbo",
                        principalTable: "DynamicAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_CategoryId",
                schema: "dbo",
                table: "CategoryAttributes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_DynamicAttributeId",
                schema: "dbo",
                table: "CategoryAttributes",
                column: "DynamicAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicAttributes_NameArabic",
                schema: "dbo",
                table: "DynamicAttributes",
                column: "NameArabic");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicAttributes_NameEnglish",
                schema: "dbo",
                table: "DynamicAttributes",
                column: "NameEnglish");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAttributes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DynamicAttributes",
                schema: "dbo");
        }
    }
}
