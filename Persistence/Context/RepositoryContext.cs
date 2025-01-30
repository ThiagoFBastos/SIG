using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Entities.Users;
using Services;

namespace Persistence.Context
{
    public class RepositoryContext: DbContext
    {
        public DbSet<Aluno> Alunos { get; private set;}
        public DbSet<Professor> Professores { get; private set; }
        public DbSet<Administrativo> Administrativos { get; private set; }
        public DbSet<Turma> Turmas { get; private set; }
        public DbSet<Endereco> Enderecos { get; private set; }
        public DbSet<AlunoTurma> AlunosTurmas { get; private set; }
        public DbSet<UsuarioAdmin> UsuarioAdmins { get; private set; }
        public DbSet<UsuarioAdministrativo> UsuarioAdministrativos { get; private set; }
        public DbSet<UsuarioProfessor> UsuarioProfessores { get; private set; }
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // endereço
            modelBuilder.Entity<Endereco>(e => {

            });

            // aluno na turma
             modelBuilder.Entity<AlunoTurma>(at => {

                at.HasIndex(at => new {at.AlunoMatricula, at.TurmaCodigo})
                .IsUnique();
             });

            // turma
            modelBuilder.Entity<Turma>(t => {

                t.HasOne(t => t.Professor)
                .WithMany(p => p.Turmas)
                .HasForeignKey(t => t.ProfessorMatricula);

                t.HasMany(t => t.Alunos)
                .WithOne(at => at.Turma)
                .HasForeignKey(at => at.TurmaCodigo);
            });

            // aluno

            modelBuilder.Entity<Aluno>(a => {
                a.HasMany(a => a.Turmas)
                .WithOne(at => at.Aluno)
                .HasForeignKey(at => at.AlunoMatricula);

                a.HasIndex(a => a.CPF)
                .IsUnique(); 

                a.HasIndex(a => a.RG)
                .IsUnique();

                a.HasIndex(a => a.Email)
                .IsUnique();

                a.HasIndex(a => a.Celular)
                .IsUnique();

                a.HasOne(a => a.Endereco)
                .WithMany()
                .HasForeignKey(a => a.EnderecoId);

                a.Property(a => a.DataChegada)
                .HasDefaultValueSql("NOW()"); // caso seja sql server trocar
            });

            // professor
             modelBuilder.Entity<Professor>(p => {
                p.HasMany(p => p.Turmas)
                .WithOne(t => t.Professor)
                .HasForeignKey(t => t.ProfessorMatricula);

                p.HasIndex(p => p.CPF)
                .IsUnique();

                p.HasIndex(p => p.RG)
                .IsUnique();

                p.HasIndex(p => p.Email)
                .IsUnique();

                p.HasIndex(p => p.Celular)
                .IsUnique();

                p.HasOne(p => p.Endereco)
                .WithMany()
                .HasForeignKey(p => p.EnderecoId);

                p.Property(p => p.DataChegada)
                .HasDefaultValueSql("NOW()"); // caso seja sql server trocar
            });

            // administrativo
            modelBuilder.Entity<Administrativo>(a => {
                a.HasIndex(a => a.CPF)
                .IsUnique();

                a.HasIndex(a => a.RG)
                .IsUnique();

                a.HasIndex(a => a.Email)
                .IsUnique();

                a.HasIndex(a => a.Celular)
                .IsUnique();

                a.HasOne(a => a.Endereco)
                .WithMany()
                .HasForeignKey(a => a.EnderecoId);

                a.Property(a => a.DataChegada)
                .HasDefaultValueSql("NOW()"); // caso seja sql server trocar
            });

            modelBuilder.Entity<UsuarioAdmin>(a =>
            {
                const string password = "15158114099Aa$$";
                string salString;
                PasswordHash hash = new PasswordHash();

                string passwordHash = hash.Encrypt(password, out salString);

                a.HasIndex(a => a.Email)
                    .IsUnique();

                a.HasData(new UsuarioAdmin { Id = Guid.NewGuid(), Email = "juliamagalhaes@outlook.com", PasswordHash = passwordHash, SalString = salString });
            });

            modelBuilder.Entity<UsuarioAdministrativo>(a =>
            {
                a.HasIndex(a => a.Email)
                    .IsUnique();

                a.HasOne(a => a.Administrativo)
                    .WithMany()
                    .HasForeignKey(a => a.AdministrativoMatricula);
            });

            modelBuilder.Entity<UsuarioProfessor>(p =>
            {
                p.HasIndex(a => a.Email)
                    .IsUnique();

                p.HasOne(a => a.Professor)
                    .WithMany()
                    .HasForeignKey(a => a.ProfessorMatricula);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}