using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Dtos.Abstract;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class AlunoForUpdateDto: IntegranteForUpdateDto
    {
        [JsonPropertyName("ano_escolar")]
        public required int AnoEscolar { get; init; }

        [JsonPropertyName("status")]
        public required int Status { get; init; }

        [JsonPropertyName("turno")]
        public required int Turno { get; init; }

        public override bool Match(object? obj)
        {
            var aluno = obj as Aluno;

            if(aluno is null)
                return false;

            return base.Match(obj)
            && AnoEscolar == (int)aluno.AnoEscolar
            && Status == (int)aluno.Status
            && Turno == (int)aluno.Turno;
        }
    }
}