using Shared.Dtos;
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
        Task<UsuarioAlunoDto> ObterUsuarioAluno(Guid id);
        Task<UsuarioAlunoDto> ObterUsuarioAlunoPorEmail(string email);
    }
}