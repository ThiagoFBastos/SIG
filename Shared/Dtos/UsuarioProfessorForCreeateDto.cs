using Shared.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class UsuarioProfessorForCreeateDto: UsuarioForCreateDto
    {
        [JsonPropertyName("professor_matricula")]
        public required Guid ProfessorMatricula { get; set; }
    }
}
