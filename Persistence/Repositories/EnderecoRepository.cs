using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class EnderecoRepository: RepositoryBase<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(RepositoryContext context): base(context) {}
        public void AddEndereco(Endereco endereco) => Add(endereco);

        public void UpdateEndereco(Endereco endereco) => Update(endereco);
        public void DeleteEndereco(Endereco endereco) => Delete(endereco);
        public Task<Endereco?> GetEnderecoAsync(Guid id)
            => FindByCondition(e => e.Id == id)
                .FirstOrDefaultAsync<Endereco?>();
    }
}