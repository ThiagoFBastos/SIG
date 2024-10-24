namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IAdministrativoRepository AdministrativoRepository { get; }
        IAlunoRepository AlunoRepository { get; }
        IAlunoTurmaRepository AlunoTurmaRepository {get; }
        IEnderecoRepository EnderecoRepository { get; }
        IProfessorRepository ProfessorRepository { get; }
        ITurmaRepository TurmaRepository { get; }
        IUsuarioAdminRepository UsuarioAdminRepository { get; }
        IUsuarioAdministrativoRepository UsuarioAdministrativoRepository { get; }
        IUsuarioAlunoRepository UsuarioAlunoRepository { get; }
        IUsuarioProfessorRepository UsuarioProfessorRepository { get; }
        Task SaveAsync(); 
    }
}