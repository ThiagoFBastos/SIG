using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.Pagination;

namespace Persistence.Repositories
{
    public class AlunoTurmaRepository: RepositoryBase<AlunoTurma>, IAlunoTurmaRepository
    {
        public AlunoTurmaRepository(RepositoryContext context): base(context) {}
        public void AddAlunoTurma(AlunoTurma alunoTurma) => Add(alunoTurma);
        public void UpdateAlunoTurma(AlunoTurma alunoTurma) => Update(alunoTurma);
        public void DeleteAlunoTurma(AlunoTurma alunoTurma) => Delete(alunoTurma);
        public Task<AlunoTurma?> GetAlunoTurmaAsync(Guid alunoMatricula, Guid turmaCodigo, GetAlunoTurmaOptions? opcoes = null)
        {
            var query = FindByCondition(at => at.AlunoMatricula == alunoMatricula && at.TurmaCodigo == turmaCodigo);

            if(opcoes != null && opcoes.IncluirAluno)
                query = query.Include(at => at.Aluno);

            return query.FirstOrDefaultAsync<AlunoTurma?>();
        }
        public Task<AlunoTurma?> GetAlunoTurmaPorCodigoAsync(Guid codigo, GetAlunoTurmaOptions? opcoes = null)
        {
            var query = FindByCondition(at => at.Codigo == codigo);

            if(opcoes != null && opcoes.IncluirAluno)
                query = query.Include(at => at.Aluno);

            return query.FirstOrDefaultAsync<AlunoTurma?>();
        }
    }
}