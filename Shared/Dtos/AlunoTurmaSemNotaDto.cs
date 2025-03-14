using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record class AlunoTurmaSemNotaDto
    {
        [JsonPropertyName("codigo")]
        public required Guid Codigo { get; set; }

        [JsonPropertyName("matricula_aluno")]
        public required Guid AlunoMatricula { get; set; }

        [JsonPropertyName("aluno")]
        public AlunoDto? Aluno { get; set; }

        [JsonPropertyName("codigo_turma")]
        public required Guid TurmaCodigo { get; set; }

        public virtual bool Match(AlunoTurma alunoTurma)
        {
            return Codigo == alunoTurma.Codigo && AlunoMatricula == alunoTurma.AlunoMatricula &&
            TurmaCodigo == alunoTurma.TurmaCodigo;
        }
    }
}
