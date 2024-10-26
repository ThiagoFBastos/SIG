using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class AlunoTurmaDto
    {
        [JsonPropertyName("codigo")]
        public required Guid Codigo { get; set; }
 
        [JsonPropertyName("matricula_aluno")]
        public required Guid AlunoMatricula { get; set; }

        [JsonPropertyName("aluno")]
        public AlunoDto? Aluno { get; set;}

        [JsonPropertyName("codigo_turma")]
        public required Guid TurmaCodigo { get; set; }

        [JsonPropertyName("turma")]
        public TurmaDto? Turma { get; set; }

        [JsonPropertyName("nota")]
        public required double Nota { get; set; }

        public bool Match(AlunoTurma alunoTurma)
        {
            return AlunoMatricula == alunoTurma.AlunoMatricula &&
            TurmaCodigo == alunoTurma.TurmaCodigo
            && Nota == alunoTurma.Nota;
        }
    }
}