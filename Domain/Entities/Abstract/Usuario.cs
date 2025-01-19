using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Abstract
{
    public abstract class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O email é necessário")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "O email deve conter entre 1 e 255 caracteres")]
        [DataType(DataType.EmailAddress, ErrorMessage = "O email tem formato incorreto")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A senha é necessária")]
        public required string PasswordHash { get; set; }

        [Required(ErrorMessage = "O parâmetro de salt string é obrigatório")]
        public required string SalString { get; set; }
    }
}