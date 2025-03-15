using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public class GetUsuarioAlunoOptions
    {
        [FromQuery(Name = "incluir_aluno")]
        [JsonPropertyName("incluir_aluno")]
        public required bool IncluirAluno { get; set; }

        [FromQuery(Name = "incluir_endereco")]
        [JsonPropertyName("incluir_endereco")]
        public required bool IncluirEndereco { get; set; }

        [FromQuery(Name = "incluir_turma")]
        [JsonPropertyName("incluir_turma")]
        public required bool IncluirTurma {  get; set; }
    }
}
