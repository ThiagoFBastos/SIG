﻿using Shared.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class UsuarioAdministrativoForCreateDto: UsuarioForCreateDto
    {
        [JsonPropertyName("matricula")]
        public required Guid AdministrativoMatricula { get; set; }
    }
}
