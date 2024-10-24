using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetFuncionariosOptions: GetIntegranteOptions
    {
        [JsonPropertyName("prefixo_cargo")]
        public string? PrefixoCargo { get; init; }

        [JsonPropertyName("status")]
        public int? Status { get; init; }

        [JsonPropertyName("salario_minimo")]
        public decimal SalarioMinimo { get; init; } = 0;

        [JsonPropertyName("salario_maximo")]
        public decimal SalarioMaximo { get; init; } = decimal.MaxValue;
    }
}