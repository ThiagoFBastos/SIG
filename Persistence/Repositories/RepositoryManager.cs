using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class RepositoryManager: IRepositoryManager
    {
        private readonly Lazy<IAdministrativoRepository> administrativoRepository;
        private readonly Lazy<IAlunoRepository> alunoRepository;
        private readonly Lazy<IAlunoTurmaRepository> alunoTurmaRepository;
        private readonly Lazy<IEnderecoRepository> enderecoRepository;
        private readonly Lazy<IProfessorRepository> professorRepository;
        private readonly Lazy<ITurmaRepository> turmaRepository;
        private readonly Lazy<IUsuarioAdminRepository> usuarioAdminRepository;
        private readonly Lazy<IUsuarioAdministrativoRepository> usuarioAdministrativoRepository;
        private readonly Lazy<IUsuarioAlunoRepository> usuarioAlunoRepository;
        private readonly Lazy<IUsuarioProfessorRepository> usuarioProfessorRepository;
        private readonly RepositoryContext _context;

        public RepositoryManager(RepositoryContext context) 
        {
            administrativoRepository = new Lazy<IAdministrativoRepository>(() => new AdministrativoRepository(context));
            alunoRepository = new Lazy<IAlunoRepository>(() => new AlunoRepository(context));
            alunoTurmaRepository = new Lazy<IAlunoTurmaRepository>(() => new AlunoTurmaRepository(context));
            enderecoRepository = new Lazy<IEnderecoRepository>(() => new EnderecoRepository(context));
            professorRepository = new Lazy<IProfessorRepository>(() => new ProfessorRepository(context));
            turmaRepository = new Lazy<ITurmaRepository>(() => new TurmaRepository(context));
            usuarioAdminRepository = new Lazy<IUsuarioAdminRepository>(() => new UsuarioAdminRepository(context));
            usuarioAdministrativoRepository = new Lazy<IUsuarioAdministrativoRepository>(() => new UsuarioAdministrativoRepository(context));
            usuarioAlunoRepository = new Lazy<IUsuarioAlunoRepository>(() => new UsuarioAlunoRepository(context));
            usuarioProfessorRepository = new Lazy<IUsuarioProfessorRepository>(() => new UsuarioProfessorRepository(context));
            _context = context;
        }

        public IAdministrativoRepository AdministrativoRepository => administrativoRepository.Value;
        public IAlunoRepository AlunoRepository => alunoRepository.Value;
        public IAlunoTurmaRepository AlunoTurmaRepository => alunoTurmaRepository.Value;
        public IEnderecoRepository EnderecoRepository => enderecoRepository.Value;
        public IProfessorRepository ProfessorRepository => professorRepository.Value;
        public ITurmaRepository TurmaRepository => turmaRepository.Value;
        public IUsuarioAdminRepository UsuarioAdminRepository => usuarioAdminRepository.Value;
        public IUsuarioAdministrativoRepository UsuarioAdministrativoRepository => usuarioAdministrativoRepository.Value;
        public IUsuarioAlunoRepository UsuarioAlunoRepository => usuarioAlunoRepository.Value;
        public IUsuarioProfessorRepository UsuarioProfessorRepository => usuarioProfessorRepository.Value;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}