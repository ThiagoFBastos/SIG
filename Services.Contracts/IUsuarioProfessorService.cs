using Domain.Entities.Users;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUsuarioProfessorService
    {
        Task<string> Login(LoginUsuarioDto loginUsuarioDto);
        Task CadastraUsuarioProfessor(UsuarioProfessorForCreateDto usuarioProfessorDto);
        Task AlteraSenhaUsuarioProfessor(Guid id, ChangeUsuarioPasswordDto changePasswordDto);
        Task<UsuarioProfessorDto> ObterUsuarioProfessor(Guid id);
        Task<UsuarioProfessorDto> ObterUsuarioProfessorPorEmail(string email);
    }
}