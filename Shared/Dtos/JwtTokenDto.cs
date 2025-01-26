using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class JwtTokenDto
    {
        [JsonPropertyName("token")]
        public required string Token { get; set; }
    }
}
