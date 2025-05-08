using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesReviews1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Reviews_ReviewId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ReviewId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Reviews");

            migrationBuilder.CreateTable(
                name: "ReviewTag",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTag", x => new { x.ReviewId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ReviewTag_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTag_TagId",
                table: "ReviewTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewTag");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewId",
                table: "Tags",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Comments",
                table: "Reviews",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "Tags",
                table: "Reviews",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ReviewId",
                table: "Tags",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Reviews_ReviewId",
                table: "Tags",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
