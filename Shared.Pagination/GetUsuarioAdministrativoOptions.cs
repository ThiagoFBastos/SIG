using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Pagination
{
    public class GetUsuarioAdministrativoOptions
    {
        [JsonPropertyName("incluir_administrativo")]
        public required bool IncluirAdministrativo { get; set; }

        [JsonPropertyName("incluir_endereco")]
        public required bool IncluirEndereco { get; set; }
    }
}
