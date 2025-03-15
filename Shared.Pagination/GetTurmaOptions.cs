using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public record class GetTurmaOptions
    {
        [FromQuery(Name = "incluir_alunos")]
        [JsonPropertyName("incluir_alunos")]
        public bool IncluirAlunos { get; init; } = false;

        [FromQuery(Name = "incluir_professor")]
        [JsonPropertyName("incluir_professor")]
        public bool IncluirProfessor { get; init; } = false;
    }
}