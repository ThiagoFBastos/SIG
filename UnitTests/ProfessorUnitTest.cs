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

namespace UnitTests
{
    public class ProfessorUnitTest
    {
        private readonly IMapper _mapper;
        private readonly ProfessorController _professorController;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly IProfessorService _professorService;
        private readonly Mock<IEnderecoService> _enderecoService;
        private readonly ITestOutputHelper _output; 

        public ProfessorUnitTest(ITestOutputHelper output)
        {
            _output = output;
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProfessorProfile>());
            _mapper = config.CreateMapper();
            
            _repositoryManager = new Mock<IRepositoryManager>();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<ProfessorService>();

            _enderecoService = new Mock<IEnderecoService>();

            _professorService = new ProfessorService(_repositoryManager.Object, logger, _mapper, _enderecoService.Object);

            _professorController = new ProfessorController(_professorService);
        }

        [Fact]
        public async Task Test_Add_Professor_Must_Work()
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

            ProfessorForCreateDto professor = new ProfessorForCreateDto
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _enderecoService.Setup(x => x.CadastrarEndereco(It.IsAny<EnderecoForCreateDto>())).ReturnsAsync(Guid.NewGuid());
            _repositoryManager.Setup(x => x.ProfessorRepository.AddProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _professorController.Add(professor);

            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            Assert.NotNull(okResponse.Value);
            Assert.True(okResponse.Value is Guid);
            Assert.Equal(okResponse.StatusCode, 200);
        }

        [Fact]
        public async Task Test_Add_Professor_Throw_Bad_Request_Due_CPF()
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

            ProfessorForCreateDto professorForCreate = new ProfessorForCreateDto
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

            Professor professor = _mapper.Map<Professor>(professorForCreate);

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);

            try
            {
                var response = await _professorController.Add(professorForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um professor com cpf: {professor.CPF}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Add_Professor_Throw_Bad_Request_Due_RG()
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

            ProfessorForCreateDto professorForCreate = new ProfessorForCreateDto
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

            Professor professor = _mapper.Map<Professor>(professorForCreate);

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);

            try
            {
                var response = await _professorController.Add(professorForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um professor com rg: {professor.RG}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Add_Professor_Throw_Bad_Request_Due_Email()
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

            ProfessorForCreateDto professorForCreate = new ProfessorForCreateDto
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

            Professor professor = _mapper.Map<Professor>(professorForCreate);

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);

            try
            {
                var response = await _professorController.Add(professorForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um professor com email: {professor.Email}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Add_Professor_Throw_Bad_Request_Due_Celular()
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

            ProfessorForCreateDto professorForCreate = new ProfessorForCreateDto
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

            Professor professor = _mapper.Map<Professor>(professorForCreate);

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);

            try
            {
                var response = await _professorController.Add(professorForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"Já existe um professor com celular: {professor.Celular}", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Update_Professor_Must_Work()
        {
            ProfessorForUpdateDto professorForUpdate = new ProfessorForUpdateDto
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

            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
            _repositoryManager.Setup(x => x.ProfessorRepository.UpdateProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _professorController.Update(Guid.NewGuid(), professorForUpdate);

            _repositoryManager.VerifyAll();
            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            
            var result = okResponse.Value as ProfessorDto;

            Assert.NotNull(result);
            Assert.True(result.Match(professor));
            Assert.Equal(okResponse.StatusCode, 200);
        }

        [Fact]
        public async Task Test_Update_Professor_Throw_NotFoundException()
        {
            ProfessorForUpdateDto professorForUpdate = new ProfessorForUpdateDto
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

            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);

            Guid matricula = Guid.NewGuid();

            try
            {
                var response = await _professorController.Update(matricula, professorForUpdate);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Professor com matrícula: {matricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Delete_Professor_Must_Work()
        {
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
            _repositoryManager.Setup(x => x.ProfessorRepository.DeleteProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _professorController.Delete(professor.Matricula);

            _repositoryManager.VerifyAll();
            Assert.NotNull(response);
            Assert.True(response is NoContentResult);

            var noContentResponse = response as NoContentResult;

            Assert.NotNull(noContentResponse);
            Assert.Equal(204, noContentResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Delete_Professor_Throw_NotFoundException()
        {
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);
            
            Guid matricula = Guid.NewGuid();

            try
            {
                var response = await _professorController.Delete(matricula);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Professor com matrícula: {matricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Professor_Must_Work()
        { 
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
           
           var response = await _professorController.Get(professor.Matricula);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as ProfessorDto;

           Assert.NotNull(result);

           Assert.True(result.Match(professor));
           Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_Professor_Throw_NotFoundException()
        {
            Guid matricula = Guid.NewGuid();
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);
           
           try
           {
                var response = await _professorController.Get(matricula);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"Professor com matrícula: {matricula} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }

        [Fact]
        public async Task Test_Get_By_CPF_Professor_Must_Work()
        {
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
           
           var response = await _professorController.GetByCPF(professor.CPF);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as ProfessorDto;

           Assert.NotNull(result);

           Assert.True(result.Match(professor));
           Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_By_CPF_Professor_Throw_NotFoundException()
        {
            const string cpf = "15158114099";
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
           
           try
           {
                var response = await _professorController.GetByCPF(cpf);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"Professor com cpf: {cpf} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }

        [Fact]
        public async Task Test_Get_By_RG_Professor_Must_Work()
        {
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
           
           var response = await _professorController.GetByRG(professor.RG);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as ProfessorDto;

           Assert.NotNull(result);

           Assert.True(result.Match(professor));
           Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_By_RG_Professor_Throw_NotFoundException()
        {
            const string rg = "339404954";
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
           
           try
           {
                var response = await _professorController.GetByRG(rg);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"Professor com rg: {rg} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }

        [Fact]
        public async Task Test_Get_By_Email_Professor_Must_Work()
        {
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
           
           var response = await _professorController.GetByEmail(professor.Email);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as ProfessorDto;

           Assert.NotNull(result);

           Assert.True(result.Match(professor));
           Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_By_Email_Professor_Throw_NotFoundException()
        {
            const string email = "shizuoheiwajima@bol.com";
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
           
           try
           {
                var response = await _professorController.GetByEmail(email);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"Professor com email: {email} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }

        [Fact]
        public async Task Test_Get_By_Celular_Professor_Must_Work()
        {
            Professor professor = new Professor
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

            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
           
           var response = await _professorController.GetByCelular(professor.Celular);

           Assert.NotNull(response);
           Assert.True(response is OkObjectResult);

           var okResponse = response as OkObjectResult;

           Assert.NotNull(okResponse);
         
           var result = okResponse.Value as ProfessorDto;

           Assert.NotNull(result);

           Assert.True(result.Match(professor));
           Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_By_Celular_Professor_Throw_NotFoundException()
        {
            const string celular = "21987654321";
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
           
           try
           {
                var response = await _professorController.GetByCelular(celular);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"Professor com celular: {celular} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }

        [Fact]
        public async Task Test_Filter_Professor_Must_Work()
        {
            var professores = new List<Professor>
            {
                new Professor
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
                }, new Professor
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
                new Professor
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
             
            _repositoryManager.Setup(x => x.ProfessorRepository.GetProfessoresAsync(It.IsAny<GetProfessoresOptions>())).ReturnsAsync(professores);

            var response = await _professorController.Filter(new GetProfessoresOptions());

            Assert.NotNull(response);
            Assert.True(response is OkObjectResult);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            var result = okResponse.Value as Pagination<ProfessorDto>;

            Assert.NotNull(result);

            Assert.Equal(professores.Count, result.Items.Count);

            for(int i = 0; i < professores.Count; ++i)
                Assert.True(result.Items[i].Match(professores[i]));

            Assert.Equal(0, result.CurrentPage);
            Assert.Equal(200, okResponse.StatusCode);
        }
    }
}