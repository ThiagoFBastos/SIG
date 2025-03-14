using Domain.Entities.Users;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IUsuarioProfessorRepository
    {
        void AddUsuarioProfessor(UsuarioProfessor usuarioProfessor);
        void UpdateUsuarioProfessor(UsuarioProfessor usuarioProfessor);

        Task<UsuarioProfessor?> GetProfessorAsync(Guid id, GetUsuarioProfessorOptions? opcoes = null);

        Task<UsuarioProfessor?> GetProfessorByEmailAsync(string email, GetUsuarioProfessorOptions? opcoes = null);
    }
}