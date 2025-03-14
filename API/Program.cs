using API.Extensions;
using FluentValidation;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.PropertyNameResolver = (type, member, expression) =>
{
    if (member != null)
    {
        var jsonProperty = member.GetCustomAttribute<JsonPropertyNameAttribute>();
        return jsonProperty?.Name ?? member.Name;
    }
    return null;
};

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureValidators();
builder.Services.ConfigureAutoMapper();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServices();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureJsonOptions();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureServiceOptions(builder.Configuration);

var app = builder.Build();

app.ConfigureExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
