using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public record class GetProfessorOptions
    {
        [FromQuery(Name = "incluir_turmas")]
        [JsonPropertyName("incluir_turmas")]
        public bool IncluirTurmas { get; init; } = false;

        [FromQuery(Name = "incluir_endereco")]
        [JsonPropertyName("incluir_endereco")]
        public bool IncluirEndereco { get; init; } = false;
    }
} 