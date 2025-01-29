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
    public class UsuarioProfessorRepository: RepositoryBase<UsuarioProfessor>, IUsuarioProfessorRepository
    {
        public UsuarioProfessorRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioProfessor(UsuarioProfessor usuarioProfessor) => Add(usuarioProfessor);
        public void UpdateUsuarioProfessor(UsuarioProfessor usuarioProfessor) => Update(usuarioProfessor);

        public Task<UsuarioProfessor?> GetProfessorAsync(Guid id)
             => FindByCondition(up => up.Id == id)
                 .FirstOrDefaultAsync();

        public Task<UsuarioProfessor?> GetProfessorByEmailAsync(string email)
            => FindByCondition(up => up.Email == email)
                .FirstOrDefaultAsync();
    }
}