using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Repositories;
using Domain.Entities;
using Persistence.Context;
using Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Repositories
{
    public class AdministrativoRepository: RepositoryBase<Administrativo>, IAdministrativoRepository
    {
        public AdministrativoRepository(RepositoryContext context): base(context) {}

        public void AddAdministrativo(Administrativo administrativo) => Add(administrativo);
        public void UpdateAdministrativo(Administrativo administrativo) => Update(administrativo);
        public void DeleteAdministrativo(Administrativo administrativo) => Delete(administrativo);

        private Task<Administrativo?> ApplyAdministrativoOptions(IQueryable<Administrativo> queryable, GetAdministrativoOptions? opcoes = null)
            => opcoes != null && opcoes.IncluirEndereco
                ? queryable
                    .Include(a => a.Endereco)
                    .FirstOrDefaultAsync<Administrativo?>()
                : queryable
                    .FirstOrDefaultAsync<Administrativo?>();

        public Task<Administrativo?> GetAdministrativoAsync(Guid matricula, GetAdministrativoOptions? opcoes = null)
            => ApplyAdministrativoOptions(FindByCondition(a => a.Matricula == matricula), opcoes);

        public Task<Administrativo?> GetAdministrativoPorCPFAsync(string cpf, GetAdministrativoOptions? opcoes = null)
            => ApplyAdministrativoOptions(FindByCondition(a => a.CPF == cpf), opcoes);

        public Task<Administrativo?> GetAdministrativoPorRGAsync(string rg, GetAdministrativoOptions? opcoes = null)
            => ApplyAdministrativoOptions(FindByCondition(a => a.RG == rg), opcoes);
        public Task<Administrativo?> GetAdministrativoPeloEmailAsync(string email, GetAdministrativoOptions? opcoes = null)
            => ApplyAdministrativoOptions(FindByCondition(a => a.Email == email), opcoes);
        public Task<Administrativo?> GetAdministrativoPeloCelularAsync(string celular, GetAdministrativoOptions? opcoes = null)
            => ApplyAdministrativoOptions(FindByCondition(a => a.Celular == celular), opcoes);

        public Task<List<Administrativo>> GetAdministrativosAsync(GetAdministrativosOptions opcoes)
        {
            IQueryable<Administrativo> query = FindAll();

            if(opcoes.Ordenacao != null)
            {
                var map = new Dictionary<string, Expression<Func<Administrativo, object>>>()
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
                query = query.Where(a => a.Cargo.StartsWith(opcoes.PrefixoCargo));

            if(opcoes.PrefixoName != null)
                query = query.Where(a => a.NomeCompleto.StartsWith(opcoes.PrefixoName));

            if(opcoes.Status != null)
                query = query.Where(a => (int)a.Status == opcoes.Status);

            query = query.Where(a => a.Salario >= opcoes.SalarioMinimo && a.Salario <= opcoes.SalarioMaximo);

            if(opcoes.IncluirEndereco)
                query = query.Include(a => a.Endereco);

            return query
                    .Skip(opcoes.ComecarApartirDe * opcoes.LimiteDeResultados)
                    .Take(opcoes.LimiteDeResultados)
                    .ToListAsync();
        }
    }
}