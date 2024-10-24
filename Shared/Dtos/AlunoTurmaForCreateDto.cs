using System.Text.Json.Serialization;

namespace Shared.Dtos
{
    public record class AlunoTurmaForCreateDto
    {
        [JsonPropertyName("matricula_aluno")]
        public required Guid AlunoMatricula { get; init; }

        [JsonPropertyName("codigo_turma")]
        public required Guid TurmaCodigo { get; init; }
    }
}    