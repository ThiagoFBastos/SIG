using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Entities.Enums;

namespace Domain.Entities.Abstract
{
    public abstract class Integrante
    {
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF tem 11 caracteres")]
        [Required(ErrorMessage = "O CPF é necessário")]
        public required string CPF { get; set; }

        [Required(ErrorMessage = "O RG é necessário")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "O RG tem entre 8 e 9 caracteres")]
        public required string RG { get; set; }

        [StringLength(255, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 255 caracteres")]
        [Required(ErrorMessage = "O nome é necessário")]
        public required string NomeCompleto { get; set; }

        [StringLength(255, MinimumLength = 1, ErrorMessage = "O email tem entre 1 e 255 caracteres")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email com formato inválido")]
        [Required(ErrorMessage = "O email é necessário")]
        public required string Email { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "O celular tem 11 caracteres")]
        [Required(ErrorMessage = "O celular é necessário")]
        public required string Celular { get; set; }

        [Required(ErrorMessage = "A data de nascimento é necessária")]
        public required DateTime DataNascimento { get; set; }
 
        [Required(ErrorMessage = "A data de chegada é necessária")]
        public DateTime DataChegada { get; set; }

        [Required(ErrorMessage = "O id do endereço é necessário")]
        public required Guid EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        [Required(ErrorMessage = "O sexo da pessoa é necessário")]
        public required Sexo Sexo { get; set; }
    }
}