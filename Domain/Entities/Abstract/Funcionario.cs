using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Abstract
{
    public abstract class Funcionario: Integrante
    {
        [Key]
        public Guid Matricula { get; set; }
        
        [Required(ErrorMessage = "O cargo é necessário")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O cargo deve possuir entre 3 e 50 caracteres")]
        public required string Cargo { get; set; } // diminuir complexidade
        
        [Required(ErrorMessage = "O salário é necessário")]
        public required decimal Salario { get; set; }

        [StringLength(50, MinimumLength = 1, ErrorMessage = "O banco deve conter entre 1 e 50 caracteres")]
        [Required(ErrorMessage = "O banco é necessário")]
        public required string Banco { get; set; } // diminuir complexidade

        [StringLength(50, MinimumLength = 1, ErrorMessage = "A conta corrente deve conter entre 1 e 255 caracteres")]
        [Required(ErrorMessage = "A conta corrente é necessária")]
        public required string ContaCorrente { get; set; } // diminuir complexidade

        [Required(ErrorMessage = "O status do emprego é necessário")]
        public required StatusEmprego Status { get; set; }

        public DateTime? DataDemissao { get; set; }

        [Required(ErrorMessage = "O horário de ínicio do expediente é necessário")]
        public required DateTime HorarioInicioExpediente { get; set; }

        [Required(ErrorMessage = "O horário do fim do expediente é necessário")]
        public required DateTime HorarioFimExpediente { get; set; }
    }
} 