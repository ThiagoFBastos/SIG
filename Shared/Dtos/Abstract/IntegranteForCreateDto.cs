using System.Text.Json.Serialization;
using Shared.Dtos;
using Domain.Entities.Abstract;

namespace Shared.Dtos.Abstract
{
    public abstract record class IntegranteForCreateDto
    {
        [JsonPropertyName("cpf")]
        public required string CPF { get; init;}

        [JsonPropertyName("rg")]
        public required string RG { get; init;}

        [JsonPropertyName("nome_completo")]
        public required string NomeCompleto { get; init;}

        [JsonPropertyName("email")]
        public required string Email { get; init; } 

        [JsonPropertyName("celular")]
        public required string Celular { get; init; }

        [JsonPropertyName("data_nascimento")]
        public required DateTime DataNascimento { get; init;}

        [JsonIgnore]
        public Guid EnderecoId { get; set; }

        [JsonPropertyName("endereco")]
        public required EnderecoForCreateDto Endereco { get; init; }

        [JsonPropertyName("sexo")]
        public required int Sexo { get; init; }

        public virtual bool Match(object? obj)
        {
            var integrante = obj as Integrante;

            if(integrante is null)
                return false;

            return integrante.Celular == Celular
            && integrante.CPF == CPF
            && integrante.DataNascimento == DataNascimento
            && integrante.Email == Email
            && integrante.NomeCompleto == NomeCompleto
            && integrante.RG == RG
            && (int)integrante.Sexo == Sexo;
        }
    }
}