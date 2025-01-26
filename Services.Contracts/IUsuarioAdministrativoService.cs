using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUsuarioAdministrativoService
    {
        Task<string> Login(LoginUsuarioDto loginUsuarioDto);
        Task CadastraUsuarioAdministrativo(UsuarioAdministrativoForCreateDto usuarioAdministrativoDto);

        Task AlteraSenhaUsuarioAdministrativo(Guid id, ChangeUsuarioPasswordDto changePasswordDto);

        Task<UsuarioAdministrativoDto> ObterUsuarioAdministrativo(Guid id);

        Task<UsuarioAdministrativoDto> ObterUsuarioAdministrativoPorEmail(string email);
    }
}