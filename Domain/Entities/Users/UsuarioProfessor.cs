using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Abstract;

namespace Domain.Entities.Users
{
    [Table("usuarios_professores")]
    public class UsuarioProfessor: Usuario
    {
        [Required(ErrorMessage = "A matrícula do professor é necessária")]
        public required Guid ProfessorMatricula { get; set; }
        public Professor? Professor { get; set; }
    }
}