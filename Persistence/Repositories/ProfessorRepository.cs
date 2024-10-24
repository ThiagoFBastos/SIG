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
    public class ProfessorRepository: RepositoryBase<Professor>, IProfessorRepository
    {
        public ProfessorRepository(RepositoryContext context): base(context) {}

        public void AddProfessor(Professor professor) => Add(professor);
        public void UpdateProfessor(Professor professor) => Update(professor);
        public void DeleteProfessor(Professor professor) => Delete(professor);
        public Task<Professor?> ApplyGetProfessorOptions(IQueryable<Professor> queryable, GetProfessorOptions? opcoes = null)
        {
            if(opcoes != null)
            {
                if(opcoes.IncluirEndereco)
                    queryable = queryable.Include(p => p.Endereco);

                if(opcoes.IncluirTurmas)
                    queryable = queryable.Include(p => p.Turmas);
            }

            return queryable.FirstOrDefaultAsync<Professor?>();
        }
        public Task<Professor?> GetProfessorAsync(Guid matricula, GetProfessorOptions? opcoes = null)
            => ApplyGetProfessorOptions(FindByCondition(p => p.Matricula == matricula), opcoes);
        public Task<Professor?> GetProfessorPorCPFAsync(string cpf, GetProfessorOptions? opcoes = null)
            => ApplyGetProfessorOptions(FindByCondition(p => p.CPF == cpf), opcoes);
        public Task<Professor?> GetProfessorPorRGAsync(string rg, GetProfessorOptions? opcoes = null)
            => ApplyGetProfessorOptions(FindByCondition(p => p.RG == rg), opcoes);
        public Task<Professor?> GetProfessorPeloEmailAsync(string email, GetProfessorOptions? opcoes = null)
            => ApplyGetProfessorOptions(FindByCondition(p => p.Email == email), opcoes);
        public Task<Professor?> GetProfessorPeloCelularAsync(string celular, GetProfessorOptions? opcoes = null)
            => ApplyGetProfessorOptions(FindByCondition(p => p.Celular == celular), opcoes);
        public Task<List<Professor>> GetProfessoresAsync(GetProfessoresOptions opcoes)
        {
             IQueryable<Professor> query = FindAll();

            if(opcoes.Ordenacao != null)
            {
                var map = new Dictionary<string, Expression<Func<Professor, object>>>()
                {
                    {"cargo", a => a.Cargo},
                    {"salario", a => a.Salario},
                    {"cpf", a => a.CPF},
                    {"rg", a => a.RG},
                    {"nome_completo", a => a.NomeCompleto},
                    {"email", a => a.Email},
                    {"celular", a => a.Celular},
                    {"data_chegada", a => a.DataChegada}
                };

                if(!map.ContainsKey(opcoes.Ordenacao))
                    throw new ArgumentException($"{opcoes.Ordenacao} não é uma opção válida para a ordenação");

                var f = map[opcoes.Ordenacao];

                query = opcoes.Crescente
                    ? query.OrderBy(f)
                    : query.OrderByDescending(f);
            }
            else
                query = query.OrderBy(a => a.Matricula);

            if(opcoes.PrefixoCargo != null)
                query = query.Where(p => p.Cargo.StartsWith(opcoes.PrefixoCargo));

            if(opcoes.PrefixoName != null)
                query = query.Where(p => p.NomeCompleto.StartsWith(opcoes.PrefixoName));

            if(opcoes.Status != null)
                query = query.Where(p => (int)p.Status == opcoes.Status);

            query = query.Where(p => p.Salario >= opcoes.SalarioMinimo && p.Salario <= opcoes.SalarioMaximo);

            if(opcoes.IncluirEndereco)
                query = query.Include(p => p.Endereco);

            if(opcoes.IncluirTurmas)
                query = query.Include(p => p.Turmas);

            return query
                    .Skip(opcoes.ComecarApartirDe * opcoes.LimiteDeResultados)
                    .Take(opcoes.LimiteDeResultados)
                    .ToListAsync();
        }
    }
}