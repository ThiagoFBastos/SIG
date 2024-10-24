using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Pagination.Abstract;

namespace Shared.Pagination
{
    public record class GetProfessoresOptions: GetFuncionariosOptions
    {
        [JsonPropertyName("incluir_turmas")]
        public bool IncluirTurmas { get; init; } = false;
    }
}