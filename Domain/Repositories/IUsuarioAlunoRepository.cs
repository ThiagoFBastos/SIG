using Domain.Entities.Users;

namespace Domain.Repositories
{
    public interface IUsuarioAlunoRepository
    {
        void AddUsuarioAluno(UsuarioAluno usuarioAluno);
        void UpdateUsuarioAluno(UsuarioAluno usuarioAluno);
        Task<UsuarioAluno?> GetAlunoAsync(Guid id);
        Task<UsuarioAluno?> GetAlunoByEmailAsync(string email);
    }
}