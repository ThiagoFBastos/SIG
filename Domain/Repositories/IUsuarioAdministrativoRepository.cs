using Domain.Entities;
using Domain.Entities.Users;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IUsuarioAdministrativoRepository
    {
        void AddUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo);

        void UpdateUsuarioAdministrativo(UsuarioAdministrativo usuarioAdministrativo);

        Task<UsuarioAdministrativo?> GetAdministrativoAsync(Guid id);

        Task<UsuarioAdministrativo?> GetAdminstrativoByEmailAsync(string email);
    }
}