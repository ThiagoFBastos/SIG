using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Moq;
using Services;
using Services.Contracts;
using API.Controllers;
using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions;
using Services.Mappers;
using Microsoft.Extensions;
using Microsoft.Extensions.Logging;
using Shared.Dtos;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json.Serialization;
using System.Text.Json;
using Domain.Entities.Users;

namespace UnitTests
{
    public class AdministrativoUnitTest
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly IAdministrativoService _administrativoService;
        private readonly Mock<IEnderecoService> _enderecoService;
        private readonly ITestOutputHelper _output;
        private readonly Mock<IUsuarioAdministrativoService> _usuarioAdministrativoService;
        public AdministrativoUnitTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AdministrativoProfile>());
            _mapper = config.CreateMapper();
            
            _repositoryManager = new Mock<IRepositoryManager>();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<AdministrativoService>();

            _enderecoService = new Mock<IEnderecoService>();
            _usuarioAdministrativoService = new Mock<IUsuarioAdministrativoService>();

            _administrativoService = new AdministrativoService(_repositoryManager.Object, logger, _mapper, _enderecoService.Object, _usuarioAdministrativoService.Object);
        }
 
        [Fact]
        public async Task Test_Add_Administrativo_Must_Work()
        {
            EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                Casa = 111,
                Complemento = "Fundos casa 2",
                CEP = "20050006"
            };

            AdministrativoForCreateDto administrativoForCreate = new AdministrativoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = enderecoForCreate,
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.AddAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);
            _enderecoService.Setup(x => x.CadastrarEndereco(It.IsAny<EnderecoForCreateDto>())).ReturnsAsync(Guid.NewGuid());
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();
            _usuarioAdministrativoService.Setup(x => x.CadastraUsuarioAdministrativo(It.IsAny<UsuarioAdministrativoForCreateDto>())).Verifiable();

            Guid matricula = await _administrativoService.CadastrarAdmnistrativo(administrativoForCreate);

            _repositoryManager.VerifyAll();
            administrativoRepository.VerifyAll();
            _usuarioAdministrativoService.VerifyAll();
        }
        
        [Fact]
        public async Task Test_Add_Administrativo_Must_Throw_BadRequestException_Due_CPF()
        {
            EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                Casa = 111,
                Complemento = "Fundos casa 2",
                CEP = "20050006"
            };

            AdministrativoForCreateDto administrativoForCreate = new AdministrativoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = enderecoForCreate,
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M
            };

            Administrativo administrativo = _mapper.Map<Administrativo>(administrativoForCreate);
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            try
            {
                _ = await _administrativoService.CadastrarAdmnistrativo(administrativoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um administrativo com o cpf: {administrativo.CPF}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        } 
        
        [Fact]
        public async Task Test_Add_Administrativo_Must_Throw_BadRequestException_Due_RG()
        {
            EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                Casa = 111,
                Complemento = "Fundos casa 2",
                CEP = "20050006"
            };

            AdministrativoForCreateDto administrativoForCreate = new AdministrativoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = enderecoForCreate,
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M
            };

            Administrativo administrativo = _mapper.Map<Administrativo>(administrativoForCreate);
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            try
            {
                _ = await _administrativoService.CadastrarAdmnistrativo(administrativoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um administrativo com o rg: {administrativo.RG}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Add_Administrativo_Must_Throw_BadRequestException_Due_Email()
        {
            EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                Casa = 111,
                Complemento = "Fundos casa 2",
                CEP = "20050006"
            };

            AdministrativoForCreateDto administrativoForCreate = new AdministrativoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = enderecoForCreate,
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M
            };

            Administrativo administrativo = _mapper.Map<Administrativo>(administrativoForCreate);
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            try
            {
                _ = await _administrativoService.CadastrarAdmnistrativo(administrativoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um administrativo com o email: {administrativo.Email}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Add_Administrativo_Must_Throw_BadRequestException_Due_Celular()
        {
            EnderecoForCreateDto enderecoForCreate = new EnderecoForCreateDto
            {
                Cidade = "Rio de Janeiro",
                Estado = (int)Estado.RJ,
                Rua = "Rua Sete de Setembro",
                Casa = 111,
                Complemento = "Fundos casa 2",
                CEP = "20050006"
            };

            AdministrativoForCreateDto administrativoForCreate = new AdministrativoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = enderecoForCreate,
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M
            };

            Administrativo administrativo = _mapper.Map<Administrativo>(administrativoForCreate);
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            administrativoRepository.Setup(x => x.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            try
            {
                _ = await _administrativoService.CadastrarAdmnistrativo(administrativoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um administrativo com o celular: {administrativo.Celular}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Update_Administrativo_Must_Work()
        {
            AdministrativoForUpdateDto administrativoForUpdate = new AdministrativoForUpdateDto
            {
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                Email = "shizuo@bol.com",
                Celular = "21912345678"
            };

            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M
            };
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
            administrativoRepository.Setup(x => x.UpdateAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            AdministrativoDto admistrativoDto = await _administrativoService.AlterarAdministrativo(Guid.NewGuid(), administrativoForUpdate);

            administrativoRepository.VerifyAll();
            _repositoryManager.VerifyAll();

            Assert.True(admistrativoDto.Match(administrativo));
        }
        
        [Fact]
        public async Task Test_Update_Administrativo_Must_Throw_NotFoundException()
        {
            AdministrativoForUpdateDto administrativoForUpdate = new AdministrativoForUpdateDto
            {
                Cargo = "Professor de educação física",
                Salario = 9999.99M,
                Banco = "Banco do Brasil",
                ContaCorrente = "XXXXXXX-XX",
                Status = (int)StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                Email = "shizuo@bol.com",
                Celular = "21912345678"
            };

            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

            Mock<IAdministrativoRepository> administrativRepository = new Mock<IAdministrativoRepository>();

            administrativRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativRepository.Object);

            Guid matricula = Guid.NewGuid();

            try
            {
                _ = await _administrativoService.AlterarAdministrativo(matricula, administrativoForUpdate);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O administrativo com matrícula: {matricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Delete_Administrativo_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
            administrativoRepository.Setup(x => x.DeleteAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            await _administrativoService.DeletarAdministrativo(administrativo.Matricula);

            _repositoryManager.VerifyAll();
            administrativoRepository.VerifyAll();
        }
        
        [Fact]
        public async Task Test_Delete_Administrativo_Must_Throw_NotFoundException()
        {
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();


            administrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            Guid matricula = Guid.NewGuid();

            try
            {
                await _administrativoService.DeletarAdministrativo(matricula);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O administrativo com matrícula: {matricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Get_Administrativo_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

           Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

           administrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           AdministrativoDto administrativoDto = await _administrativoService.ObterAdministrativoPorMatricula(administrativo.Matricula);

           Assert.True(administrativoDto.Match(administrativo));
        }   
        
        [Fact]
        public async Task Test_Get_Administrativo_Must_Throw_Exception()
        {
            Guid matricula = Guid.NewGuid();
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           try
           {
                _ = await _administrativoService.ObterAdministrativoPorMatricula(matricula);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"O administrativo com matrícula: {matricula} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Get_Administrativo_By_CPF_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           AdministrativoDto administrativoDto = await _administrativoService.ObterAdministrativoPorCPF(administrativo.CPF);

           Assert.True(administrativoDto.Match(administrativo));
        }   
        
        [Fact]
        public async Task Test_Get_Administrativo_By_CPF_MustThrow_Exception()
        {
            const string cpf = "15158114099";
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           try
           {
                _ = await _administrativoService.ObterAdministrativoPorCPF(cpf);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"O administrativo com cpf: {cpf} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Get_Administrativo_By_RG_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           AdministrativoDto administrativoDto = await _administrativoService.ObterAdministrativoPorRG(administrativo.RG);

           Assert.True(administrativoDto.Match(administrativo));
        }   
        
        [Fact]
        public async Task Test_Get_Administrativo_By_RG_MustThrow_Exception()
        {
            const string rg = "339404954";
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           try
           {
                _ = await _administrativoService.ObterAdministrativoPorRG(rg);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"O administrativo com rg: {rg} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Get_Administrativo_By_Email_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           AdministrativoDto administrativoDto = await _administrativoService.ObterAdministrativoPeloEmail(administrativo.Email);

           Assert.True(administrativoDto.Match(administrativo));
        }   
        
        [Fact]
        public async Task Test_Get_Administrativo_By_Email_MustThrow_Exception()
        {
            const string email = "shizuoheiwajima@bol.com";
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();


            administrativoRepository.Setup(x => x.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

           try
           {
                _ = await _administrativoService.ObterAdministrativoPeloEmail(email);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"O administrativo com email: {email} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Get_Administrativo_By_Celular_Must_Work()
        {
            Administrativo administrativo = new Administrativo
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                Cargo = "Professor de educação física",
                Salario = 5000.99M,
                Banco = "Itau",
                ContaCorrente = "YYYYYYYY-YY",
                Status = StatusEmprego.ATIVO,
                HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                EnderecoId = Guid.NewGuid(),
                Endereco = null,
                Email = "shizuoheiwajima@bol.com",
                Celular = "21987654321",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                Matricula = Guid.NewGuid()
            };
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            AdministrativoDto administrativoDto = await _administrativoService.ObterAdministrativoPeloCelular(administrativo.Celular);

           Assert.True(administrativoDto.Match(administrativo));
        }   
        
        [Fact]
        public async Task Test_Get_Administrativo_By_Celular_MustThrow_Exception()
        {
            const string celular = "21987654321";
            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            try
            {
                _ = await _administrativoService.ObterAdministrativoPeloCelular(celular);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"O administrativo com celular: {celular} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Filter_Administrativos_Must_Work()
        {
            var administrativos = new List<Administrativo>
            {
                new Administrativo
                {
                    NomeCompleto = "Shizuo Heiwajima",
                    CPF = "15158114099",
                    RG = "339404954",
                    Cargo = "Professor de educação física",
                    Salario = 5000.99M,
                    Banco = "Itau",
                    ContaCorrente = "YYYYYYYY-YY",
                    Status = StatusEmprego.ATIVO,
                    HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                    HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                    EnderecoId = Guid.NewGuid(),
                    Endereco = null,
                    Email = "shizuoheiwajima@bol.com",
                    Celular = "21987654321",
                    DataNascimento = new DateTime(1980, 12, 31),
                    Sexo = Sexo.M,
                    Matricula = Guid.NewGuid()
                }, new Administrativo
                {
                    NomeCompleto = "Shizuo Heiwajima",
                    CPF = "15158114099",
                    RG = "339404954",
                    Cargo = "Professor de educação física",
                    Salario = 5000.99M,
                    Banco = "Itau",
                    ContaCorrente = "YYYYYYYY-YY",
                    Status = StatusEmprego.ATIVO,
                    HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                    HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                    EnderecoId = Guid.NewGuid(),
                    Endereco = null,
                    Email = "shizuoheiwajima@bol.com",
                    Celular = "21987654321",
                    DataNascimento = new DateTime(1980, 12, 31),
                    Sexo = Sexo.M,
                    Matricula = Guid.NewGuid()
                },
                new Administrativo
                {
                    NomeCompleto = "Shizuo Heiwajima",
                    CPF = "15158114099",
                    RG = "339404954",
                    Cargo = "Professor de educação física",
                    Salario = 5000.99M,
                    Banco = "Itau",
                    ContaCorrente = "YYYYYYYY-YY",
                    Status = StatusEmprego.ATIVO,
                    HorarioInicioExpediente = new DateTime(1, 1, 1, 8, 0, 0, 0),
                    HorarioFimExpediente = new DateTime(1, 1, 1, 18, 0, 0, 0),
                    EnderecoId = Guid.NewGuid(),
                    Endereco = null,
                    Email = "shizuoheiwajima@bol.com",
                    Celular = "21987654321",
                    DataNascimento = new DateTime(1980, 12, 31),
                    Sexo = Sexo.M,
                    Matricula = Guid.NewGuid()
                }
            };

            Mock<IAdministrativoRepository> administrativoRepository = new Mock<IAdministrativoRepository>();

            administrativoRepository.Setup(x => x.GetAdministrativosAsync(It.IsAny<GetAdministrativosOptions>())).ReturnsAsync(administrativos);
            _repositoryManager.SetupGet(x => x.AdministrativoRepository).Returns(administrativoRepository.Object);

            Pagination<AdministrativoDto> administrativoDto = await _administrativoService.ObterAdministrativos(new GetAdministrativosOptions());

            Assert.Equal(administrativos.Count, administrativoDto.Items.Count);

            for (int i = 0; i < administrativos.Count; ++i)
                Assert.True(administrativoDto.Items[i].Match(administrativos[i]));
        }
    }
} 