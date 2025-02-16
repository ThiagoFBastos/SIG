using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public class GetUsuarioAlunoOptions
    {
        [JsonPropertyName("incluir_aluno")]
        public required bool IncluirAluno { get; set; }
    }
}
