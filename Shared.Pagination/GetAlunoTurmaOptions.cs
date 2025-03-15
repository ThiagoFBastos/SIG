using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public record class GetAlunoTurmaOptions
    {
        [FromQuery(Name = "incluir_aluno")]
        [JsonPropertyName("incluir_aluno")]
        public bool IncluirAluno { get; init; } = false;
    }
}