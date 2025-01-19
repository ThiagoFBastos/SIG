using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdminInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios_admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    SalString = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_admins", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "usuarios_admins",
                columns: new[] { "Id", "Email", "PasswordHash", "SalString" },
                values: new object[] { new Guid("4c675501-acea-4287-9539-e87d4bdbf59a"), "agathadesouza@outlook.com", "vRzoZsEojWhd+SITDQPjivMuuWoYMDziFfd4xL5sb80=", "S0EpC9M2amBJ2oXauo8h/Q==" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_admins_Email",
                table: "usuarios_admins",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios_admins");
        }
    }
}
