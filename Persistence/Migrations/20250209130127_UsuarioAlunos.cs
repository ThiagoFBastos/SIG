using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioAlunos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios_alunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlunoMatricula = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    SalString = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios_alunos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_alunos_alunos_AlunoMatricula",
                        column: x => x.AlunoMatricula,
                        principalTable: "alunos",
                        principalColumn: "Matricula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_alunos_AlunoMatricula",
                table: "usuarios_alunos",
                column: "AlunoMatricula");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_alunos_Email",
                table: "usuarios_alunos",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios_alunos");
        }
    }
}
