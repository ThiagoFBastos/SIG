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

namespace UnitTests
{
    public class AdministrativoUnitTest
    {
        private readonly IMapper _mapper;
        private readonly AdministrativoController _admnistrativoController;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly IAdministrativoService _administrativoService;
        private readonly Mock<IEnderecoService> _enderecoService;
        private readonly ITestOutputHelper _output;
        public AdministrativoUnitTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AdministrativoProfile>());
            _mapper = config.CreateMapper();
            
            _repositoryManager = new Mock<IRepositoryManager>();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<AdministrativoService>();

            _enderecoService = new Mock<IEnderecoService>();

            _administrativoService = new AdministrativoService(_repositoryManager.Object, logger, _mapper, _enderecoService.Object);

            _admnistrativoController = new AdministrativoController(_administrativoService);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _enderecoService.Setup(x => x.CadastrarEndereco(It.IsAny<EnderecoForCreateDto>())).ReturnsAsync(Guid.NewGuid());
            _repositoryManager.Setup(x => x.AdministrativoRepository.AddAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _admnistrativoController.Add(administrativoForCreate);

            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            Assert.NotNull(okResponse.Value);
            Assert.True(okResponse.Value is Guid);
            Assert.Equal(okResponse.StatusCode, 200);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);

            try
            {
                var response = await _admnistrativoController.Add(administrativoForCreate);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);

            try
            {
                var response = await _admnistrativoController.Add(administrativoForCreate);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);

            try
            {
                var response = await _admnistrativoController.Add(administrativoForCreate);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);

            try
            {
                var response = await _admnistrativoController.Add(administrativoForCreate);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.Setup(x => x.AdministrativoRepository.UpdateAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _admnistrativoController.Update(Guid.NewGuid(), administrativoForUpdate);

            _repositoryManager.VerifyAll();
            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            
            var result = okResponse.Value as AdministrativoDto;

            Assert.NotNull(result);
            Assert.True(result.Match(administrativo));
            Assert.Equal(okResponse.StatusCode, 200);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);

            Guid matricula = Guid.NewGuid();

            try
            {
                var response = await _admnistrativoController.Update(matricula, administrativoForUpdate);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
            _repositoryManager.Setup(x => x.AdministrativoRepository.DeleteAdministrativo(It.IsAny<Administrativo>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _admnistrativoController.Delete(administrativo.Matricula);

            _repositoryManager.VerifyAll();
            Assert.NotNull(response);
            Assert.True(response is NoContentResult);

            var noContentResponse = response as NoContentResult;

            Assert.NotNull(noContentResponse);
            Assert.Equal(204, noContentResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Delete_Administrativo_Must_Throw_NotFoundException()
        {
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);
            
            Guid matricula = Guid.NewGuid();

            try
            {
                var response = await _admnistrativoController.Delete(matricula);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync(administrativo);
           
           var response = await _admnistrativoController.Get(administrativo.Matricula);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as AdministrativoDto;

           Assert.NotNull(result);

           Assert.True(result.Match(administrativo));
           Assert.Equal(200, okResponse.StatusCode);
        }   

        [Fact]
        public async Task Test_Get_Administrativo_Must_Throw_Exception()
        {
            Guid matricula = Guid.NewGuid();
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Administrativo?)null);
           
           try
           {
                var response = await _admnistrativoController.Get(matricula);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
           
           var response = await _admnistrativoController.GetByCPF(administrativo.CPF);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as AdministrativoDto;

           Assert.NotNull(result);

           Assert.True(result.Match(administrativo));
           Assert.Equal(200, okResponse.StatusCode);
        }   

        [Fact]
        public async Task Test_Get_Administrativo_By_CPF_MustThrow_Exception()
        {
            const string cpf = "15158114099";
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
           
           try
           {
                var response = await _admnistrativoController.GetByCPF(cpf);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
           
           var response = await _admnistrativoController.GetByRG(administrativo.RG);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as AdministrativoDto;

           Assert.NotNull(result);

           Assert.True(result.Match(administrativo));
           Assert.Equal(200, okResponse.StatusCode);
        }   

        [Fact]
        public async Task Test_Get_Administrativo_By_RG_MustThrow_Exception()
        {
            const string rg = "339404954";
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
           
           try
           {
                var response = await _admnistrativoController.GetByRG(rg);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
           
           var response = await _admnistrativoController.GetByEmail(administrativo.Email);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as AdministrativoDto;

           Assert.NotNull(result);

           Assert.True(result.Match(administrativo));
           Assert.Equal(200, okResponse.StatusCode);
        }   

        [Fact]
        public async Task Test_Get_Administrativo_By_Email_MustThrow_Exception()
        {
            const string email = "shizuoheiwajima@bol.com";
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
           
           try
           {
                var response = await _admnistrativoController.GetByEmail(email);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(administrativo);
           
           var response = await _admnistrativoController.GetByCelular(administrativo.Celular);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as AdministrativoDto;

           Assert.NotNull(result);

           Assert.True(result.Match(administrativo));
           Assert.Equal(200, okResponse.StatusCode);
        }   

        [Fact]
        public async Task Test_Get_Administrativo_By_Celular_MustThrow_Exception()
        {
            const string celular = "21987654321";
            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativoPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Administrativo?)null);
           
           try
           {
                var response = await _admnistrativoController.GetByCelular(celular);
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

            _repositoryManager.Setup(x => x.AdministrativoRepository.GetAdministrativosAsync(It.IsAny<GetAdministrativosOptions>())).ReturnsAsync(administrativos);

            var response = await _admnistrativoController.Filter(new GetAdministrativosOptions());

            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            var result = okResponse.Value as Pagination<AdministrativoDto>;

            Assert.NotNull(result);

            Assert.Equal(administrativos.Count, result.Items.Count);

            for(int i = 0; i < administrativos.Count; ++i)
                Assert.True(result.Items[i].Match(administrativos[i]));

            Assert.Equal(0, result.CurrentPage);
            Assert.Equal(200, okResponse.StatusCode);
        }
    }
} 