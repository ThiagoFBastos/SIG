using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class UsuarioAlunoRepository: RepositoryBase<UsuarioAluno>, IUsuarioAlunoRepository
    {
        public UsuarioAlunoRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioAluno(UsuarioAluno usuarioAluno) => Add(usuarioAluno);
        public void UpdateUsuarioAluno(UsuarioAluno usuarioAluno) => Update(usuarioAluno);
        public Task<UsuarioAluno?> GetAlunoAsync(Guid id)
            => FindByCondition(ua => ua.Id == id)
                .FirstOrDefaultAsync();
        public Task<UsuarioAluno?> GetAlunoByEmailAsync(string email)
            => FindByCondition(ua => ua.Email == email)
                .FirstOrDefaultAsync();
    }
}