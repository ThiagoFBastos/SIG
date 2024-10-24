using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Pagination.Abstract;

namespace Shared.Pagination
{
    public record class GetTurmasOptions: GetOptions
    {
        [JsonPropertyName("prefixo_disciplina")]
        public string? PrefixoDisciplina { get; init; }

        [JsonPropertyName("ano_escolar")]
        public int? AnoEscolar { get; init; }

        [JsonPropertyName("a_partir_data")]
        public DateTime ApartirData { get; init; } = DateTime.MinValue;

        [JsonPropertyName("ate_data")]
        public DateTime AteData { get; init; } = DateTime.MaxValue;

        [JsonPropertyName("incluir_alunos")]
        public bool IncluirAlunos { get; init; } = false;

        [JsonPropertyName("incluir_professor")]
        public bool IncluirProfessor { get; init; } = false;
    }
}