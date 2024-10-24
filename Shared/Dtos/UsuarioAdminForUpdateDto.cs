using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAdminForUpdateDto(string Password): UsuarioForUpdateDto(Password)
    {
        
    }
}