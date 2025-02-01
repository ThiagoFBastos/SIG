using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAlunoDto: UsuarioDto
    {
        [JsonPropertyName("aluno_matricula")]
        public required Guid AlunoMatricula { get; set; }

        [JsonPropertyName("aluno")]
        public required AlunoDto Aluno { get; set; }

        public bool Match(UsuarioAluno usuario)
        {
            return usuario.Email == Email && usuario.Id == Id && usuario.AlunoMatricula == AlunoMatricula;
        }
    }
}