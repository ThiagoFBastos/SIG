using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class UsuarioAlunoForCreateDto: UsuarioAdminForCreateDto
    {
        [JsonPropertyName("aluno_matricula")]
        public required Guid AlunoMatricula { get; set; }
    }
}
