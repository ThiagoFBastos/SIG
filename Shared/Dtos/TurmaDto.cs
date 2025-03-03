using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class TurmaDto: TurmaSemAlunoTurmaDto
    {
        [JsonPropertyName("professor")]
        public ProfessorDto? Professor { get; set; }

        [JsonPropertyName("alunos")]
        public List<AlunoTurmaDto>? Alunos { get; set; }
    }
} 