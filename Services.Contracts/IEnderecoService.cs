using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Dtos;

namespace Services.Contracts
{
    public interface IEnderecoService
    {
        Task<Guid> CadastrarEndereco(EnderecoForCreateDto endereco);

        Task<EnderecoDto> AtualizarEndereco(Guid idEndereco, EnderecoForUpdateDto endereco);

        Task DeletarEndereco(Guid idEndereco);

        Task<EnderecoDto> ObterEnderecoPorId(Guid idEndereco);
    }
}