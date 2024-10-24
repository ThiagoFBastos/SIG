using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetIntegranteOptions: GetOptions
    {
        [JsonPropertyName("prefixo_name")]
        public string? PrefixoName { get; init; }

        [JsonPropertyName("incluir_endereco")]
        public bool IncluirEndereco { get; init; } = false;
    }
}