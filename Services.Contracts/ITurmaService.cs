using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos;
using Shared.Pagination;

namespace Services.Contracts
{
    public interface ITurmaService
    {
        Task<Guid> CadastrarTurma(TurmaForCreateDto turma);

        Task<TurmaDto> AlterarTurma(Guid codigoTurma, TurmaForUpdateDto turma);

        Task DeletarTurma(Guid codigoTurma);

        Task<TurmaDto> ObterTurmaPorCodigo(Guid codigoTurma, GetTurmaOptions? opcoes = null);

        Task<TurmaSemNotaDto> ObterTurmaPorCodigoSemNota(Guid codigoTurma, GetTurmaOptions? opcoes = null);

        Task<Pagination<TurmaDto>> ObterTurmas(GetTurmasOptions opcoes);

        Task<Pagination<TurmaSemNotaDto>> ObterTurmasSemNota(GetTurmasOptions opcoes);
    }
}