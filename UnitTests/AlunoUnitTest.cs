using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using AutoMapper;
using Domain.Repositories;
using Moq;
using Services.Contracts;
using Services.Mappers;
using Xunit;
using Microsoft.Extensions.Logging;
using Services;
using Domain.Entities;
using Shared.Pagination;
using Shared.Dtos;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;
using Xunit.Abstractions;

namespace UnitTests
{
    public class AlunoUnitTest
    {
        //_
        private readonly IMapper _mapper;
        private readonly AlunoController _alunoController;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly Mock<IEnderecoService> _enderecoService;
        private readonly ITestOutputHelper _output;

        public AlunoUnitTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AlunoProfile>());
            _mapper = config.CreateMapper();

            _repositoryManager = new Mock<IRepositoryManager>();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<AlunoService>();

            _enderecoService = new Mock<IEnderecoService>();

            var alunoService = new AlunoService(_repositoryManager.Object, logger, _mapper, _enderecoService.Object);

            _alunoController = new AlunoController(alunoService);
        }
 
        [Fact]
        public async Task Test_Add_Aluno_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            
            alunoRepository.Setup(x => x.GetAlunoPorCPFAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPorRGAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPeloEmailAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPeloCelularAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            alunoRepository.Setup(x => x.AddAluno(It.IsAny<Aluno>())).Verifiable();

            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);
            _enderecoService.Setup(x => x.CadastrarEndereco(It.IsAny<EnderecoForCreateDto>())).Verifiable();
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            AlunoForCreateDto aluno = new AlunoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Endereco = new EnderecoForCreateDto {
                    Cidade = "Rio de Janeiro",
                    Estado = (int)Estado.RJ,
                    Rua = "Rua Sete de Setembro",
                    Casa = 111,
                    Complemento = "Fundos casa 2",
                    CEP = "20050006"
                },
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M,
                AnoEscolar = (int)Periodo.EM_3,
                Status = (int)StatusMatricula.ATIVO,
                Turno = (int)Turno.MANHA
            };

            var response = await _alunoController.Add(aluno);

            _repositoryManager.VerifyAll();
            _enderecoService.VerifyAll();
            alunoRepository.VerifyAll();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            Assert.IsType<Guid>(okResponse.Value);
            Assert.Equal(200, okResponse.StatusCode);
        }

        [Theory]
        [InlineData("CPF")]
        [InlineData("RG")]
        [InlineData("Email")]
        [InlineData("Celular")]
        public async Task Test_Add_Aluno_Should_Fail_Document_Alreary_Exist(string doc)
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            }; 

            AlunoForCreateDto alunoForCreate = new AlunoForCreateDto
            {
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Endereco = new EnderecoForCreateDto {
                    Cidade = "Rio de Janeiro",
                    Estado = (int)Estado.RJ,
                    Rua = "Rua Sete de Setembro",
                    Casa = 111,
                    Complemento = "Fundos casa 2",
                    CEP = "20050006"
                },
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = (int)Sexo.M,
                AnoEscolar = (int)Periodo.EM_3,
                Status = (int)StatusMatricula.ATIVO,
                Turno = (int)Turno.MANHA
            };
            
            alunoRepository.Setup(x => x.GetAlunoPorCPFAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(doc == "CPF" ? aluno : (Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPorRGAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(doc == "RG" ? aluno : (Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPeloEmailAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(doc == "Email" ? aluno : (Aluno?)null);
            alunoRepository.Setup(x => x.GetAlunoPeloCelularAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(doc == "Celular" ? aluno: (Aluno?)null);
        
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.Add(alunoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException)
            {

            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Update_Aluno_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            
            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            alunoRepository.Setup(x => x.UpdateAluno(It.IsAny<Aluno>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            AlunoForUpdateDto alunoForUpdate = new AlunoForUpdateDto
            {
                Email = "ouzihs@lob.moc",
                Celular = "21876543219",
                AnoEscolar = (int)Periodo.EM_1,
                Status = (int)StatusMatricula.CONCLUIDO,
                Turno = (int)Turno.TARDE
            };

            Guid alunoMatricula = Guid.NewGuid();

            var response = await _alunoController.Update(alunoMatricula, alunoForUpdate);

            _repositoryManager.VerifyAll();
            alunoRepository.VerifyAll();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;
 
            Assert.NotNull(okResponse);
            Assert.Equal(200, okResponse.StatusCode);
            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Update_Aluno_Should_Work_Aluno_Already_Exist()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            
            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            AlunoForUpdateDto alunoForUpdate = new AlunoForUpdateDto
            {
                Email = "ouzihs@lob.moc",
                Celular = "21876543219",
                AnoEscolar = (int)Periodo.EM_1,
                Status = (int)StatusMatricula.CONCLUIDO,
                Turno = (int)Turno.TARDE
            };

            Guid alunoMatricula = Guid.NewGuid();

            try
            {
                var response = await _alunoController.Update(alunoMatricula, alunoForUpdate);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com matrícula: {alunoMatricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Delete_Aluno_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Guid matriculaAluno = Guid.NewGuid();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            alunoRepository.Setup(x => x.DeleteAluno(It.IsAny<Aluno>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _alunoController.Delete(matriculaAluno);

            Assert.NotNull(response);
            Assert.IsType<NoContentResult>(response);

            var okResponse = response as NoContentResult;

            Assert.NotNull(okResponse);

            Assert.Equal(204, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Delete_Aluno_Shouldnt_Work_Aluno_Dont_Exists()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Guid matriculaAluno = Guid.NewGuid();

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.Delete(matriculaAluno);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com matrícula: {matriculaAluno} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Matricula_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Guid matriculaAluno = Guid.NewGuid();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.Get(matriculaAluno);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.Equal(200, okResponse.StatusCode);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Matricula_Shouldnt_Work_Aluno_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Guid matriculaAluno = Guid.NewGuid();
            
            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.Get(matriculaAluno);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com matrícula: {matriculaAluno} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Aluno_By_CPF_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoPorCPFAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.GetByCPF("12345678910");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.Equal(200, okResponse.StatusCode);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Get_Aluno_By_CPF_Shouldnt_Work_CPF_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            string cpf = "12345678910";

            alunoRepository.Setup(x => x.GetAlunoPorCPFAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.GetByCPF(cpf);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com cpf: {cpf} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Aluno_By_RG_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoPorRGAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.GetByRG("489784561");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.Equal(200, okResponse.StatusCode);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Get_Aluno_By_RG_Shouldnt_Work_RG_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            string rg = "489784561";

            alunoRepository.Setup(x => x.GetAlunoPorRGAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.GetByRG(rg);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com rg: {rg} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Email_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoPeloEmailAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.GetByEmail("fulanodetal@gmail.com");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.Equal(200, okResponse.StatusCode);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Email_Shouldnt_Work_Email_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            string email = "fulanodetal@gmail.com";

            alunoRepository.Setup(x => x.GetAlunoPeloEmailAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.GetByEmail(email);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com email: {email} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Celular_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();

            Aluno aluno = new Aluno
            {
                Matricula = Guid.NewGuid(),
                NomeCompleto = "Shizuo Heiwajima",
                CPF = "15158114099",
                RG = "339404954",
                EnderecoId = Guid.NewGuid(),
                Email = "shizuo@bol.com",
                Celular = "21912345678",
                DataNascimento = new DateTime(1980, 12, 31),
                Sexo = Sexo.M,
                AnoEscolar = Periodo.EM_3,
                Status = StatusMatricula.ATIVO,
                Turno = Turno.MANHA
            };

            alunoRepository.Setup(x => x.GetAlunoPeloCelularAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.GetByCelular("21998765432");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoDto>(okResponse.Value);

            AlunoDto? alunoDto = okResponse.Value as AlunoDto;

            Assert.NotNull(alunoDto);

            Assert.Equal(200, okResponse.StatusCode);

            Assert.True(alunoDto.Match(aluno));
        }

        [Fact]
        public async Task Test_Get_Aluno_By_Celular_Shouldnt_Work_Celular_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            string celular = "21998765432";

            alunoRepository.Setup(x => x.GetAlunoPeloCelularAsync(It.IsAny<string>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            try
            {
                var response = await _alunoController.GetByCelular(celular);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"Aluno com celular: {celular} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_All_Alunos_Must_Work()
        {
            List<Aluno> alunos = new List<Aluno>
            {
                new Aluno
                {
                    Matricula = Guid.NewGuid(),
                    NomeCompleto = "Shizuo Heiwajima",
                    CPF = "15158114099",
                    RG = "339404954",
                    EnderecoId = Guid.NewGuid(),
                    Email = "shizuo@bol.com",
                    Celular = "21912345678",
                    DataNascimento = new DateTime(1980, 12, 31),
                    Sexo = Sexo.M,
                    AnoEscolar = Periodo.EM_3,
                    Status = StatusMatricula.ATIVO,
                    Turno = Turno.MANHA
                },
                new Aluno
                {
                    Matricula = Guid.NewGuid(),
                    NomeCompleto = "Lelouch Lamperouge",
                    CPF = "15152134099",
                    RG = "339404954",
                    EnderecoId = Guid.NewGuid(),
                    Email = "lelouch@yahoo.com",
                    Celular = "21912245678",
                    DataNascimento = new DateTime(2000, 10, 9),
                    Sexo = Sexo.M,
                    AnoEscolar = Periodo.EM_3,
                    Status = StatusMatricula.ATIVO,
                    Turno = Turno.TARDE
                },
                new Aluno
                {
                    Matricula = Guid.NewGuid(),
                    NomeCompleto = "Light Yagami",
                    CPF = "35458514099",
                    RG = "329304554",
                    EnderecoId = Guid.NewGuid(),
                    Email = "yagami@gmail.com",
                    Celular = "21923345478",
                    DataNascimento = new DateTime(2001, 5, 13),
                    Sexo = Sexo.M,
                    AnoEscolar = Periodo.EM_3,
                    Status = StatusMatricula.ATIVO,
                    Turno = Turno.MANHA
                }
            };

            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            alunoRepository.Setup(x => x.GetAlunosAsync(It.IsAny<GetAlunosOptions>())).ReturnsAsync(alunos);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            var response = await _alunoController.Filter(new GetAlunosOptions {});

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<Pagination<AlunoDto>>(okResponse.Value);

            Pagination<AlunoDto>? pagination = okResponse.Value as Pagination<AlunoDto>;

            Assert.NotNull(pagination);
            Assert.Equal(200, okResponse.StatusCode);

            Assert.Equal(pagination.Items.Count, alunos.Count);
            
            for(int i = 0; i < alunos.Count; ++i)
                Assert.True(pagination.Items[i].Match(alunos[i]));
        }
    }
}