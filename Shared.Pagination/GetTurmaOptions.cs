using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public record class GetTurmaOptions
    {
        [JsonPropertyName("incluir_alunos")]
        public bool IncluirAlunos { get; init; } = false;

        [JsonPropertyName("incluir_professor")]
        public bool IncluirProfessor { get; init; } = false;
    }
}