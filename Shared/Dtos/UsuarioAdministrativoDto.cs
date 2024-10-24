using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAdministrativoDto: UsuarioDto
    {
        [JsonPropertyName("matricula_administrativo")]
        public required Guid Matricula { get; set; }

        [JsonPropertyName("administrativo")]
        public required AdministrativoDto AdministrativoDto { get; set; }
    }
} 