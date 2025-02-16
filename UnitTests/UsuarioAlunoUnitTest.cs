using AutoMapper;
using Domain.Repositories;
using Moq;
using Services.Contracts;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Mappers;
using Microsoft.Extensions.Logging;
using Shared.Dtos;
using Microsoft.AspNetCore.Server.HttpSys;
using Domain.Entities.Users;
using System.Security.Claims;
using Domain.Exceptions;
using Domain.Entities.Abstract;
using Shared.Pagination;

namespace UnitTests
{
    public class UsuarioAlunoUnitTest
    {
        public readonly IMapper _mapper;
        public readonly Mock<IPasswordHash> _passwordHash;
        public readonly Mock<ITokensService> _tokensService;
        public readonly Mock<IRepositoryManager> _repositoryManager;
        public readonly UsuarioAlunoService _usuarioAlunoService;
        public readonly Mock<IUsuarioAlunoRepository> _usuarioAlunoRepository;

        public UsuarioAlunoUnitTest()
        {
            _passwordHash = new Mock<IPasswordHash>();
            _tokensService = new Mock<ITokensService>();
            _repositoryManager = new Mock<IRepositoryManager>();
            _usuarioAlunoRepository = new Mock<IUsuarioAlunoRepository>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioAlunoProfile>());
            _mapper = config.CreateMapper();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<UsuarioAlunoService>();

            _usuarioAlunoService = new UsuarioAlunoService(logger: logger, mapper: _mapper, passwordHash: _passwordHash.Object, tokensService: _tokensService.Object, repositoryManager: _repositoryManager.Object);
        }

        [Fact]
        public async Task Test_UsuarioAluno_Login_Must_Work()
        {
            LoginUsuarioDto loginDto = new LoginUsuarioDto
            {
                Email = "yujiitadori@gmail.com",
                Password = "eu sou o jujutsu kaisen"
            };

            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadori@gmail.com",
                PasswordHash = "ripoe2298920@lkr$%c<",
                SalString = "ppepeo#$$!0-4_$",
                Id = Guid.NewGuid()
            };

            _usuarioAlunoRepository.Setup(y => y.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _repositoryManager.SetupGet(y => y.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);
            _tokensService.Setup(y => y.JwtToken(It.IsAny<List<Claim>>())).Returns("expansão de domínio");

            string token = await _usuarioAlunoService.Login(loginDto);

            Assert.Equal("expansão de domínio", token);
        }


        [Fact]
        public async Task Test_UsuarioAluno_Login_Shoundnt_Work_Email_Not_Found()
        {
            LoginUsuarioDto loginDto = new LoginUsuarioDto
            {
                Email = "yujiitadori@gmail.com",
                Password = "eu sou o jujutsu kaisen"
            };

       
            _usuarioAlunoRepository.Setup(y => y.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync((UsuarioAluno?)null);
            _repositoryManager.SetupGet(y => y.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {

                _ = await _usuarioAlunoService.Login(loginDto);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal("email não encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_UsuarioAluno_Login_Shoundnt_Work_Password_Is_Incorrect()
        {
            LoginUsuarioDto loginDto = new LoginUsuarioDto
            {
                Email = "yujiitadori@gmail.com",
                Password = "eu sou o jujutsu kaisen"
            };

            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadori@gmail.com",
                PasswordHash = "ripoe2298920@lkr$%c<",
                SalString = "ppepeo#$$!0-4_$",
                Id = Guid.NewGuid()
            };

            _usuarioAlunoRepository.Setup(y => y.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _repositoryManager.SetupGet(y => y.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {
                _ = await _usuarioAlunoService.Login(loginDto);
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
        public async Task Test_UsuarioAluno_Add_Must_Work()
        {
            UsuarioAlunoForCreateDto usuarioDto = new UsuarioAlunoForCreateDto
            {
                Email = "yujiitadoir@gmail.com",
                Password = "jujutsu kaisen",
                AlunoMatricula = Guid.NewGuid()
            };

            _usuarioAlunoRepository.Setup(x => x.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync((UsuarioAluno?)null);
            _usuarioAlunoRepository.Setup(x => x.AddUsuarioAluno(It.IsAny<UsuarioAluno>())).Verifiable();
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            string saltString;

            _passwordHash.Setup(x => x.Encrypt(It.IsAny<string>(), out saltString)).Returns("3lkkdkdkdkje9404040");

            await _usuarioAlunoService.CadastraUsuarioAluno(usuarioDto);

            _usuarioAlunoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_UsuarioAluno_Add_Shouldnt_Work_Email_Alreay_Exists()
        {
            UsuarioAlunoForCreateDto usuarioDto = new UsuarioAlunoForCreateDto
            {
                Email = "yujiitadoir@gmail.com",
                Password = "jujutsu kaisen",
                AlunoMatricula = Guid.NewGuid()
            };

            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                PasswordHash = "kk#$%pppfpf<1R$Qle",
                SalString = "/E.Eo4o4o%09-ofkr<"
            };

            _usuarioAlunoRepository.Setup(x => x.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {
                await _usuarioAlunoService.CadastraUsuarioAluno(usuarioDto);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal("usuário com email já existente", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_UsuarioAluno_Update_Must_Work()
        {
            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "expansão de domínio",
                OldPassword = "sukuna me ajuda"
            };

            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                PasswordHash = "kk#$%pppfpf<1R$Qle",
                SalString = "/E.Eo4o4o%09-ofkr<",
                Id = Guid.NewGuid()
            };

            string saltString;

            _usuarioAlunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _usuarioAlunoRepository.Setup(x => x.UpdateUsuarioAluno(It.IsAny<UsuarioAluno>())).Verifiable();
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);
            _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _passwordHash.Setup(x => x.Encrypt(It.IsAny<string>(), out saltString)).Returns("Ek3k3kkoo]$$92$");
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            await _usuarioAlunoService.AlteraSenhaUsuarioAluno(usuario.Id, changePassword);

            _usuarioAlunoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_UsuarioAluno_Update_Shouldnt_Work_Email_Not_Found()
        {
            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "expansão de domínio",
                OldPassword = "sukuna me ajuda"
            };

            Guid usuarioId = Guid.NewGuid();

            _usuarioAlunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync((UsuarioAluno?)null);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {
                await _usuarioAlunoService.AlteraSenhaUsuarioAluno(usuarioId, changePassword);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal("usuário não encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_UsuarioAluno_Update_Shouldnt_Work_Password_Is_Incorrect()
        {
            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "expansão de domínio",
                OldPassword = "sukuna me ajuda"
            };

            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                PasswordHash = "kk#$%pppfpf<1R$Qle",
                SalString = "/E.Eo4o4o%09-ofkr<",
                Id = Guid.NewGuid()
            };

            string saltString;

            _usuarioAlunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);
            _passwordHash.Setup(x => x.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            try
            {
                await _usuarioAlunoService.AlteraSenhaUsuarioAluno(usuario.Id, changePassword);
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
        public async Task Test_UsuarioAluno_GetById_Must_Work()
        {
            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                PasswordHash = "kk#$%pppfpf<1R$Qle",
                SalString = "/E.Eo4o4o%09-ofkr<",
                Id = Guid.NewGuid()
            };

            _usuarioAlunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            UsuarioAlunoDto usuarioDto = await _usuarioAlunoService.ObterUsuarioAluno(usuario.Id);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_UsuarioAluno_GetById_Shouldnt_Work_Id_Not_Found()
        {
            Guid usuarioId = Guid.NewGuid();

            _usuarioAlunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync((UsuarioAluno?)null);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {
                _ = await _usuarioAlunoService.ObterUsuarioAluno(usuarioId);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal("usuário não encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_UsuarioAluno_GetByEmail_Must_Work()
        {
            UsuarioAluno usuario = new UsuarioAluno
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                PasswordHash = "kk#$%pppfpf<1R$Qle",
                SalString = "/E.Eo4o4o%09-ofkr<",
                Id = Guid.NewGuid()
            };

            _usuarioAlunoRepository.Setup(x => x.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            UsuarioAlunoDto usuarioDto = await _usuarioAlunoService.ObterUsuarioAlunoPorEmail(usuario.Email);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_UsuarioAluno_GetByEmail_Shouldnt_Work_Email_Not_Found()
        {
            string email = "yujiitadoir@gmail.com";

            _usuarioAlunoRepository.Setup(x => x.GetAlunoByEmailAsync(It.IsAny<string>(), It.IsAny<GetUsuarioAlunoOptions>())).ReturnsAsync((UsuarioAluno?)null);
            _repositoryManager.SetupGet(x => x.UsuarioAlunoRepository).Returns(_usuarioAlunoRepository.Object);

            try
            {
                _ = await _usuarioAlunoService.ObterUsuarioAlunoPorEmail(email);
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

        [Fact]
        public void Test_If_UsuarioAlunoCreateDto_Mapper_Is_Working()
        {
            UsuarioAlunoForCreateDto usuarioAlunoForCreate = new UsuarioAlunoForCreateDto
            {
                AlunoMatricula = Guid.NewGuid(),
                Email = "yujiitadoir@gmail.com",
                Password = "Seis olhos"
            };

            UsuarioAluno usuario = _mapper.Map<UsuarioAluno>(usuarioAlunoForCreate);

            Assert.Equal(usuarioAlunoForCreate.Email, usuario.Email);
            Assert.Equal(usuarioAlunoForCreate.AlunoMatricula, usuario.AlunoMatricula);
            Assert.Equal("", usuario.PasswordHash);
            Assert.Equal("", usuario.SalString);
        }
    }
}
