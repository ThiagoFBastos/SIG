using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IMapper _mapper;
        private readonly AlunoService _alunoService;
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

            _alunoService = new AlunoService(_repositoryManager.Object, logger, _mapper, _enderecoService.Object);
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

            Guid matricula = await _alunoService.CadastrarAluno(aluno);

            alunoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
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
                _ = await _alunoService.CadastrarAluno(alunoForCreate);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                switch (doc)
                {
                    case "CPF":
                        Assert.Equal($"Já existe um aluno com cpf: {aluno.CPF}", ex.Message);
                        break;
                    case "RG":
                        Assert.Equal($"Já existe um aluno com rg: {aluno.RG}", ex.Message);
                        break;
                    case "Email":
                        Assert.Equal($"Já existe um aluno com email: {aluno.Email}", ex.Message);
                        break;
                    case "Celular":
                        Assert.Equal($"Já existe um aluno com celular: {aluno.Celular}", ex.Message);
                        break;
                    default:
                        Assert.Fail();
                        break;
                }
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

            AlunoDto alunoDto = await _alunoService.AlterarAluno(alunoMatricula, alunoForUpdate);

            _repositoryManager.VerifyAll();
            alunoRepository.VerifyAll();

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
                _ = await _alunoService.AlterarAluno(alunoMatricula, alunoForUpdate);
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

             await _alunoService.DeletarAluno(matriculaAluno);

            alunoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
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
                await _alunoService.DeletarAluno(matriculaAluno);
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

            AlunoDto alunoDto = await _alunoService.ObterAlunoPorMatricula(matriculaAluno);

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
                _ = await _alunoService.ObterAlunoPorMatricula(matriculaAluno);
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

            AlunoDto alunoDto = await _alunoService.ObterAlunoPorCPF(aluno.CPF);

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
                _ = await _alunoService.ObterAlunoPorCPF(cpf);
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
            
            AlunoDto alunoDto  = await _alunoService.ObterAlunoPorRG(aluno.RG);

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
                _ = await _alunoService.ObterAlunoPorRG(rg);
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

            AlunoDto alunoDto = await _alunoService.ObterAlunoPeloEmail(aluno.Email);

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
                _ = await _alunoService.ObterAlunoPeloEmail(email);
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

            AlunoDto alunoDto = await _alunoService.ObterAlunoPeloCelular(aluno.Celular);

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
                _ = await _alunoService.ObterAlunoPeloCelular(celular);
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

            Pagination<AlunoDto> pagination = await _alunoService.ObterAlunos(new GetAlunosOptions { });

            Assert.Equal(pagination.Items.Count, alunos.Count);
            
            for(int i = 0; i < alunos.Count; ++i)
                Assert.True(pagination.Items[i].Match(alunos[i]));
        }
    }
}