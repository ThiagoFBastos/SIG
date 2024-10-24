using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioProfessorForUpdateDto(string Password): UsuarioForUpdateDto(Password)
    {
        
    }
}