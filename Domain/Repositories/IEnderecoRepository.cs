using Domain.Entities;

namespace Domain.Repositories
{
    public interface IEnderecoRepository
    {
        void AddEndereco(Endereco endereco);
        void UpdateEndereco(Endereco endereco);
        void DeleteEndereco(Endereco endereco);
        Task<Endereco?> GetEnderecoAsync(Guid id);
    }
}