using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Dtos.Abstract;
using Domain.Entities;

namespace Shared.Dtos
{
    public record class AlunoDto: IntegranteDto
    {
        [JsonPropertyName("matricula")]
        public required Guid Matricula {get; set;}

        [JsonPropertyName("ano_escolar")]
        public required int AnoEscolar {get; set;}

        [JsonPropertyName("status")]
        public required int Status {get; set;}

        [JsonPropertyName("turno")]
        public required int Turno {get; set;}

        [JsonPropertyName("turmas")]
        public List<AlunoTurmaDto>? Turmas { get; set; }

        [JsonPropertyName("media")]
        public double? Media { get; set; }
        public override bool Match(object? obj)
        {
            var aluno = obj as Aluno;

            if(aluno is null)
                return false;

            return base.Match(obj)
            && Matricula == aluno.Matricula
            && AnoEscolar == (int)aluno.AnoEscolar
            && Status == (int)aluno.Status
            && Turno == (int)aluno.Turno
            && Media == aluno.Media;
        }
    }
}