using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record class AlunoTurmaForUpdateDto
    {
        [JsonPropertyName("codigo_turma")]
        public required Guid TurmaCodigo { get; init; }

        [JsonPropertyName("nota")]
        public required double Nota { get; init; }
    }
}