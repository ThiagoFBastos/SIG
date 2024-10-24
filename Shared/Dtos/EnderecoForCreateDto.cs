using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared.Dtos
{
    public record EnderecoForCreateDto
    {
         [JsonPropertyName("cidade")]
         public required string Cidade { get; init; }

         [JsonPropertyName("estado")]
         public required int Estado { get; init; }

         [JsonPropertyName("cep")]
         public required string CEP { get; init; }

         [JsonPropertyName("rua")]
         public required string Rua { get; init; }

         [JsonPropertyName("casa")]
         public required int Casa { get; init; }

         [JsonPropertyName("complemento")]
         public string? Complemento { get; init; }

         public bool Match(Endereco endereco)
         {
            return Cidade == endereco.Cidade
            && Estado == (int)endereco.Estado
            && CEP == endereco.CEP
            && Rua == endereco.Rua
            && Casa == endereco.Casa
            && Complemento == endereco.Complemento;
         }
    }
} 