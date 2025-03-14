using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class AlunoTurmaChangeTurmaDto
    {
        [JsonPropertyName("codigo_turma")]
        public required Guid TurmaCodigo { get; init; }
    }
}
