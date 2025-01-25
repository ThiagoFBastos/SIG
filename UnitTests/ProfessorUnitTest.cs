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
        private readonly ProfessorService _professorService;
        private readonly Mock<IRepositoryManager> _repositoryManager;
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

            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.AddProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);
            _enderecoService.Setup(x => x.CadastrarEndereco(It.IsAny<EnderecoForCreateDto>())).ReturnsAsync(Guid.NewGuid());
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            Guid matricula = await _professorService.CadastrarProfessor(professor);

            professorRepository.VerifyAll();
            _repositoryManager.VerifyAll();
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            try
            {
                _ = await _professorService.CadastrarProfessor(professorForCreate);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();


            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            try
            {
                _ = await _professorService.CadastrarProfessor(professorForCreate);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            try
            {
                _ = await _professorService.CadastrarProfessor(professorForCreate);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            professorRepository.Setup(x => x.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            try
            {
                _ = await _professorService.CadastrarProfessor(professorForCreate);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
            professorRepository.Setup(x => x.UpdateProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            ProfessorDto professorDto = await _professorService.AlterarProfessor(Guid.NewGuid(), professorForUpdate);

            professorRepository.VerifyAll();
            _repositoryManager.VerifyAll();

            Assert.True(professorDto.Match(professor));
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

            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();
            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Guid matricula = Guid.NewGuid();

            try
            {
                _ = await _professorService.AlterarProfessor(matricula, professorForUpdate);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
            professorRepository.Setup(x => x.DeleteProfessor(It.IsAny<Professor>())).Verifiable();
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            await _professorService.DeletarProfessor(professor.Matricula);

            professorRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }
        
        [Fact]
        public async Task Test_Delete_Professor_Throw_NotFoundException()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Guid matricula = Guid.NewGuid();

            try
            {
                await _professorService.DeletarProfessor(matricula);
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

            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           ProfessorDto professorDto = await _professorService.ObterProfessorPorMatricula(professor.Matricula);

            Assert.True(professorDto.Match(professor));
        }
        
        [Fact]
        public async Task Test_Get_Professor_Throw_NotFoundException()
        {
            Guid matricula = Guid.NewGuid();
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           try
           {
                _ = await _professorService.ObterProfessorPorMatricula(matricula);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            ProfessorDto professorDto = await _professorService.ObterProfessorPorCPF(professor.CPF);

            Assert.True(professorDto.Match(professor));
        }
        
        [Fact]
        public async Task Test_Get_By_CPF_Professor_Throw_NotFoundException()
        {
            const string cpf = "15158114099";
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorCPFAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           try
           {
                _ = await _professorService.ObterProfessorPorCPF(cpf);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            ProfessorDto professorDto = await _professorService.ObterProfessorPorRG(professor.RG);

            Assert.True(professorDto.Match(professor));

        }
        
        [Fact]
        public async Task Test_Get_By_RG_Professor_Throw_NotFoundException()
        {
            const string rg = "339404954";
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPorRGAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           try
           {
                _ = await _professorService.ObterProfessorPorRG(rg);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           ProfessorDto professorDto = await _professorService.ObterProfessorPeloEmail(professor.Email);

            Assert.True(professorDto.Match(professor));
        }
        
        [Fact]
        public async Task Test_Get_By_Email_Professor_Throw_NotFoundException()
        {
            const string email = "shizuoheiwajima@bol.com";
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPeloEmailAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           try
           {
                _ = await _professorService.ObterProfessorPeloEmail(email);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

           ProfessorDto professorDto = await _professorService.ObterProfessorPeloCelular(professor.Celular);

            Assert.True(professorDto.Match(professor));
        }
        
        [Fact]
        public async Task Test_Get_By_Celular_Professor_Throw_NotFoundException()
        {
            const string celular = "21987654321";
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorPeloCelularAsync(It.IsAny<string>(), null)).ReturnsAsync((Professor?)null);
           _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository?.Object);

           try
           {
                _ = await _professorService.ObterProfessorPeloCelular(celular);
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
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessoresAsync(It.IsAny<GetProfessoresOptions>())).ReturnsAsync(professores);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Pagination<ProfessorDto> pagination = await _professorService.ObterProfessores(new GetProfessoresOptions());

            Assert.Equal(pagination.Items.Count, professores.Count);

            for (int i = 0; i < professores.Count; ++i)
                Assert.True(pagination.Items[i].Match(professores[i]));
        }
    }
}