using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Abstract;

namespace Shared.Dtos.Abstract
{
    public abstract record class IntegranteDto
    { 
        [JsonPropertyName("cpf")]
        public required string CPF { get; set; }

        [JsonPropertyName("rg")]
        public required string RG { get; set; }

        [JsonPropertyName("nome_completo")]
        public required string NomeCompleto { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("celular")]
        public required string Celular { get; set; }

        [JsonPropertyName("data_nascimento")]
        public required DateTime DataNascimento { get; set; }

        [JsonPropertyName("data_chegada")]
        public required DateTime DataChegada { get; set; }

        [JsonPropertyName("endereco_id")]
        public required Guid EnderecoId { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDto? Endereco { get; set; }

        [JsonPropertyName("sexo")]
        public required int Sexo { get; set; }
        public virtual bool Match(object? obj)
        {
             var integrante = obj as Integrante;

            if(integrante is null)
                return false;

            return integrante.Celular == Celular
            && integrante.CPF == CPF
            && integrante.DataChegada == DataChegada
            && integrante.DataNascimento == DataNascimento
            && integrante.Email == Email
            && integrante.EnderecoId == EnderecoId
            && integrante.NomeCompleto == NomeCompleto
            && integrante.RG == RG
            && (int)integrante.Sexo == Sexo;
        }
    }
}