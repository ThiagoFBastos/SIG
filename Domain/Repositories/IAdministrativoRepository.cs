using Domain.Entities;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IAdministrativoRepository
    {
        void AddAdministrativo(Administrativo administrativo);
        void UpdateAdministrativo(Administrativo administrativo);
        void DeleteAdministrativo(Administrativo administrativo);
        Task<Administrativo?> GetAdministrativoAsync(Guid matricula, GetAdministrativoOptions? opcoes = null);
        Task<Administrativo?> GetAdministrativoPorCPFAsync(string cpf, GetAdministrativoOptions? opcoes = null);
        Task<Administrativo?> GetAdministrativoPorRGAsync(string rg, GetAdministrativoOptions? opcoes = null);
        Task<Administrativo?> GetAdministrativoPeloEmailAsync(string email, GetAdministrativoOptions? opcoes = null);
        Task<Administrativo?> GetAdministrativoPeloCelularAsync(string celular, GetAdministrativoOptions? opcoes = null);
        Task<List<Administrativo>> GetAdministrativosAsync(GetAdministrativosOptions opcoes);
    }
}