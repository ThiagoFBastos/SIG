using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    [Table("turmas")]
    public class Turma
    {
        [Key]
        
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "O professor é necessário")]
        public required Guid ProfessorMatricula { get; set; }

        public Professor? Professor { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "A disciplina tem entre 3 e 50 caracteres")]
        [Required(ErrorMessage = "A disciplina é necessária")]
        public required string Disciplina { get; set; }

        [Required(ErrorMessage = "O ano escolar é necessário")]
        public required Periodo AnoEscolar { get; set; }

        [Required(ErrorMessage = "A data de inicio da turma é necessária")]
        public required DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "A data de término da turma é necessária")]
        public required DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O horário de inicio da aula é necessário")]
        public required DateTime HorarioAulaInicio { get; set; }

        [Required(ErrorMessage = "O horário de fim da aula é necessário")]
        public required DateTime HorarioAulaFim { get; set; }
        public List<AlunoTurma> Alunos { get; } = [];
    }
}