using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class EnderecoDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("cidade")]
         public required string Cidade { get; set; }

         [JsonPropertyName("estado")]
         public required int Estado { get; set; }

         [JsonPropertyName("cep")]
         public required string CEP { get; set; }

         [JsonPropertyName("rua")]
         public required string Rua { get; set; }

         [JsonPropertyName("casa")]
         public required int Casa { get; set; }

         [JsonPropertyName("complemento")]
         public string? Complemento { get; set; }

         public bool Match(Endereco endereco)
         {
            return Id == endereco.Id
            && Cidade == endereco.Cidade
            && Estado == (int)endereco.Estado
            && CEP == endereco.CEP
            && Rua == endereco.Rua
            && Casa == endereco.Casa
            && Complemento == endereco.Complemento;
         }
    }
}