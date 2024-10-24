using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Shared.Pagination;

namespace Persistence.Repositories
{
    public class TurmaRepository: RepositoryBase<Turma>, ITurmaRepository
    {
        public TurmaRepository(RepositoryContext context): base(context) {}
        public void AddTurma(Turma turma) => Add(turma);
        public void UpdateTurma(Turma turma) => Update(turma);
        public void DeleteTurma(Turma turma) => Delete(turma);
        public Task<Turma?> GetTurmaAsync(Guid codigoTurma, GetTurmaOptions? opcoes = null)
        {
            var query = FindByCondition(t => t.Codigo == codigoTurma);

            if(opcoes is not null)
            {
                if(opcoes.IncluirAlunos)
                    query = query.Include(t => t.Alunos);

                if(opcoes.IncluirProfessor)
                    query = query.Include(t => t.Professor);
            }

            return query.FirstOrDefaultAsync<Turma?>();
        }
        public Task<List<Turma>> GetTurmasAsync(GetTurmasOptions opcoes)
        {
            IOrderedQueryable<Turma> orderedQuery;

            if(opcoes.Ordenacao is not null)
            {
                var map = new Dictionary<string, Expression<Func<Turma, object>>>()
                {
                    {"disciplina", t => t.Disciplina},
                    {"ano_escolar", t => t.AnoEscolar},
                    {"data_inicio", t => t.DataInicio},
                    {"data_fim", t => t.DataFim}
                };

                if(!map.ContainsKey(opcoes.Ordenacao))
                    throw new ArgumentException($"{opcoes.Ordenacao} não é um valor válido para a ordenação");

                var f = map[opcoes.Ordenacao];

                orderedQuery = opcoes.Crescente ? FindAll().OrderBy(f) : FindAll().OrderByDescending(f);
            }
            else
                orderedQuery = FindAll().OrderBy(t => t.Codigo);
            
            var query = orderedQuery
                .Where(t => t.DataInicio >= opcoes.ApartirData && t.DataFim <= opcoes.AteData);

            if(opcoes.PrefixoDisciplina != null)
                query = query.Where(t => t.Disciplina.StartsWith(opcoes.PrefixoDisciplina));

            if(opcoes.AnoEscolar != null)
                query = query.Where(t => (int)t.AnoEscolar == opcoes.AnoEscolar);

            if(opcoes.IncluirAlunos)
                query = query.Include(t => t.Alunos);

            if(opcoes.IncluirProfessor)
                query = query.Include(t => t.Professor);

            return query
                    .Skip(opcoes.ComecarApartirDe * opcoes.LimiteDeResultados)
                    .Take(opcoes.LimiteDeResultados)
                    .ToListAsync();
        }
    }
}