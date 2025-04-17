using Domain.Entities;
using Domain.Entities.Enums;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Shared.Dtos;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;

namespace IntegrationTests
{
    public class EnderecoControllerTest: IClassFixture<CustomApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;

        private readonly CustomApplicationFactory<Program> _factory;

        public EnderecoControllerTest(CustomApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var credentials = new
            {
                email = "juliamagalhaes@outlook.com",
                password = "15158114099Aa$$"
            };

            var response = await _client.PostAsJsonAsync("/api/UsuarioAdmin/login", credentials);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            JwtTokenDto? token = JsonSerializer.Deserialize<JwtTokenDto>(content);

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token?.Token}");
        }

        [Fact]
        public async Task Test_Endereco_Must_Work()
        {
            Endereco endereco = new Endereco
            {
                Cidade = "Rio de Janeiro",
                Estado = Estado.RJ,
                Rua = "Rua Sete de Setembro",
                CEP = "21100412",
                Casa = 10,
                Complemento = "Fundos casa 3"
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            context.Set<Endereco>().Add(endereco);
            await context.SaveChangesAsync();

            var response = await _client.GetAsync($"/api/Endereco/{endereco.Id}");

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            EnderecoDto? enderecoRetornado = JsonSerializer.Deserialize<EnderecoDto>(content);

            enderecoRetornado.Should().Be(new EnderecoDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                CEP = "21100412",
                Casa = 10,
                Complemento = "Fundos casa 3",
                Id = endereco.Id
            });
        }

        [Fact]
        public async Task Test_Get_Endereco_Must_Not_Find()
        {
            Guid enderecoId = Guid.NewGuid();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            Endereco? endereco = await context.Set<Endereco>().FindAsync(enderecoId);

            endereco.Should().BeNull();

            var response = await _client.GetAsync($"/api/Endereco/{enderecoId}");

            response.Should().BeOfType<HttpResponseMessage>().Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task Test_Update_Endereco_Must_Work()
        {
            Endereco endereco = new Endereco
            {
                Cidade = "Rio de Janeiro",
                Estado = Estado.RJ,
                Rua = "Rua Sete de Setembro",
                CEP = "21100412",
                Casa = 10,
                Complemento = "Fundos casa 3"
            };

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep = "11340400",
                casa = 71
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            context.Set<Endereco>().Add(endereco);
            await context.SaveChangesAsync();

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{endereco.Id}", payload);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            EnderecoDto? enderecoAtualizado = JsonSerializer.Deserialize<EnderecoDto>(content);

            enderecoAtualizado.Should().Be(new EnderecoDto
            {
                Cidade = "São Paulo",
                Estado = (int)Estado.SP,
                Rua = "Vila do Chaves",
                CEP = "11340400",
                Casa = 71,
                Id = endereco.Id
            });

            Endereco? enderecoDoBanco = await context.Enderecos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == endereco.Id);

            enderecoDoBanco.Should().NotBeNull();

            string obj = JsonSerializer.Serialize(enderecoDoBanco);

            enderecoAtualizado.Match(enderecoDoBanco).Should().BeTrue();
        }

        [Fact]
        public async Task Test_Uodate_Endereco_Must_Not_Find()
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep = "11340400",
                casa = 71
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
            Endereco? endereco = await context.Set<Endereco>().FindAsync(enderecoId);

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? title = obj?["title"]?.ToString();
            title.Should().Be($"O endereço com id: {enderecoId} não foi encontrado");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(51)]
        public async Task Test_Update_Endereco_Must_Fail_Because_Cidade_Length_Is_Not_Valid(int length)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = new string('x', length),
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep = "11340400",
                casa = 71
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? cidade = obj?["errors"]?["cidade"]?[0]?.ToString();
            cidade.Should().Be("a cidade possui entre 1 e 50 caracteres");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(27)]
        public async Task Test_Update_Endereco_Must_Fail_Because_Estado_Value_Is_Not_Valid(int estadoId)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = estadoId,
                rua = "Vila do Chaves",
                cep = "11340400",
                casa = 71
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);
            
            string? estado = obj?["errors"]?["estado"]?[0]?.ToString();
            estado.Should().Be("o estado é um código válido");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("22")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("1234567")]
        [InlineData("123456789")]
        public async Task Test_Update_Endereco_Must_Fail_Because_CEP_Length_Is_Invalid(string cep)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep,
                casa = 71
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? CEP = obj?["errors"]?["cep"]?[0]?.ToString();

            CEP.Should().Be("o cep possui 8 digitos");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(301)]
        public async Task Test_Update_Endereco_Must_Fail_Because_Rua_Length_Is_Invalid(int length)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = new string('x', length),
                cep = "12345678",
                casa = 71
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? rua = obj?["errors"]?["rua"]?[0]?.ToString();

            rua.Should().Be("a rua possui entre 3 e 300 caracteres");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Test_Update_Endereco_Must_Fail_Because_Casa_Is_Not_Positive(int numero)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep = "12345678",
                casa = numero
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? casa = obj?["errors"]?["casa"]?[0]?.ToString();

            casa.Should().Be("o número da casa é positivo");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(51)]
        public async Task Test_Update_Endereco_Must_Fail_Because_Complemento_Length_Is_Invalid(int length)
        {
            Guid enderecoId = Guid.NewGuid();

            var payload = new
            {
                cidade = "São Paulo",
                estado = (int)Estado.SP,
                rua = "Vila do Chaves",
                cep = "12345678",
                casa = 71,
                complemento = new string('x', length)
            };

            var response = await _client.PutAsJsonAsync($"/api/Endereco/{enderecoId}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            string content = await response.Content.ReadAsStringAsync();

            JsonNode? obj = JsonObject.Parse(content);

            string? complemento = obj?["errors"]?["complemento"]?[0]?.ToString();

            complemento.Should().Be("o complemento possui até 50 caracteres");
        }
    }
}
