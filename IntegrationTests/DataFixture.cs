using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Repositories;

namespace IntegrationTests
{
    public class DataFixture
    {
        public RepositoryManager RepositoryManager { get; }

        private const string _connectionString = "Host=localhost;Port=5432;Pooling=true;Database=SIG_Test;User Id=postgres;Password=admin@123;";

        public DataFixture()
        {
            DbContextOptions<RepositoryContext> options = new DbContextOptionsBuilder<RepositoryContext>()
              .UseNpgsql(_connectionString)
              .Options;

            RepositoryContext context = new RepositoryContext(options);

            RepositoryManager = new RepositoryManager(context);
        }
    }
}