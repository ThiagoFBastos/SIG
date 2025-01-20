using Domain.Entities.Users;

namespace Domain.Repositories
{
    public interface IUsuarioAdminRepository
    {
        void AddUsuarioAdmin(UsuarioAdmin usuarioAdmin);
        void UpdateUsuarioAdmin(UsuarioAdmin usuarioAdmin);
        void DeleteUsuarioAdmin(UsuarioAdmin usuarioAdmin);
        Task<UsuarioAdmin?> GetUsuarioAdminByIdAsync(Guid id);
        Task<UsuarioAdmin?> GetUsuarioAdminByEmailAsync(string email);
        Task<List<UsuarioAdmin>> GetAllUsuarioAdmin();
    }
}