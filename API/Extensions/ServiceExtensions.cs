using Domain.Repositories;
using Persistence.Context;
using Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using API.Validators;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Services.Mappers;
using System.Text.Json.Serialization;

namespace API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        { 
           services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });
        }
        public static void ConfigureRepositoryManager(this IServiceCollection services) 
            => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<RepositoryContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Persistence")));

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AdministrativoProfile));
            services.AddAutoMapper(typeof(AlunoProfile));
            services.AddAutoMapper(typeof(ProfessorProfile));
            services.AddAutoMapper(typeof(TurmaProfile));
            services.AddAutoMapper(typeof(AlunoTurmaProfile));
            services.AddAutoMapper(typeof(EnderecoProfile));
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAdministrativoService, AdministrativoService>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddScoped<IAlunoTurmaService, AlunoTurmaService>();
            services.AddScoped<IEnderecoService, EnderecoService>();
            services.AddScoped<IProfessorService, ProfessorService>();
            services.AddScoped<ITurmaService, TurmaService>();
        }

        public static void ConfigureValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<AdministrativoForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<AdmnistrativoForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<AlunoForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<AlunoForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<AlunoTurmaForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<AlunoTurmaForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<EnderecoForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<EnderecoForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<ProfessorForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<ProfessorForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<TurmaForCreateValidator>();
            services.AddValidatorsFromAssemblyContaining<TurmaForUpdateValidator>();
            services.AddValidatorsFromAssemblyContaining<GetTurmasOptionsValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAdministrativosOptionsValidator>();
            services.AddValidatorsFromAssemblyContaining<GetProfessoresOptionsValidator>();
            services.AddValidatorsFromAssemblyContaining<GetAlunosOptionsValidator>();
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes();
               
                string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SIG API",
                    Version = "v1",
                    Description = "API para um sistema gerenciador de uma escola"
                });
            });
        }

        public static void ConfigureJsonOptions(this IServiceCollection services)
            => services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
    }
}