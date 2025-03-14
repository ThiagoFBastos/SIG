using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos.Abstract
{
    public abstract record class TurmaSemAlunoTurmaDto
    {
        [JsonPropertyName("codigo")]
        public required Guid Codigo { get; set; }

        [JsonPropertyName("matricula_professor")]
        public required Guid ProfessorMatricula { get; set; }

        [JsonPropertyName("disciplina")]
        public required string Disciplina { get; set; }

        [JsonPropertyName("ano_escolar")]
        public required int AnoEscolar { get; set; }

        [JsonPropertyName("data_inicio")]
        public required DateTime DataInicio { get; set; }

        [JsonPropertyName("data_fim")]
        public required DateTime DataFim { get; set; }

        [JsonPropertyName("horario_aula_inicio")]
        public required DateTime HorarioAulaInicio { get; set; }

        [JsonPropertyName("horario_aula_fim")]
        public required DateTime HorarioAulaFim { get; set; }

        public bool Match(object? obj)
        {
            var turma = obj as Turma;

            if (turma is null)
                return false;

            return Codigo == turma.Codigo
            && ProfessorMatricula == turma.ProfessorMatricula
            && Disciplina == turma.Disciplina
            && AnoEscolar == (int)turma.AnoEscolar
            && DataInicio == turma.DataInicio
            && DataFim == turma.DataFim
            && HorarioAulaInicio == turma.HorarioAulaInicio
            && HorarioAulaFim == turma.HorarioAulaFim;
        }
    }
}
