using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.Pagination;

namespace Persistence.Repositories
{
    public class UsuarioAdministrativoRepository: RepositoryBase<UsuarioAdministrativo>, IUsuarioAdministrativoRepository
    {
        public UsuarioAdministrativoRepository(RepositoryContext context): base(context)
        {
            
        }

        public void AddUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo) => Add(usuarioAdministrativo);

        public void UpdateUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo) => Update(usuarioAdministrativo);

        public Task<UsuarioAdministrativo?> GetAdministrativoAsync(Guid id, GetUsuarioAdministrativoOptions? opcoes = null)
        {
            var usuario = FindByCondition(ua => ua.Id == id);

            if (opcoes != null && opcoes.IncluirAdministrativo)
                usuario = usuario.Include(ua => ua.Administrativo);

            return usuario.FirstOrDefaultAsync();
        }

        public Task<UsuarioAdministrativo?> GetAdminstrativoByEmailAsync(string email, GetUsuarioAdministrativoOptions? opcoes = null)
        {
            var usuario = FindByCondition(ua => ua.Email == email);

            if (opcoes != null && opcoes.IncluirAdministrativo)
                usuario = usuario.Include(ua => ua.Administrativo);

            return usuario.FirstOrDefaultAsync();
        }
    }
}