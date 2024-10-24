using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAlunoForUpdateDto(string Password): UsuarioForUpdateDto(Password)
    {
        
    }
}