using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Dtos;
using Shared.Pagination;

namespace Services.Contracts
{
    public interface IAlunoService
    {
        Task<Guid> CadastrarAluno(AlunoForCreateDto aluno);
        Task<AlunoDto> AlterarAluno(Guid matriculaAluno, AlunoForUpdateDto aluno);
        Task DeletarAluno(Guid matriculaAluno);
        Task<AlunoDto> ObterAlunoPorMatricula(Guid matriculaAluno, GetAlunoOptions? opcoes = null);
        Task<AlunoDto> ObterAlunoPorCPF(string cpf, GetAlunoOptions? opcoes = null);
        Task<AlunoDto> ObterAlunoPorRG(string rg, GetAlunoOptions? opcoes = null);
        Task<AlunoDto> ObterAlunoPeloEmail(string email, GetAlunoOptions? opcoes = null);
        Task<AlunoDto> ObterAlunoPeloCelular(string rg, GetAlunoOptions? opcoes = null);
        Task<Pagination<AlunoDto>> ObterAlunos(GetAlunosOptions opcoes);
    }
}  