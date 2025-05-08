using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesReviews1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContentId",
                table: "Reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ModeratorId",
                table: "Reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ModeratorId",
                table: "Reports",
                column: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_ModeratorId",
                table: "Reports",
                column: "ModeratorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_ModeratorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ModeratorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ModeratorId",
                table: "Reports");
        }
    }
}
