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
    public class UsuarioAdminRepository: RepositoryBase<UsuarioAdmin>, IUsuarioAdminRepository
    {
        public UsuarioAdminRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioAdmin(UsuarioAdmin usuarioAdmin) => Add(usuarioAdmin);

        public void UpdateUsuarioAdmin(UsuarioAdmin usuarioAdmin) => Update(usuarioAdmin);

        public void DeleteUsuarioAdmin(UsuarioAdmin usuarioAdmin) => Delete(usuarioAdmin);

        public Task<UsuarioAdmin?> GetUsuarioAdminByIdAsync(Guid id)
            => FindByCondition(ua => ua.Id == id)
                .FirstOrDefaultAsync();

        public Task<UsuarioAdmin?> GetUsuarioAdminByEmailAsync(string email)
               => FindByCondition(ua => ua.Email == email)
                .FirstOrDefaultAsync();

    }
}