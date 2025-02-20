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
    public class UsuarioProfessorRepository: RepositoryBase<UsuarioProfessor>, IUsuarioProfessorRepository
    {
        public UsuarioProfessorRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioProfessor(UsuarioProfessor usuarioProfessor) => Add(usuarioProfessor);
        public void UpdateUsuarioProfessor(UsuarioProfessor usuarioProfessor) => Update(usuarioProfessor);

        public Task<UsuarioProfessor?> GetProfessorAsync(Guid id, GetUsuarioProfessorOptions? opcoes = null)
        {
            var usuario = FindByCondition(up => up.Id == id);

            if (opcoes != null && opcoes.IncluirProfessor)
                usuario = usuario.Include(up => up.Professor);

            return usuario.FirstOrDefaultAsync();
        }

        public Task<UsuarioProfessor?> GetProfessorByEmailAsync(string email, GetUsuarioProfessorOptions? opcoes = null)
        { 
            var usuario = FindByCondition(up => up.Email == email);

            if (opcoes != null && opcoes.IncluirProfessor)
                usuario = usuario.Include(up => up.Professor);

            return usuario.FirstOrDefaultAsync();
        }
    }
}