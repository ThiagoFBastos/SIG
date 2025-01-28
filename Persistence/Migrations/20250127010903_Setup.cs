using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enderecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CEP = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Rua = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Casa = table.Column<int>(type: "integer", nullable: false),
                    Complemento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enderecos", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "administrativos",
                columns: table => new
                {
                    Matricula = table.Column<Guid>(type: "uuid", nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    RG = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    NomeCompleto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Celular = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataChegada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Salario = table.Column<decimal>(type: "numeric", nullable: false),
                    Banco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContaCorrente = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataDemissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HorarioInicioExpediente = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HorarioFimExpediente = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrativos", x => x.Matricula);
                    table.ForeignKey(
                        name: "FK_administrativos_enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alunos",
                columns: table => new
                {
                    Matricula = table.Column<Guid>(type: "uuid", nullable: false),
                    AnoEscolar = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Turno = table.Column<int>(type: "integer", nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    RG = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    NomeCompleto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Celular = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataChegada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sexo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alunos", x => x.Matricula);
                    table.ForeignKey(
                        name: "FK_alunos_enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professores",
                columns: table => new
                {
                    Matricula = table.Column<Guid>(type: "uuid", nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    RG = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    NomeCompleto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Celular = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataChegada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Salario = table.Column<decimal>(type: "numeric", nullable: false),
                    Banco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContaCorrente = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataDemissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HorarioInicioExpediente = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HorarioFimExpediente = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professores", x => x.Matricula);
                    table.ForeignKey(
                        name: "FK_professores_enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "enderecos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "turmas",
                columns: table => new
                {
                    Codigo = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorMatricula = table.Column<Guid>(type: "uuid", nullable: false),
                    Disciplina = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AnoEscolar = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HorarioAulaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HorarioAulaFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turmas", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_turmas_professores_ProfessorMatricula",
                        column: x => x.ProfessorMatricula,
                        principalTable: "professores",
                        principalColumn: "Matricula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alunos_turma",
                columns: table => new
                {
                    Codigo = table.Column<Guid>(type: "uuid", nullable: false),
                    AlunoMatricula = table.Column<Guid>(type: "uuid", nullable: false),
                    TurmaCodigo = table.Column<Guid>(type: "uuid", nullable: false),
                    Nota = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alunos_turma", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_alunos_turma_alunos_AlunoMatricula",
                        column: x => x.AlunoMatricula,
                        principalTable: "alunos",
                        principalColumn: "Matricula",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alunos_turma_turmas_TurmaCodigo",
                        column: x => x.TurmaCodigo,
                        principalTable: "turmas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "usuarios_admins",
                columns: new[] { "Id", "Email", "PasswordHash", "SalString" },
                values: new object[] { new Guid("bc8862eb-5be7-4dfb-a1e4-b85c7072fb16"), "juliamagalhaes@outlook.com", "sMjTxzPJm/DGJB3yH2K0nWlLIRJJms6VQx+AqshSyNc=", "Ur0j4RP9p9ZZSh/uRx5PJw==" });

            migrationBuilder.CreateIndex(
                name: "IX_administrativos_Celular",
                table: "administrativos",
                column: "Celular",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_administrativos_CPF",
                table: "administrativos",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_administrativos_Email",
                table: "administrativos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_administrativos_EnderecoId",
                table: "administrativos",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_administrativos_RG",
                table: "administrativos",
                column: "RG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_Celular",
                table: "alunos",
                column: "Celular",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_CPF",
                table: "alunos",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_Email",
                table: "alunos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_EnderecoId",
                table: "alunos",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_alunos_RG",
                table: "alunos",
                column: "RG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_turma_AlunoMatricula_TurmaCodigo",
                table: "alunos_turma",
                columns: new[] { "AlunoMatricula", "TurmaCodigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_alunos_turma_TurmaCodigo",
                table: "alunos_turma",
                column: "TurmaCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_professores_Celular",
                table: "professores",
                column: "Celular",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professores_CPF",
                table: "professores",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professores_Email",
                table: "professores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professores_EnderecoId",
                table: "professores",
                column: "EnderecoId");

            migrationBuilder.CreateIndex(
                name: "IX_professores_RG",
                table: "professores",
                column: "RG",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turmas_ProfessorMatricula",
                table: "turmas",
                column: "ProfessorMatricula");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_administrativos_AdministrativoMatricula",
                table: "usuarios_administrativos",
                column: "AdministrativoMatricula");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_administrativos_Email",
                table: "usuarios_administrativos",
                column: "Email",
                unique: true);

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
                name: "alunos_turma");

            migrationBuilder.DropTable(
                name: "usuarios_administrativos");

            migrationBuilder.DropTable(
                name: "usuarios_admins");

            migrationBuilder.DropTable(
                name: "alunos");

            migrationBuilder.DropTable(
                name: "turmas");

            migrationBuilder.DropTable(
                name: "administrativos");

            migrationBuilder.DropTable(
                name: "professores");

            migrationBuilder.DropTable(
                name: "enderecos");
        }
    }
}
