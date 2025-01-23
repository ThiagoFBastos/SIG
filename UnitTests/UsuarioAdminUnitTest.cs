using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence.Repositories;
using Services;
using Services.Contracts;
using Services.Mappers;
using Shared.Dtos;
using System.Security.Claims;
using Xunit.Abstractions;

namespace UnitTests;

public class UsuarioAdminUnitTest
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryManager> _repositoryManager;
    private readonly ITestOutputHelper _output;
    private readonly Mock<IPasswordHash> _passwordHash;
    private readonly Mock<ITokensService> _tokensService;
    private readonly UsuarioAdminService _usuarioAdminService;

    public UsuarioAdminUnitTest(ITestOutputHelper output)
    {
        _output = output;

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioAdminProfile>());
        _mapper = config.CreateMapper();

        ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

        var serviceLogger = factory.CreateLogger<UsuarioAdminService>();

        _repositoryManager = new Mock<IRepositoryManager>();
        _passwordHash = new Mock<IPasswordHash>();
        _tokensService = new Mock<ITokensService>();

        _usuarioAdminService = new UsuarioAdminService(serviceLogger, _tokensService.Object, _repositoryManager.Object, _mapper, _passwordHash.Object);
    }

    [Fact]
    public async Task Test_GetAll_Must_Work()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();
        List<UsuarioAdmin> admins = new List<UsuarioAdmin>()
        {
            new UsuarioAdmin
            {
                Id = Guid.NewGuid(),
                Email = "otaviomesquita@gmail.com",
                PasswordHash = "password",
                SalString = "salt"
            },
            new UsuarioAdmin
            {
                Id = Guid.NewGuid(),
                Email = "leticiasabetella@gmail.com",
                PasswordHash = "password",
                SalString = "salt"
            }
        };

        var expectedAdmins = _mapper.Map<List<UsuarioAdminDto>>(admins);

        usuarioAdminRepository.Setup(x => x.GetAllUsuarioAdmin()).ReturnsAsync(admins);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);

        List<UsuarioAdminDto> adminsResults = await _usuarioAdminService.ObterListaDeUsuariosAdmin();

        for (int i = 0; i < admins.Count; ++i)
            Assert.True(adminsResults[i].Match(admins[i]));
    }

    [Fact]
    public async Task Test_Delete_Must_Work()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();
        const string adminEmail = "keanureeves@outlook.com";

        UsuarioAdmin admin = new UsuarioAdmin
        {
            Id = Guid.NewGuid(),
            Email = adminEmail,
            PasswordHash = "sómatrix12sãobons",
            SalString = "salt"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync(admin);
        usuarioAdminRepository.Setup(x => x.DeleteUsuarioAdmin(It.IsAny<UsuarioAdmin>())).Verifiable();
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        await _usuarioAdminService.DeletarUsuarioAdmin(adminEmail);

        _repositoryManager.VerifyAll();
        usuarioAdminRepository.VerifyAll();
    }

    [Fact]
    public async Task Test_Delete_Shouldnt_Work_Email_Not_Exists()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();
        const string adminEmail = "keanureeves@outlook.com";

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioAdmin?)null);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);

        try
        {
            await _usuarioAdminService.DeletarUsuarioAdmin(adminEmail);
            Assert.Fail();
        }
        catch (NotFoundException ex)
        {
            Assert.Equal("email não encontrado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_Add_Must_Work()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();
        UsuarioAdminForCreateDto usuarioAdminForCreateDto = new UsuarioAdminForCreateDto
        {
            Email = "robertdowneyjr@gmail.com",
            Password = "eu sou o homem de ferro"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioAdmin?)null);
        usuarioAdminRepository.Setup(x => x.AddUsuarioAdmin(It.IsAny<UsuarioAdmin>())).Verifiable();
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        Guid id = await _usuarioAdminService.CadastraUsuarioAdmin(usuarioAdminForCreateDto);

        _repositoryManager.VerifyAll();
        usuarioAdminRepository.VerifyAll();
    }

    [Fact]
    public async Task Test_Add_Shouldnt_Work_Email_Already_Exist()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        UsuarioAdminForCreateDto usuarioAdminForCreateDto = new UsuarioAdminForCreateDto
        {
            Email = "robertdowneyjr@gmail.com",
            Password = "eu sou o homem de ferro"
        };

        UsuarioAdmin usuarioAdmin = new UsuarioAdmin
        {
            Email = "robertdowneyjr@gmail.com",
            PasswordHash = "eu sou o homem de ferro",
            SalString = "prova que o Tony Stark tem um coração"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuarioAdmin);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);

        try
        {
            await _usuarioAdminService.CadastraUsuarioAdmin(usuarioAdminForCreateDto);
            Assert.Fail();
        }
        catch(BadRequestException ex)
        {
            Assert.Equal("o email já está sendo usado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_Login_Must_Work()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        UsuarioAdmin usuarioAdmin = new UsuarioAdmin
        {
            Id = Guid.NewGuid(),
            Email = "chrishemsworth@gmail.com",
            PasswordHash = "Give me Thanos",
            SalString = "Devia ter acertado a cabeça"
        };

        LoginUsuarioDto loginDto = new LoginUsuarioDto
        {
            Email = "chrishemsworth@gmail.com",
            Password = "Give me Thanos"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuarioAdmin);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokensService.Setup(x => x.JwtToken(It.IsAny<List<Claim>>())).Returns("jwt");
        
        string token = await _usuarioAdminService.Login(loginDto);

        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public async Task Test_Login_Shouldnt_Work_Email_Dont_Exists()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        LoginUsuarioDto loginDto = new LoginUsuarioDto
        {
            Email = "chrishemsworth@gmail.com",
            Password = "Give me Thanos"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioAdmin?)null);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);

        try
        {
            string token = await _usuarioAdminService.Login(loginDto);
            Assert.Fail();
        }
        catch(UnauthorizedException ex)
        {
            Assert.Equal("email e/ou senha incorretos", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_Login_Shouldnt_Work_Password_Is_Incorrect()
    {
        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        UsuarioAdmin usuarioAdmin = new UsuarioAdmin
        {
            Id = Guid.NewGuid(),
            Email = "chrishemsworth@gmail.com",
            PasswordHash = "Give me Thanos",
            SalString = "Devia ter acertado a cabeça"
        };

        LoginUsuarioDto loginDto = new LoginUsuarioDto
        {
            Email = "chrishemsworth@gmail.com",
            Password = "Give me Thanos"
        };

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuarioAdmin);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        try
        {
            string token = await _usuarioAdminService.Login(loginDto);
            Assert.Fail();
        }
        catch (UnauthorizedException ex)
        {
            Assert.Equal("email e/ou senha incorretos", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_ChangePassword_Must_Work()
    {
        ChangeUsuarioPasswordDto changePasswordDto = new ChangeUsuarioPasswordDto
        {
            NewPassword = "Itachi não é mais poderoso que Madara",
            OldPassword = "Itachi é mais forte que Madara"
        };

        Guid usuarioId = Guid.NewGuid();

        UsuarioAdmin usuarioAdmin = new UsuarioAdmin
        {
            Email = "guy@yahoo.com",
            PasswordHash = "Oitavo portão",
            SalString = "4 guerra ninja",
            Id = usuarioId
        };
        string saltString = "";

        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByIdAsync(It.IsAny<Guid>())).ReturnsAsync(usuarioAdmin);
        usuarioAdminRepository.Setup(x => x.UpdateUsuarioAdmin(It.IsAny<UsuarioAdmin>())).Verifiable();
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _passwordHash.Setup(x => x.Encrypt(It.IsAny<string>(), out saltString)).Returns("se não fosse o juubi, Madara teria perdido");
        _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

        await _usuarioAdminService.AlterarUsuarioAdminSenha(usuarioId, changePasswordDto);

        usuarioAdminRepository.VerifyAll();
        _repositoryManager.VerifyAll();
    }

    [Fact]
    public async Task Test_ChangePassword_Shouldnt_Work_User_Not_Exists()
    {
        ChangeUsuarioPasswordDto changePasswordDto = new ChangeUsuarioPasswordDto
        {
            NewPassword = "Itachi não é mais poderoso que Madara",
            OldPassword = "Itachi é mais forte que Madara"
        };

        Guid usuarioId = Guid.NewGuid();

        UsuarioAdmin usuarioAdmin = new UsuarioAdmin
        {
            Email = "guy@yahoo.com",
            PasswordHash = "Oitavo portão",
            SalString = "4 guerra ninja",
            Id = usuarioId
        };

        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByIdAsync(It.IsAny<Guid>())).ReturnsAsync(usuarioAdmin);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);
        _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        try
        {
            await _usuarioAdminService.AlterarUsuarioAdminSenha(usuarioId, changePasswordDto);
            Assert.Fail();
        }
        catch(UnauthorizedException ex)
        {
            Assert.Equal("senha incorreta", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }

    [Fact]
    public async Task Test_ChangePassword_Shouldnt_Work_Password_Is_Incorrect()
    {
        ChangeUsuarioPasswordDto changePasswordDto = new ChangeUsuarioPasswordDto
        {
            NewPassword = "Itachi não é mais poderoso que Madara",
            OldPassword = "Itachi é mais forte que Madara"
        };

        Guid usuarioId = Guid.NewGuid();

        Mock<IUsuarioAdminRepository> usuarioAdminRepository = new Mock<IUsuarioAdminRepository>();

        usuarioAdminRepository.Setup(x => x.GetUsuarioAdminByIdAsync(It.IsAny<Guid>())).ReturnsAsync((UsuarioAdmin?)null);
        _repositoryManager.SetupGet(x => x.UsuarioAdminRepository).Returns(usuarioAdminRepository.Object);

        try
        {
            await _usuarioAdminService.AlterarUsuarioAdminSenha(usuarioId, changePasswordDto);
            Assert.Fail();
        }
        catch (NotFoundException ex)
        {
            Assert.Equal("usuário não encontrado", ex.Message);
        }
        catch
        {
            Assert.Fail();
        }
    }
}
