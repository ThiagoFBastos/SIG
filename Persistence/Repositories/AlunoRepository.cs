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
    public class AlunoRepository: RepositoryBase<Aluno>, IAlunoRepository
    {
        public AlunoRepository(RepositoryContext context): base(context) {}
        public void AddAluno(Aluno aluno) => Add(aluno);
        public void UpdateAluno(Aluno aluno) => Update(aluno);
        public void DeleteAluno(Aluno aluno) => Delete(aluno);
        public Task<Aluno?> ApplyGetAlunoOptions(IQueryable<Aluno> queryable, GetAlunoOptions? opcoes = null)
        {
            if(opcoes != null)
            {
                if(opcoes.IncluirEndereco)
                    queryable = queryable.Include(a => a.Endereco);

                if(opcoes.IncluirTurmas)
                    queryable = queryable.Include(a => a.Turmas);

                if(opcoes.IncluirMedia)
                {
                    if(!opcoes.IncluirTurmas)
                        queryable = queryable.Include(a => a.Turmas);

                    queryable = queryable
                                    .Select(a => new Aluno {
                                        Matricula = a.Matricula,
                                        AnoEscolar = a.AnoEscolar,
                                        Celular = a.Celular,
                                        CPF = a.CPF,
                                        DataChegada = a.DataChegada,
                                        DataNascimento = a.DataNascimento,
                                        Email = a.Email,
                                        EnderecoId = a.EnderecoId,
                                        Endereco = a.Endereco,
                                        NomeCompleto = a.NomeCompleto,
                                        RG = a.RG,
                                        Turno = a.Turno,
                                        Sexo = a.Sexo,
                                        Status = a.Status,
                                        Media = a.Turmas.Average(t => t.Nota),
                                        Turmas = opcoes.IncluirTurmas ? a.Turmas : new List<AlunoTurma>()
                                    });
                        
                }
            }

            return queryable.FirstOrDefaultAsync<Aluno?>();
        }
        public Task<Aluno?> GetAlunoAsync(Guid matricula, GetAlunoOptions? opcoes = null)
            => ApplyGetAlunoOptions(FindByCondition(a => a.Matricula == matricula), opcoes);
        public Task<Aluno?> GetAlunoPorCPFAsync(string cpf, GetAlunoOptions? opcoes = null)
            => ApplyGetAlunoOptions(FindByCondition(a => a.CPF == cpf), opcoes);
        public Task<Aluno?> GetAlunoPorRGAsync(string rg, GetAlunoOptions? opcoes = null)
            => ApplyGetAlunoOptions(FindByCondition(a => a.RG == rg), opcoes);
        public Task<Aluno?> GetAlunoPeloEmailAsync(string email, GetAlunoOptions? opcoes = null)
            => ApplyGetAlunoOptions(FindByCondition(a => a.Email == email), opcoes);
        public Task<Aluno?> GetAlunoPeloCelularAsync(string celular, GetAlunoOptions? opcoes = null)
            => ApplyGetAlunoOptions(FindByCondition(a => a.Celular == celular), opcoes);
        public Task<List<Aluno>> GetAlunosAsync(GetAlunosOptions opcoes)
        {
            IQueryable<Aluno> query = FindAll();

            if(opcoes.PrefixoName != null)
                query = query.Where(a => a.NomeCompleto.StartsWith(opcoes.PrefixoName));

            if(opcoes.Turno != null)
                query = query.Where(a => (int)a.Turno == opcoes.Turno);

            if(opcoes.IncluirEndereco)
                query = query.Include(a => a.Endereco);

            if(opcoes.IncluirTurmas)
                query = query.Include(a => a.Turmas);
                
            if(opcoes.Ordenacao is not null)
            {
                var map = new Dictionary<string, Expression<Func<Aluno, object>>>()
                {
                    {"cpf", a => a.CPF},
                    {"rg", a => a.RG},
                    {"nome_completo", a => a.NomeCompleto},
                    {"email", a => a.Email},
                    {"celular", a => a.Celular},
                    {"data_chegada", a => a.DataChegada}
                };

                if(!map.ContainsKey(opcoes.Ordenacao))
                    throw new ArgumentException($"{opcoes.Ordenacao} não é um valor válido para a ordenação");

                var f = map[opcoes.Ordenacao];

                query = opcoes.Crescente ? query.OrderBy(f) : query.OrderByDescending(f);
            } else
                query = query.OrderBy(a => a.Matricula);

            return query
                    .Skip(opcoes.ComecarApartirDe * opcoes.LimiteDeResultados)
                    .Take(opcoes.LimiteDeResultados)
                    .ToListAsync();
        }
    }
}