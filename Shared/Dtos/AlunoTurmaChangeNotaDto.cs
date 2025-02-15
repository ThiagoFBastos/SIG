using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class AlunoTurmaChangeNotaDto
    {
        [JsonPropertyName("nota")]
        public required double Nota { get; set; }
    }
}
