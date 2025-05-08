using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamesReviews1.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "HashedPassword", "Role", "UserName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "admin@example.com", "$2a$11$EmyLvPJ/8B1rAJxphPEcA..boxC3dGYKzFvjedpL.Eq3hEiVT8Vo.", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
