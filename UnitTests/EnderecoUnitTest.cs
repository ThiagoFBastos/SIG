using Xunit;
using Moq;
using Domain.Exceptions;
using Domain.Entities;
using API.Controllers;
using Services.Contracts;
using Shared.Dtos;
using Domain.Entities.Enums;
using AutoMapper;
using Services;
using Microsoft.Extensions.Logging;
using Services.Mappers;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace UnitTests;

public class EnderecoUnitTest
{
    private readonly IMapper _mapper;
    private readonly EnderecoController _enderecoController;
    private readonly Mock<IRepositoryManager> _repositoryManager;
    private readonly IEnderecoService _enderecoService;
    private readonly ITestOutputHelper _output;
    public EnderecoUnitTest(ITestOutputHelper output)
    {
        _output = output;
        var config = new MapperConfiguration(cfg => cfg.AddProfile<EnderecoProfile>());

        _mapper = config.CreateMapper();

        _repositoryManager = new Mock<IRepositoryManager>();

        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

        var logger = factory.CreateLogger<EnderecoService>();

        _enderecoService = new EnderecoService(_repositoryManager.Object, logger, _mapper);

        _enderecoController = new EnderecoController(_enderecoService);
    }

    private bool CompareEqualEnderecos(EnderecoForUpdateDto enderecoForUpdate, Endereco endereco)
    {
        return enderecoForUpdate.Casa == endereco.Casa
        && enderecoForUpdate.CEP == endereco.CEP
        && enderecoForUpdate.Cidade == endereco.Cidade
        && enderecoForUpdate.Complemento == endereco.Complemento
        && enderecoForUpdate.Estado == (int)endereco.Estado
        && enderecoForUpdate.Rua == endereco.Rua;   
    }

    [Fact]
    public async Task Test_Endereco_Update_Must_Work()
    {
        EnderecoForUpdateDto enderecoForUpdate = new EnderecoForUpdateDto
        {
            Cidade = "Rio de Janeiro",
            Estado = (int)Estado.RJ,
            CEP = "20230010",
            Rua = "Rua Teresopolis",
            Casa = 86
        };

        Endereco endereco = _mapper.Map<Endereco>(enderecoForUpdate);

        Guid id = Guid.NewGuid();

        endereco.Id = id;

        _repositoryManager.Setup(x => x.EnderecoRepository.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync(endereco);
        _repositoryManager.Setup(x => x.EnderecoRepository.UpdateEndereco(It.IsAny<Endereco>())).Verifiable();
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        var response = await _enderecoController.Update(id, enderecoForUpdate);

        _repositoryManager.VerifyAll();
        Assert.NotNull(response);
        Assert.True(response is OkObjectResult);

        var okResponse = response as OkObjectResult;

        Assert.NotNull(okResponse);
        
        var result = okResponse.Value as EnderecoDto;

        Assert.NotNull(result);

        Assert.True(CompareEqualEnderecos(enderecoForUpdate, endereco));
        Assert.True(result.Match(endereco));
        Assert.Equal(okResponse.StatusCode, 200);
    }

    [Fact]
    public async Task Test_Endereco_Update_Must_Throw_NotFoundException()
    {
        EnderecoForUpdateDto enderecoForUpdate = new EnderecoForUpdateDto
        {
            Cidade = "Rio de Janeiro",
            Estado = (int)Estado.RJ,
            CEP = "20230010",
            Rua = "Rua Teresopolis",
            Casa = 86
        };

        Guid id = Guid.NewGuid();

        _repositoryManager.Setup(x => x.EnderecoRepository.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync((Endereco?)null);
        
        try
        {
            var response = await _enderecoController.Update(id, enderecoForUpdate);
            _repositoryManager.VerifyAll();
            Assert.Fail();
        }
        catch(NotFoundException ex)
        {
            Assert.Equal($"O endereço com id: {id} não foi encontrado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_Endereco_Get_Must_Work()
    {
        Guid id = Guid.NewGuid();

        Endereco endereco = new Endereco
        {
            Id = id,
            Cidade = "Rio de Janeiro",
            Estado = Estado.RJ,
            Rua = "Rua Teresópolis",
            Casa = 86,
            CEP = "20230010" 
        };

        _repositoryManager.Setup(x => x.EnderecoRepository.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync(endereco);

        var response = await _enderecoController.Get(id);

        Assert.NotNull(response);
        Assert.True(response is OkObjectResult);

        var okResponse = response as OkObjectResult;

        Assert.NotNull(okResponse);

        var enderecoDto = okResponse.Value as EnderecoDto;

        Assert.NotNull(enderecoDto);

        Assert.True(enderecoDto.Match(endereco));
        Assert.Equal(okResponse.StatusCode, 200);
    }

    [Fact]
    public async Task Test_Endereco_Get_Must_Throw_NotFoundException()
    {
        Guid id = Guid.NewGuid();

        Endereco endereco = new Endereco
        {
            Id = id,
            Cidade = "Rio de Janeiro",
            Estado = Estado.RJ,
            Rua = "Rua Teresópolis",
            Casa = 86,
            CEP = "20230010"
        };

        _repositoryManager.Setup(x => x.EnderecoRepository.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync((Endereco?)null);

        try
        {
            var response = await _enderecoController.Get(id);
            Assert.Fail();
        }
        catch(NotFoundException ex)
        {
            Assert.Equal($"O endereço com id: {id} não foi encontrado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }
}