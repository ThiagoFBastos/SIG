using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Pagination;

namespace Services.Contracts
{
    public interface IAlunoTurmaService
    { 
        Task<Guid> CadastrarAlunoNaTurma(AlunoTurmaForCreateDto alunoTurma);

        Task<AlunoTurmaDto> AlterarAlunoNaTurma(Guid matriculaAluno, Guid codigoTurma, AlunoTurmaForUpdateDto alunoTurma);

        Task DeletarAlunoDaTurma(Guid matriculaAluno, Guid codigoTurma);

        Task<AlunoTurmaDto> ObterAlunoDaTurma(Guid matriculaAluno, Guid codigoTurma, GetAlunoTurmaOptions? opcoes = null);

        Task<AlunoTurmaDto> ObterAlunoDatTurmaPorCodigo(Guid codigo, GetAlunoTurmaOptions? opcoes = null);
    }
}