using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public record class GetAlunoTurmaOptions
    {
        [JsonPropertyName("incluir_aluno")]
        public bool IncluirAluno { get; init; } = false;
    }
}