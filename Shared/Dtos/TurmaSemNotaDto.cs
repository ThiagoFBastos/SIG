using Newtonsoft.Json;
using Shared.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record class TurmaSemNotaDto: TurmaSemAlunoTurmaDto
    {
        [JsonPropertyName("alunos")]
        public List<AlunoTurmaSemNotaDto>? Alunos { get; set; }
    }
}
