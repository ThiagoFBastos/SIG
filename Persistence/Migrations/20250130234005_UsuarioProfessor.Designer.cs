﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Persistence.Context;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    [Migration("20250130234005_UsuarioProfessor")]
    partial class UsuarioProfessor
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Administrativo", b =>
                {
                    b.Property<Guid>("Matricula")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Banco")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("Cargo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("ContaCorrente")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("DataChegada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<DateTime?>("DataDemissao")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("EnderecoId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("HorarioFimExpediente")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("HorarioInicioExpediente")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("RG")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<decimal>("Salario")
                        .HasColumnType("numeric");

                    b.Property<int>("Sexo")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Matricula");

                    b.HasIndex("CPF")
                        .IsUnique();

                    b.HasIndex("Celular")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("EnderecoId");

                    b.HasIndex("RG")
                        .IsUnique();

                    b.ToTable("administrativos");
                });

            modelBuilder.Entity("Domain.Entities.Aluno", b =>
                {
                    b.Property<Guid>("Matricula")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AnoEscolar")
                        .HasColumnType("integer");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<DateTime>("DataChegada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("EnderecoId")
                        .HasColumnType("uuid");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("RG")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<int>("Sexo")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Turno")
                        .HasColumnType("integer");

                    b.HasKey("Matricula");

                    b.HasIndex("CPF")
                        .IsUnique();

                    b.HasIndex("Celular")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("EnderecoId");

                    b.HasIndex("RG")
                        .IsUnique();

                    b.ToTable("alunos");
                });

            modelBuilder.Entity("Domain.Entities.AlunoTurma", b =>
                {
                    b.Property<Guid>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AlunoMatricula")
                        .HasColumnType("uuid");

                    b.Property<double>("Nota")
                        .HasColumnType("double precision");

                    b.Property<Guid>("TurmaCodigo")
                        .HasColumnType("uuid");

                    b.HasKey("Codigo");

                    b.HasIndex("TurmaCodigo");

                    b.HasIndex("AlunoMatricula", "TurmaCodigo")
                        .IsUnique();

                    b.ToTable("alunos_turma");
                });

            modelBuilder.Entity("Domain.Entities.Endereco", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<int>("Casa")
                        .HasColumnType("integer");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Complemento")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Estado")
                        .HasColumnType("integer");

                    b.Property<string>("Rua")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.ToTable("enderecos");
                });

            modelBuilder.Entity("Domain.Entities.Professor", b =>
                {
                    b.Property<Guid>("Matricula")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Banco")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("Cargo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("ContaCorrente")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("DataChegada")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<DateTime?>("DataDemissao")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<Guid>("EnderecoId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("HorarioFimExpediente")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("HorarioInicioExpediente")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("RG")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<decimal>("Salario")
                        .HasColumnType("numeric");

                    b.Property<int>("Sexo")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Matricula");

                    b.HasIndex("CPF")
                        .IsUnique();

                    b.HasIndex("Celular")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("EnderecoId");

                    b.HasIndex("RG")
                        .IsUnique();

                    b.ToTable("professores");
                });

            modelBuilder.Entity("Domain.Entities.Turma", b =>
                {
                    b.Property<Guid>("Codigo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AnoEscolar")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DataFim")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DataInicio")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Disciplina")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("HorarioAulaFim")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("HorarioAulaInicio")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProfessorMatricula")
                        .HasColumnType("uuid");

                    b.HasKey("Codigo");

                    b.HasIndex("ProfessorMatricula");

                    b.ToTable("turmas");
                });

            modelBuilder.Entity("Domain.Entities.Users.UsuarioAdmin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SalString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("usuarios_admins");

                    b.HasData(
                        new
                        {
                            Id = new Guid("1a9b9430-45b9-4878-aea1-6f6d8f70d5ff"),
                            Email = "juliamagalhaes@outlook.com",
                            PasswordHash = "SUvGJ3xPiM28dINpJa5+UTsCyqDJZTPPdE/Vliwruec=",
                            SalString = "qYeEjC4mxFyEkwIPxQFSKA=="
                        });
                });

            modelBuilder.Entity("Domain.Entities.Users.UsuarioAdministrativo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AdministrativoMatricula")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SalString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AdministrativoMatricula");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("usuarios_administrativos");
                });

            modelBuilder.Entity("Domain.Entities.Users.UsuarioProfessor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("ProfessorMatricula")
                        .HasColumnType("uuid");

                    b.Property<string>("SalString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("ProfessorMatricula");

                    b.ToTable("usuarios_professores");
                });

            modelBuilder.Entity("Domain.Entities.Administrativo", b =>
                {
                    b.HasOne("Domain.Entities.Endereco", "Endereco")
                        .WithMany()
                        .HasForeignKey("EnderecoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Endereco");
                });

            modelBuilder.Entity("Domain.Entities.Aluno", b =>
                {
                    b.HasOne("Domain.Entities.Endereco", "Endereco")
                        .WithMany()
                        .HasForeignKey("EnderecoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Endereco");
                });

            modelBuilder.Entity("Domain.Entities.AlunoTurma", b =>
                {
                    b.HasOne("Domain.Entities.Aluno", "Aluno")
                        .WithMany("Turmas")
                        .HasForeignKey("AlunoMatricula")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Turma", "Turma")
                        .WithMany("Alunos")
                        .HasForeignKey("TurmaCodigo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aluno");

                    b.Navigation("Turma");
                });

            modelBuilder.Entity("Domain.Entities.Professor", b =>
                {
                    b.HasOne("Domain.Entities.Endereco", "Endereco")
                        .WithMany()
                        .HasForeignKey("EnderecoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Endereco");
                });

            modelBuilder.Entity("Domain.Entities.Turma", b =>
                {
                    b.HasOne("Domain.Entities.Professor", "Professor")
                        .WithMany("Turmas")
                        .HasForeignKey("ProfessorMatricula")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Professor");
                });

            modelBuilder.Entity("Domain.Entities.Users.UsuarioAdministrativo", b =>
                {
                    b.HasOne("Domain.Entities.Administrativo", "Administrativo")
                        .WithMany()
                        .HasForeignKey("AdministrativoMatricula")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Administrativo");
                });

            modelBuilder.Entity("Domain.Entities.Users.UsuarioProfessor", b =>
                {
                    b.HasOne("Domain.Entities.Professor", "Professor")
                        .WithMany()
                        .HasForeignKey("ProfessorMatricula")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Professor");
                });

            modelBuilder.Entity("Domain.Entities.Aluno", b =>
                {
                    b.Navigation("Turmas");
                });

            modelBuilder.Entity("Domain.Entities.Professor", b =>
                {
                    b.Navigation("Turmas");
                });

            modelBuilder.Entity("Domain.Entities.Turma", b =>
                {
                    b.Navigation("Alunos");
                });
#pragma warning restore 612, 618
        }
    }
}
