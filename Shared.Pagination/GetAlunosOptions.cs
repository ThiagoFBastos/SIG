using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Pagination.Abstract;

namespace Shared.Pagination
{
    public record class GetAlunosOptions: GetIntegranteOptions
    {
        [JsonPropertyName("ano_escolar")]
        public int? AnoEscolar { get; init; }

        [JsonPropertyName("turno")]
        public int? Turno { get; init; }

        [JsonPropertyName("incluir_turmas")]
        public bool IncluirTurmas { get; init; } = false;
    } 
} 