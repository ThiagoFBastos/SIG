using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities
{
    [Table("enderecos")]
    public class Endereco
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A cidade é necessária")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "A cidade deve ter entre 2 e 50 caracteres")]
        public required string Cidade { get; set; }

        [Required(ErrorMessage = "O estado é necessário")]
        public required Estado Estado { get; set; }

        [Required(ErrorMessage = "O CEP é necessário")]
        [DataType(DataType.PostalCode, ErrorMessage = "O CEP está com formato inválido")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP é composto por 8 algarismos")]
        public required string CEP { get; set; }

        [Required(ErrorMessage = "A rua é necessária")]
        [StringLength(300, MinimumLength = 3, ErrorMessage = "A rua deve possuir entre 3 e 300 caracteres")]
        public required string Rua { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O número da casa é positivo")]
        [Required(ErrorMessage = "O número da casa é necessária")]
        public required int Casa { get; set; }
        
        [MaxLength(50, ErrorMessage = "O complemento deve ter até 50 caracteres")]
        public string? Complemento { get; set; }
    }
}