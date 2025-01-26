using AutoMapper;
using Domain.Repositories;
using Moq;
using Services;
using Services.Contracts;
using Services.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.Dtos;
using Domain.Entities.Users;
using Persistence.Repositories;
using Domain.Exceptions;
using Domain.Entities.Abstract;
using System.Reflection.Metadata;

namespace UnitTests
{
    public class UsuarioAdministrativoUnitTest
    {
        public readonly IMapper _mapper;
        public readonly Mock<IPasswordHash> _passwordHash;
        public readonly Mock<ITokensService> _tokensService;
        public readonly Mock<IRepositoryManager> _repositoryManager;
        public readonly UsuarioAdministrativoService _usuarioAdministrativoService;
        public readonly Mock<IUsuarioAdministrativoRepository> _usuarioAdministrativoRepository;

        public UsuarioAdministrativoUnitTest()
        {
            _passwordHash = new Mock<IPasswordHash>();
            _tokensService = new Mock<ITokensService>();
            _repositoryManager = new Mock<IRepositoryManager>();
            _usuarioAdministrativoRepository = new Mock<IUsuarioAdministrativoRepository>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioAdministrativoProfile>());
            _mapper = config.CreateMapper();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<UsuarioAdministrativoService>();

            _usuarioAdministrativoService = new UsuarioAdministrativoService(logger: logger, mapper: _mapper, passwordHash: _passwordHash.Object, repositoryManager: _repositoryManager.Object, tokensService: _tokensService.Object);
        }

        [Fact]
        public async Task Test_Add_UsuarioAdministrativo_Must_Work()
        {
            UsuarioAdministrativoForCreateDto usuarioAdministrativoForCreate = new UsuarioAdministrativoForCreateDto
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };
            string saltString;

            _usuarioAdministrativoRepository.Setup(y => y.GetAdminstrativoByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioAdministrativo?)null);
            _usuarioAdministrativoRepository.Setup(y => y.AddUsuarioAdministrativo(It.IsAny<UsuarioAdministrativo>())).Verifiable();
            _passwordHash.Setup(x => x.Encrypt(It.IsAny<string>(), out saltString)).Returns("Sukuna");
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);
            _repositoryManager.Setup(y => y.SaveAsync()).Verifiable();

            await _usuarioAdministrativoService.CadastraUsuarioAdministrativo(usuarioAdministrativoForCreate);

            _usuarioAdministrativoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_Add_UsuarioAdministrativo_shouldnt_Work_Email_Already_Exist()
        {
            UsuarioAdministrativo usuario = new UsuarioAdministrativo
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            UsuarioAdministrativoForCreateDto usuarioAdministrativoForCreate = new UsuarioAdministrativoForCreateDto
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };

            _usuarioAdministrativoRepository.Setup(y => y.GetAdminstrativoByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            try
            {
                await _usuarioAdministrativoService.CadastraUsuarioAdministrativo(usuarioAdministrativoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal("email já existente", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Update_UsuarioAdministrativo_Must_workt()
        {
            UsuarioAdministrativo usuario = new UsuarioAdministrativo
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "raio roxo",
                OldPassword = "Seis Olhos"
            };

            Guid id = Guid.NewGuid();
            
            _usuarioAdministrativoRepository.Setup(y => y.GetAdministrativoAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _usuarioAdministrativoRepository.Setup(y => y.UpdateUsuarioAdministrativo(It.IsAny<UsuarioAdministrativo>())).Verifiable();
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _repositoryManager.Setup(y => y.SaveAsync()).Verifiable();

            await _usuarioAdministrativoService.AlteraSenhaUsuarioAdministrativo(id, changePassword);

            _usuarioAdministrativoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_Update_UsuarioAdministrativo_Should_workt_Id_Not_Exist()
        {
            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "raio roxo",
                OldPassword = "Seis Olhos"
            };

            Guid id = Guid.NewGuid();

            _usuarioAdministrativoRepository.Setup(y => y.GetAdministrativoAsync(It.IsAny<Guid>())).ReturnsAsync((UsuarioAdministrativo?)null);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            try
            {
                await _usuarioAdministrativoService.AlteraSenhaUsuarioAdministrativo(id, changePassword);
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
        public async Task Test_Update_UsuarioAdministrativo_Should_workt_Password_Is_Incorrect()
        {
            UsuarioAdministrativo usuario = new UsuarioAdministrativo
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "raio roxo",
                OldPassword = "Seis Olhos"
            };

            Guid id = Guid.NewGuid();

            _usuarioAdministrativoRepository.Setup(y => y.GetAdministrativoAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            try
            {
                await _usuarioAdministrativoService.AlteraSenhaUsuarioAdministrativo(id, changePassword);
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
        public async Task Test_GetById_UsuarioAdministrativo_Must_Work()
        {
            UsuarioAdministrativo usuario = new UsuarioAdministrativo
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            Guid id = Guid.NewGuid();

            _usuarioAdministrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativo(id);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_GetById_UsuarioAdministrativo_Shouldnt_Work_Id_Not_Exist()
        {
            Guid id = Guid.NewGuid();
            _usuarioAdministrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>())).ReturnsAsync((UsuarioAdministrativo?)null);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            try
            {
                _ = await _usuarioAdministrativoService.ObterUsuarioAdministrativo(id);
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
        public async Task Test_GetByEmail_UsuarioAdministrativo_Must_Work()
        {
            UsuarioAdministrativo usuario = new UsuarioAdministrativo
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            string email = "gojo@gmail.com";

            _usuarioAdministrativoRepository.Setup(x => x.GetAdminstrativoByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativoPorEmail(email);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_GetByEmail_UsuarioAdministrativo_Shouldnt_Work_Email_not_Exist()
        {
            string email = "gojo@gmail.com";
            _usuarioAdministrativoRepository.Setup(x => x.GetAdminstrativoByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioAdministrativo?)null);
            _repositoryManager.SetupGet(y => y.UsuarioAdministrativoRepository).Returns(_usuarioAdministrativoRepository.Object);

            try
            {
                _ = await _usuarioAdministrativoService.ObterUsuarioAdministrativoPorEmail(email);
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
        public async Task Test_If_AdministrativoForCreateDto_Mapper_Is_Working()
        {
            UsuarioAdministrativoForCreateDto usuarioAdministrativoForCreate = new UsuarioAdministrativoForCreateDto
            {
                AdministrativoMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };

            UsuarioAdministrativo usuario = _mapper.Map<UsuarioAdministrativo>(usuarioAdministrativoForCreate);

            Assert.Equal(usuarioAdministrativoForCreate.Email, usuario.Email);
            Assert.Equal(usuarioAdministrativoForCreate.AdministrativoMatricula, usuario.AdministrativoMatricula);
            Assert.Equal("", usuario.PasswordHash);
            Assert.Equal("", usuario.SalString);
        }
    }
}
