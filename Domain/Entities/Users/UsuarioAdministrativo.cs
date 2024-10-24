using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Abstract;

namespace Domain.Entities.Users
{
    [Table("usuarios_administrativos")]
    public class UsuarioAdministrativo: Usuario
    {
        [Required(ErrorMessage = "A matrícula do administrativo é necessária")]
        public required Guid AdministrativoMatricula { get; set; }
        public Administrativo? Administrativo { get; set; }
    }
} 