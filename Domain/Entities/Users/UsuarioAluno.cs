using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Abstract;

namespace Domain.Entities.Users
{
    [Table("usuarios_alunos")]
    public class UsuarioAluno: Usuario
    {
        [Required(ErrorMessage = "A matrícula do aluno é necessária")]
        public required Guid AlunoMatricula { get; set; }     
        public Aluno? Aluno { get; set; }
    }
} 