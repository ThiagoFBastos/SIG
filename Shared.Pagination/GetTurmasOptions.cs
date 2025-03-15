using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Pagination.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public record class GetTurmasOptions: GetOptions
    {
        [FromQuery(Name = "prefixo_disciplina")]
        [JsonPropertyName("prefixo_disciplina")]
        public string? PrefixoDisciplina { get; init; }

        [FromQuery(Name = "ano_escolar")]
        [JsonPropertyName("ano_escolar")]
        public int? AnoEscolar { get; init; }

        [FromQuery(Name = "a_partir_data")]
        [JsonPropertyName("a_partir_data")]
        public DateTime ApartirData { get; init; } = DateTime.MinValue;

        [FromQuery(Name = "ate_data")]
        [JsonPropertyName("ate_data")]
        public DateTime AteData { get; init; } = DateTime.MaxValue;

        [FromQuery(Name = "incluir_alunos")]
        [JsonPropertyName("incluir_alunos")]
        public bool IncluirAlunos { get; init; } = false;

        [FromQuery(Name = "incluir_professor")]
        [JsonPropertyName("incluir_professor")]
        public bool IncluirProfessor { get; init; } = false;
    }
}