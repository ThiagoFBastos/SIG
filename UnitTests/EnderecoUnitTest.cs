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
    private readonly EnderecoService _enderecoService;
    private readonly Mock<IRepositoryManager> _repositoryManager;
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
        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        endereco.Id = id;

        enderecoRepository.Setup(x => x.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync(endereco);
        enderecoRepository.Setup(x => x.UpdateEndereco(It.IsAny<Endereco>())).Verifiable();
        _repositoryManager.SetupGet(x => x.EnderecoRepository).Returns(enderecoRepository.Object);
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        EnderecoDto enderecoDto = await _enderecoService.AtualizarEndereco(id, enderecoForUpdate);

        _repositoryManager.VerifyAll();
        enderecoRepository.VerifyAll();

        Assert.True(enderecoDto.Match(endereco));
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
        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(x => x.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync((Endereco?)null);
        _repositoryManager.SetupGet(x => x.EnderecoRepository).Returns(enderecoRepository.Object);

        try
        {
            _ = await _enderecoService.AtualizarEndereco(id, enderecoForUpdate);
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
        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(x => x.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync(endereco);
        _repositoryManager.SetupGet(x => x.EnderecoRepository).Returns(enderecoRepository.Object);

        EnderecoDto enderecoDto = await _enderecoService.ObterEnderecoPorId(id);

        Assert.True(enderecoDto.Match(endereco));
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
        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(x => x.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync((Endereco?)null);
        _repositoryManager.SetupGet(x => x.EnderecoRepository).Returns(enderecoRepository.Object);

        try
        {
            _ = await _enderecoService.ObterEnderecoPorId(id);
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
    public async Task Test_Add_Endereco_Must_Work()
    {
        EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
        {
            Cidade = "Rio de Janeiro",
            Estado = (int)Estado.RJ,
            Rua = "Rua Sete de Setembro",
            CEP = "21100412",
            Casa = 10,
            Complemento = "Fundos casa 3"
        };

        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(x => x.AddEndereco(It.IsAny<Endereco>())).Verifiable();
        _repositoryManager.SetupGet(x => x.EnderecoRepository).Returns(enderecoRepository.Object);
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        Guid codigo = await _enderecoService.CadastrarEndereco(enderecoForCreate);

        enderecoRepository.VerifyAll();
        _repositoryManager.VerifyAll();
    }

    [Fact]
    public async Task Test_Delete_Endereco_Must_Work()
    {
        Endereco endereco = new Endereco
        {
            Id = Guid.NewGuid(),
            Cidade = "Rio de Janeiro",
            Estado = Estado.RJ,
            Rua = "Rua Sete de Setembro",
            CEP = "21100412",
            Casa = 10,
            Complemento = "Fundos casa 3"
        };

        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(y => y.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync(endereco);
        enderecoRepository.Setup(y => y.DeleteEndereco(It.IsAny<Endereco>())).Verifiable();
        _repositoryManager.SetupGet(y => y.EnderecoRepository).Returns(enderecoRepository.Object);
        _repositoryManager.Setup(y => y.SaveAsync()).Verifiable();

        await _enderecoService.DeletarEndereco(endereco.Id);

        enderecoRepository.VerifyAll();
        _repositoryManager.VerifyAll();
    }

    [Fact]
    public async Task Test_Delete_Endereco_Shouldnt_Work_Endereco_Not_Exists()
    {
        Guid enderecoId = Guid.NewGuid();
        Mock<IEnderecoRepository> enderecoRepository = new Mock<IEnderecoRepository>();

        enderecoRepository.Setup(y => y.GetEnderecoAsync(It.IsAny<Guid>())).ReturnsAsync((Endereco?)null);
        _repositoryManager.SetupGet(y => y.EnderecoRepository).Returns(enderecoRepository.Object);

        try
        {
            await _enderecoService.DeletarEndereco(enderecoId);
            Assert.Fail();
        }
        catch(NotFoundException ex)
        {
            Assert.Equal($"O endereço com id: {enderecoId} não foi encontrado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }
}