using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioProfessorDto: UsuarioDto
    {
        [JsonPropertyName("matricula_professor")]
        public required Guid ProfessorMatricula { get; set; }

        [JsonPropertyName("professor")]
        public required ProfessorDto Professor { get; set; }
    }
}