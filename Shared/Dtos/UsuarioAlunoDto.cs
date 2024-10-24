using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAlunoDto: UsuarioDto
    {
        [JsonPropertyName("matricula_aluno")]
        public required Guid Matricula { get; set; }

        [JsonPropertyName("aluno")]
        public required AlunoDto Aluno { get; set; }
    }
}