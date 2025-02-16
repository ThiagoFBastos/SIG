using Shared.Dtos;
using Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUsuarioAlunoService
    {
        Task<string> Login(LoginUsuarioDto loginUsuarioDto);
        Task CadastraUsuarioAluno(UsuarioAlunoForCreateDto usuarioAlunoDto);
        Task AlteraSenhaUsuarioAluno(Guid id, ChangeUsuarioPasswordDto changePasswordDto);
        Task<UsuarioAlunoDto> ObterUsuarioAluno(Guid id, GetUsuarioAlunoOptions? opcoes = null);
        Task<UsuarioAlunoDto> ObterUsuarioAlunoPorEmail(string email, GetUsuarioAlunoOptions? opcoes = null);
    }
}