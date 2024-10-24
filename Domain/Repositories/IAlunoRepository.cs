using Domain.Entities;
using Shared.Pagination;

namespace Domain.Repositories
{
    public interface IAlunoRepository
    {
        void AddAluno(Aluno aluno);
        void UpdateAluno(Aluno aluno);
        void DeleteAluno(Aluno aluno);
        Task<Aluno?> GetAlunoAsync(Guid matricula, GetAlunoOptions? opcoes = null);
        Task<Aluno?> GetAlunoPorCPFAsync(string cpf, GetAlunoOptions? opcoes = null);
        Task<Aluno?> GetAlunoPorRGAsync(string rg, GetAlunoOptions? opcoes = null);
        Task<Aluno?> GetAlunoPeloEmailAsync(string email, GetAlunoOptions? opcoes = null);
        Task<Aluno?> GetAlunoPeloCelularAsync(string celular, GetAlunoOptions? opcoes = null);
        Task<List<Aluno>> GetAlunosAsync(GetAlunosOptions opcoes);
    }
}