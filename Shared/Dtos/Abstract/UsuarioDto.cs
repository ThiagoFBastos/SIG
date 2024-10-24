using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos.Abstract
{
    public abstract record class UsuarioDto
    {
         [JsonPropertyName("id")]
         public required Guid Id { get; set;}

         [JsonPropertyName("email")]
         public required string Email { get; set; }
    }
}