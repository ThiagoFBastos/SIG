using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Administrativos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "usuarios_admins",
                keyColumn: "Id",
                keyValue: new Guid("4c675501-acea-4287-9539-e87d4bdbf59a"));

            migrationBuilder.CreateTable(
                name: "usuarios_administrativos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdministrativoMatricula = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    SalString = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_administrativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_administrativos_administrativos_AdministrativoMatr~",
                        column: x => x.AdministrativoMatricula,
                        principalTable: "administrativos",
                        principalColumn: "Matricula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "usuarios_admins",
                columns: new[] { "Id", "Email", "PasswordHash", "SalString" },
                values: new object[] { new Guid("2ea5e2e9-3661-4706-b747-c8137de54b90"), "juliamagalhaes@outlook.com", "fshIo7UdrAnmToluuZKDE8dBASVxSMBV+EixhhU7HUM=", "uLPAd/dP4O/Nhg+WIKC07g==" });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_administrativos_AdministrativoMatricula",
                table: "usuarios_administrativos",
                column: "AdministrativoMatricula");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_administrativos_Email",
                table: "usuarios_administrativos",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios_administrativos");

            migrationBuilder.DeleteData(
                table: "usuarios_admins",
                keyColumn: "Id",
                keyValue: new Guid("2ea5e2e9-3661-4706-b747-c8137de54b90"));

            migrationBuilder.InsertData(
                table: "usuarios_admins",
                columns: new[] { "Id", "Email", "PasswordHash", "SalString" },
                values: new object[] { new Guid("4c675501-acea-4287-9539-e87d4bdbf59a"), "agathadesouza@outlook.com", "vRzoZsEojWhd+SITDQPjivMuuWoYMDziFfd4xL5sb80=", "S0EpC9M2amBJ2oXauo8h/Q==" });
        }
    }
}
