using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("alunos_turma")]
    public class AlunoTurma
    {
        [Key]
        public Guid Codigo { get; set; }

        [Required(ErrorMessage = "A matrícula do aluno é necessária")]
        public required Guid AlunoMatricula { get; set; }

        public Aluno? Aluno { get; set; }

        [Required(ErrorMessage = "O código da turma é necessário")]
        public required Guid TurmaCodigo { get; set; }

        public Turma? Turma { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "A nota deve ser não negativa")]
        public double Nota { get; set; } = 0;
    }
}  