using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public record class GetProfessorOptions
    {
        [JsonPropertyName("incluir_turmas")]
        public bool IncluirTurmas { get; init; } = false;

        [JsonPropertyName("incluir_endereco")]
        public bool IncluirEndereco { get; init; } = false;
    }
} 