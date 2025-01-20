using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class ChangeUsuarioPasswordDto
    {
        [JsonPropertyName("new_password")]
        public required string NewPassword { get; set; }

        [JsonPropertyName("old_password")]
        public required string OldPassword { get; set; }
    }
}
