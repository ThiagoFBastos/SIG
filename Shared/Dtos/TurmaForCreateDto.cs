using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class TurmaForCreateDto
    {
        [JsonPropertyName("matricula_professor")]
        public required Guid ProfessorMatricula { get; init; }
        
        [JsonPropertyName("disciplina")]
        public required string Disciplina { get; init; }

        [JsonPropertyName("ano_escolar")]
        public required int AnoEscolar { get; init; }

        [JsonPropertyName("data_inicio")]
        public required DateTime DataInicio { get; init; }

        [JsonPropertyName("data_fim")]
        public required DateTime DataFim { get; init; }

        [JsonPropertyName("horario_aula_inicio")]
        public required DateTime HorarioAulaInicio { get; init; }

        [JsonPropertyName("horario_aula_fim")]
        public required DateTime HorarioAulaFim { get; init; }

        public bool Match(object? obj)
        {
            var turma = obj as Turma;

            if(turma is null)
                return false;

            return ProfessorMatricula == turma.ProfessorMatricula
            && Disciplina == turma.Disciplina
            && AnoEscolar == (int)turma.AnoEscolar
            && DataInicio == turma.DataInicio
            && DataFim == turma.DataFim
            && HorarioAulaInicio == turma.HorarioAulaInicio
            && HorarioAulaFim == turma.HorarioAulaFim;
        }
    }
}    