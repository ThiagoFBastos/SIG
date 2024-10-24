using Domain.Entities;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IProfessorRepository
    {
        void AddProfessor(Professor professor);
        void UpdateProfessor(Professor professor);
        void DeleteProfessor(Professor professor);
        Task<Professor?> GetProfessorAsync(Guid matricula, GetProfessorOptions? opcoes = null);
        Task<Professor?> GetProfessorPorCPFAsync(string cpf, GetProfessorOptions? opcoes = null);
        Task<Professor?> GetProfessorPorRGAsync(string rg, GetProfessorOptions? opcoes = null);
        Task<Professor?> GetProfessorPeloEmailAsync(string email, GetProfessorOptions? opcoes = null);
        Task<Professor?> GetProfessorPeloCelularAsync(string celular, GetProfessorOptions? opcoes = null);
        Task<List<Professor>> GetProfessoresAsync(GetProfessoresOptions opcoes);
    }
}