using Domain.Entities.Users;

namespace Domain.Repositories
{
    public interface IUsuarioProfessorRepository
    {
        void AddUsuarioProfessor(UsuarioProfessor usuarioProfessor);
        void UpdateUsuarioProfessor(UsuarioProfessor usuarioProfessor);

        Task<UsuarioProfessor?> GetProfessorAsync(Guid id);

        Task<UsuarioProfessor?> GetProfessorByEmailAsync(string email);
    }
}