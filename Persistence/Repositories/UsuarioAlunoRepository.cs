using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.Pagination;

namespace Persistence.Repositories
{
    public class UsuarioAlunoRepository: RepositoryBase<UsuarioAluno>, IUsuarioAlunoRepository
    {
        public UsuarioAlunoRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioAluno(UsuarioAluno usuarioAluno) => Add(usuarioAluno);
        public void UpdateUsuarioAluno(UsuarioAluno usuarioAluno) => Update(usuarioAluno);
        public Task<UsuarioAluno?> GetAlunoAsync(Guid id, GetUsuarioAlunoOptions? opcoes = null)
        {
            var usuario = FindByCondition(ua => ua.Id == id);

            if (opcoes != null && opcoes.IncluirAluno)
                usuario = usuario.Include(ua => ua.Aluno);

            return usuario.FirstOrDefaultAsync();
        }

        public Task<UsuarioAluno?> GetAlunoByEmailAsync(string email, GetUsuarioAlunoOptions? opcoes = null)
        {
            var usuario = FindByCondition(ua => ua.Email == email);

            if (opcoes != null && opcoes.IncluirAluno)
                usuario = usuario.Include(ua => ua.Aluno);

            return usuario.FirstOrDefaultAsync();
        }
    }
}