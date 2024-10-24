using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Dtos;
using Shared.Pagination;

namespace Services.Contracts
{
    public interface IProfessorService
    {
        Task<Guid> CadastrarProfessor(ProfessorForCreateDto professor);

        Task<ProfessorDto> AlterarProfessor(Guid matriculaProfessor, ProfessorForUpdateDto professor);

        Task DeletarProfessor(Guid matriculaProfessor);

        Task<ProfessorDto> ObterProfessorPorMatricula(Guid matriculaProfessor, GetProfessorOptions? opcoes = null);

        Task<ProfessorDto> ObterProfessorPorCPF(string cpf, GetProfessorOptions? opcoes = null);

        Task<ProfessorDto> ObterProfessorPorRG(string rg, GetProfessorOptions? opcoes = null);

        Task<ProfessorDto> ObterProfessorPeloEmail(string email, GetProfessorOptions? opcoes = null);

        Task<ProfessorDto> ObterProfessorPeloCelular(string celular, GetProfessorOptions? opcoes = null);

        Task<Pagination<ProfessorDto>> ObterProfessores(GetProfessoresOptions opcoes);
    }
}