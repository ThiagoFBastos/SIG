using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class AlunoTurmaDto: AlunoTurmaSemNotaDto
    {
        [JsonPropertyName("nota")]
        public required double Nota { get; set; }

        [JsonPropertyName("turma")]
        public TurmaDto? Turma { get; set; }
        
        public override bool Match(AlunoTurma alunoTurma)
        {
            return base.Match(alunoTurma) && Nota == alunoTurma.Nota;
        }
    }
}