using Domain.Entities.Users;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IUsuarioAlunoRepository
    {
        void AddUsuarioAluno(UsuarioAluno usuarioAluno);
        void UpdateUsuarioAluno(UsuarioAluno usuarioAluno);
        Task<UsuarioAluno?> GetAlunoAsync(Guid id, GetUsuarioAlunoOptions? opcoes = null);
        Task<UsuarioAluno?> GetAlunoByEmailAsync(string email, GetUsuarioAlunoOptions? opcoes = null);
    }
}