using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class UsuarioAdministrativoRepository: RepositoryBase<UsuarioAdministrativo>, IUsuarioAdministrativoRepository
    {
        public UsuarioAdministrativoRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo) => Add(usuarioAdministrativo);

        public void UpdateUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo) => Update(usuarioAdministrativo);

        public Task<UsuarioAdministrativo?> GetAdministrativoAsync(Guid id)
               => FindByCondition(ua => ua.Id == id)
                    .FirstOrDefaultAsync();

        public Task<UsuarioAdministrativo?> GetAdminstrativoByEmailAsync(string email)
                => FindByCondition(ua => ua.Email == email)
                    .FirstOrDefaultAsync();
    }
}