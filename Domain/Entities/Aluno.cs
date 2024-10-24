using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Abstract;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    [Table("alunos")]
    public class Aluno: Integrante
    {
        [Key] 
        public Guid Matricula { get; set; }

        [Required(ErrorMessage = "O período escolar é necessário")]
        public required Periodo AnoEscolar { get; set; }

        [Required(ErrorMessage = "O status da matrícula é necessário")]
        public required StatusMatricula Status { get; set; }

        [Required(ErrorMessage = "O turno é necessário")]
        public required Turno Turno { get; set; }

        public List<AlunoTurma> Turmas { get; set; } = [];

        [NotMapped]
        public double? Media { get; set; }
    }
}