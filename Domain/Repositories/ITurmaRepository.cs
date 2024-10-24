using Domain.Entities;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface ITurmaRepository
    {
        void AddTurma(Turma turma);
        void UpdateTurma(Turma turma);
        void DeleteTurma(Turma turma);
        Task<Turma?> GetTurmaAsync(Guid codigoTurma, GetTurmaOptions? opcoes = null);
        Task<List<Turma>> GetTurmasAsync(GetTurmasOptions opcoes);
    }
}