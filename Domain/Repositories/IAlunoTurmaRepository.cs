using Domain.Entities;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IAlunoTurmaRepository
    {
        void AddAlunoTurma(AlunoTurma alunoTurma);
        void UpdateAlunoTurma(AlunoTurma alunoTurma);
        void DeleteAlunoTurma(AlunoTurma alunoTurma);
        Task<AlunoTurma?> GetAlunoTurmaAsync(Guid alunoMatricula, Guid turmaCodigo, GetAlunoTurmaOptions? opcoes = null);
        Task<AlunoTurma?> GetAlunoTurmaPorCodigoAsync(Guid codigo, GetAlunoTurmaOptions? opcoes = null);
    }
}