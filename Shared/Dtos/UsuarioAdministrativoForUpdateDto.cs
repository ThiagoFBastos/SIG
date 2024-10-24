using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAdministrativoForUpdateDto(string Password): UsuarioForUpdateDto(Password)
    {
        
    }
}