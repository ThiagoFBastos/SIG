using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Services.Contracts;
using Services;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Mappers;

namespace UnitTests
{
    public class UsuarioProfessorUnitTest
    {
        public readonly IMapper _mapper;
        public readonly Mock<IPasswordHash> _passwordHash;
        public readonly Mock<ITokensService> _tokensService;
        public readonly Mock<IRepositoryManager> _repositoryManager;
        public readonly UsuarioProfessorService _usuarioProfessorService;
        public readonly Mock<IUsuarioProfessorRepository> _usuarioProfessorRepository;

        public UsuarioProfessorUnitTest()
        {
            _passwordHash = new Mock<IPasswordHash>();
            _tokensService = new Mock<ITokensService>();
            _repositoryManager = new Mock<IRepositoryManager>();
            _usuarioProfessorRepository = new Mock<IUsuarioProfessorRepository>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioProfessorProfile>());
            _mapper = config.CreateMapper();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<UsuarioProfessorService>();

            _usuarioProfessorService = new UsuarioProfessorService(logger: logger, mapper: _mapper, passwordHash: _passwordHash.Object, repositoryManaher: _repositoryManager.Object, tokensService: _tokensService.Object);
        }

        [Fact]
        public async Task Test_Add_UsuarioProfessor_Must_Work()
        {
            UsuarioProfessorForCreateDto usuarioProfessorForCreate = new UsuarioProfessorForCreateDto
            {
                ProfessorMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };
            string saltString;

            _usuarioProfessorRepository.Setup(y => y.GetProfessorByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioProfessor?)null);
            _usuarioProfessorRepository.Setup(y => y.AddUsuarioProfessor(It.IsAny<UsuarioProfessor>())).Verifiable();
            _passwordHash.Setup(x => x.Encrypt(It.IsAny<string>(), out saltString)).Returns("Sukuna");
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);
            _repositoryManager.Setup(y => y.SaveAsync()).Verifiable();

            await _usuarioProfessorService.CadastraUsuarioProfessor(usuarioProfessorForCreate);

            _usuarioProfessorRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_Add_UsuarioProfessor_shouldnt_Work_Email_Already_Exist()
        {
            UsuarioProfessor usuario = new UsuarioProfessor
            {
                ProfessorMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            UsuarioProfessorForCreateDto usuarioProfessorForCreate = new UsuarioProfessorForCreateDto
            {
                ProfessorMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };

            _usuarioProfessorRepository.Setup(y => y.GetProfessorByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            try
            {
                await _usuarioProfessorService.CadastraUsuarioProfessor(usuarioProfessorForCreate);
                Assert.Fail();
            }
            catch (BadRequestException ex)
            {
                Assert.Equal("email já existente", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Update_UsuarioProfessor_Must_workt()
        {
            UsuarioProfessor usuario = new UsuarioProfessor
            {
                ProfessorMatricula = Guid.NewGuid(),
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

            _usuarioProfessorRepository.Setup(y => y.GetProfessorAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _usuarioProfessorRepository.Setup(y => y.UpdateUsuarioProfessor(It.IsAny<UsuarioProfessor>())).Verifiable();
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _repositoryManager.Setup(y => y.SaveAsync()).Verifiable();

            await _usuarioProfessorService.AlteraSenhaUsuarioProfessor(id, changePassword);

            _usuarioProfessorRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }

        [Fact]
        public async Task Test_Update_UsuarioProfessor_Should_workt_Id_Not_Exist()
        {
            ChangeUsuarioPasswordDto changePassword = new ChangeUsuarioPasswordDto
            {
                NewPassword = "raio roxo",
                OldPassword = "Seis Olhos"
            };

            Guid id = Guid.NewGuid();

            _usuarioProfessorRepository.Setup(y => y.GetProfessorAsync(It.IsAny<Guid>())).ReturnsAsync((UsuarioProfessor?)null);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            try
            {
                await _usuarioProfessorService.AlteraSenhaUsuarioProfessor(id, changePassword);
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
        public async Task Test_Update_UsuarioProfessor_Should_workt_Password_Is_Incorrect()
        {
            UsuarioProfessor usuario = new UsuarioProfessor
            {
                ProfessorMatricula = Guid.NewGuid(),
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

            _usuarioProfessorRepository.Setup(y => y.GetProfessorAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);
            _passwordHash.Setup(y => y.Decrypt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            try
            {
                await _usuarioProfessorService.AlteraSenhaUsuarioProfessor(id, changePassword);
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
        public async Task Test_GetById_UsuarioProfessor_Must_Work()
        {
            UsuarioProfessor usuario = new UsuarioProfessor
            {
                ProfessorMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            Guid id = Guid.NewGuid();

            _usuarioProfessorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessor(id);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_GetById_UsuarioProfessor_Shouldnt_Work_Id_Not_Exist()
        {
            Guid id = Guid.NewGuid();
            _usuarioProfessorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>())).ReturnsAsync((UsuarioProfessor?)null);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            try
            {
                _ = await _usuarioProfessorService.ObterUsuarioProfessor(id);
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
        public async Task Test_GetByEmail_UsuarioProfessor_Must_Work()
        {
            UsuarioProfessor usuario = new UsuarioProfessor
            {
                ProfessorMatricula = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                PasswordHash = "Seis Olhos",
                SalString = "Muryōkūsho"
            };

            string email = "gojo@gmail.com";

            _usuarioProfessorRepository.Setup(x => x.GetProfessorByEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessorPorEmail(email);

            Assert.True(usuarioDto.Match(usuario));
        }

        [Fact]
        public async Task Test_GetByEmail_UsuarioProfessor_Shouldnt_Work_Email_not_Exist()
        {
            string email = "gojo@gmail.com";
            _usuarioProfessorRepository.Setup(x => x.GetProfessorByEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioProfessor?)null);
            _repositoryManager.SetupGet(y => y.UsuarioProfessorRepository).Returns(_usuarioProfessorRepository.Object);

            try
            {
                _ = await _usuarioProfessorService.ObterUsuarioProfessorPorEmail(email);
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
        public void Test_If_UsuarioProfessorCreateDto_Mapper_Is_Working()
        {
            UsuarioProfessorForCreateDto usuarioProfessorForCreate = new UsuarioProfessorForCreateDto
            {
                ProfessorMatricula = Guid.NewGuid(),
                Email = "gojo@gmail.com",
                Password = "Seis olhos"
            };

            UsuarioProfessor usuario = _mapper.Map<UsuarioProfessor>(usuarioProfessorForCreate);

            Assert.Equal(usuarioProfessorForCreate.Email, usuario.Email);
            Assert.Equal(usuarioProfessorForCreate.ProfessorMatricula, usuario.ProfessorMatricula);
            Assert.Equal("", usuario.PasswordHash);
            Assert.Equal("", usuario.SalString);
        }
    }
}
