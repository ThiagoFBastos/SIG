using Domain.Entities;
using Shared.Dtos;
using Shared.Pagination;

namespace Services.Contracts
{
    public interface IAdministrativoService
    {
        Task<Guid> CadastrarAdministrativo(AdministrativoForCreateDto administrativo);

        Task<AdministrativoDto> AlterarAdministrativo(Guid administrativoMatricula, AdministrativoForUpdateDto administrativo);

        Task DeletarAdministrativo(Guid administrativoMatricula);

        Task<AdministrativoDto> ObterAdministrativoPorMatricula(Guid administrativoMatricula, GetAdministrativoOptions? opcoes = null);

        Task<AdministrativoDto> ObterAdministrativoPorCPF(string cpf, GetAdministrativoOptions? opcoes = null);

        Task<AdministrativoDto> ObterAdministrativoPorRG(string rg, GetAdministrativoOptions? opcoes = null);

        Task<AdministrativoDto> ObterAdministrativoPeloEmail(string email, GetAdministrativoOptions? opcoes = null);

        Task<AdministrativoDto> ObterAdministrativoPeloCelular(string celular, GetAdministrativoOptions? opcoes = null);

        Task<Pagination<AdministrativoDto>> ObterAdministrativos(GetAdministrativosOptions opcoes);
    }
}