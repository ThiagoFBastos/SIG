using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAdministrativoDto: UsuarioDto
    {
        [JsonPropertyName("matricula_administrativo")]
        public required Guid AdministrativoMatricula { get; set; }

        [JsonPropertyName("administrativo")]
        public AdministrativoDto? Administrativo { get; set; }

        public bool Match(UsuarioAdministrativo usuario)
        {
            return usuario.Email == Email && usuario.Id == Id && usuario.AdministrativoMatricula == AdministrativoMatricula;
        }
    }
} 