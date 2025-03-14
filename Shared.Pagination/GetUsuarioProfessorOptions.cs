using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public class GetUsuarioProfessorOptions
    {
        [FromQuery(Name = "incluir_professor")]
        public required bool IncluirProfessor {  get; set; }

        [FromQuery(Name = "incluir_endereco")]
        public required bool IncluirEndereco { get; set; }
    }
}
