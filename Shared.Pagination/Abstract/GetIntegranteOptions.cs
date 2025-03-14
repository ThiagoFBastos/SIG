using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetIntegranteOptions: GetOptions
    {
        [FromQuery(Name = "prefixo_name")]
        [JsonPropertyName("prefixo_name")]
        public string? PrefixoName { get; init; }

        [FromQuery(Name = "incluir_endereco")]
        [JsonPropertyName("incluir_endereco")]
        public bool IncluirEndereco { get; init; } = false;
    }
}