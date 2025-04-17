using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IntegrationTests
{
    public class CustomApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        private readonly SqliteConnection _connection;

        public CustomApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(IDbContextOptionsConfiguration<RepositoryContext>));

                if(dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                if(dbConnectionDescriptor != null)
                    services.Remove(dbConnectionDescriptor);

                services.AddDbContext<RepositoryContext>((container, options) => 
                {
                    options.UseSqlite(_connection, b => b.MigrationsAssembly("Persistence"))
                            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RepositoryContext>();

                await db.Database.MigrateAsync();
            });

            builder.UseEnvironment("Development");
        }
    }
}
