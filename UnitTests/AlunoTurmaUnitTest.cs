using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Moq;
using API.Controllers;
using AutoMapper;
using Services;
using Services.Mappers;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Domain.Entities.Enums;
using Domain.Entities;
using Shared.Pagination;
using Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

namespace UnitTests
{
    public class AlunoTurmaUnitTest
    {
        private readonly AlunoTurmaService _alunoTurmaService;
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly ITestOutputHelper _output;

        public AlunoTurmaUnitTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(cfg => cfg.AddProfile<AlunoTurmaProfile>());
            _mapper = config.CreateMapper();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<AlunoTurmaService>();

            _repositoryManager = new Mock<IRepositoryManager>();

            _alunoTurmaService = new AlunoTurmaService(_repositoryManager.Object, logger, _mapper);
        }

        [Fact]
        public async Task Test_AlunoTurma_Create_Must_Work()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

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

            Turma turma = new Turma
            {
                Codigo = Guid.NewGuid(),
                ProfessorMatricula = Guid.NewGuid(),
                 Disciplina = "Matemática",
                 AnoEscolar = Periodo.EF_9,
                 DataInicio = new DateTime(2024, 3, 1),
                 DataFim = new DateTime(2024, 12, 31),
                 HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
            };

            AlunoTurmaForCreateDto alunoTurma = new AlunoTurmaForCreateDto
            {
                AlunoMatricula = aluno.Matricula,
                TurmaCodigo = Guid.NewGuid()
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync(turma);
            alunoTurmaRepository.Setup(x => x.AddAlunoTurma(It.IsAny<AlunoTurma>())).Verifiable();
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);
            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            Guid matricula = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);

            alunoRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        } 
        
        [Fact]
        public async Task Test_AlunoTurma_Create_Shouldnt_Work_Aluno_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync((Aluno?)null);

            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);

            Guid alunoMatricula = Guid.NewGuid();

            AlunoTurmaForCreateDto alunoTurma = new AlunoTurmaForCreateDto
            {
                AlunoMatricula = alunoMatricula,
                TurmaCodigo = Guid.NewGuid()
            };

            try
            {
                _ = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);
                Assert.Fail();
            }
            catch (BadRequestException ex)
            {
                Assert.Equal($"O aluno com matrícula: {alunoMatricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Create_Shouldnt_Work_Turma_NotFound()
        {
            Mock<IAlunoRepository> alunoRepository = new Mock<IAlunoRepository>();
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

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

            Guid turmaCodigo = Guid.NewGuid();

            AlunoTurmaForCreateDto alunoTurma = new AlunoTurmaForCreateDto
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = turmaCodigo
            };

            alunoRepository.Setup(x => x.GetAlunoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoOptions>())).ReturnsAsync(aluno);
            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync((Turma?)null);
            _repositoryManager.SetupGet(x => x.AlunoRepository).Returns(alunoRepository.Object);
            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);
            

            try
            {
                _ = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"A turma com código: {turmaCodigo} não foi encontrada", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_AlunoTurma_AlterarTurma_Must_Work()
        {
            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
                Nota = 5
            };

            AlunoTurmaChangeTurmaDto changeTurmaDto = new AlunoTurmaChangeTurmaDto
            {
                TurmaCodigo = Guid.NewGuid()
            };

            Guid matriculaAluno = Guid.NewGuid();
            Guid codigoTurma = Guid.NewGuid();

            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);
            alunoTurmaRepository.Setup(x => x.UpdateAlunoTurma(It.IsAny<AlunoTurma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            AlunoTurmaDto alunoTurmaDto = await _alunoTurmaService.AlterarTurma(matriculaAluno, codigoTurma, changeTurmaDto);

            alunoTurmaRepository.VerifyAll();
            _repositoryManager.VerifyAll();

            Assert.True(alunoTurmaDto.Match(alunoTurma));
            Assert.Equal(changeTurmaDto.TurmaCodigo, alunoTurmaDto.TurmaCodigo);
        }

        [Fact]
        public async Task Test_AlunoTurma_AlterarTurma_Shouldnt_Work_AlunoTurma_Not_Exists()
        {
           
            AlunoTurmaChangeTurmaDto changeTurmaDto = new AlunoTurmaChangeTurmaDto
            {
                TurmaCodigo = Guid.NewGuid()
            };

            Guid matriculaAluno = Guid.NewGuid();
            Guid codigoTurma = Guid.NewGuid();

            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            try
            {
                _ = await _alunoTurmaService.AlterarTurma(matriculaAluno, codigoTurma, changeTurmaDto);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_AlunoTurma_AlterarNota_Must_Work()
        {
            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
                Nota = 5
            };

            AlunoTurmaChangeNotaDto changeNotaDto = new AlunoTurmaChangeNotaDto
            {
                Nota = 7.0
            };

            Guid matriculaAluno = Guid.NewGuid();
            Guid codigoTurma = Guid.NewGuid();

            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);
            alunoTurmaRepository.Setup(x => x.UpdateAlunoTurma(It.IsAny<AlunoTurma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            AlunoTurmaDto alunoTurmaDto = await _alunoTurmaService.AlterarNota(matriculaAluno, codigoTurma, changeNotaDto);

            alunoTurmaRepository.VerifyAll();
            _repositoryManager.VerifyAll();

            Assert.True(alunoTurmaDto.Match(alunoTurma));
            Assert.Equal(changeNotaDto.Nota, alunoTurmaDto.Nota);
        }

        [Fact]
        public async Task Test_AlunoTurma_AlterarNota_Shouldnt_Work_AlunoTurma_Not_Exists()
        {

            AlunoTurmaChangeNotaDto changeNotaDto = new AlunoTurmaChangeNotaDto
            {
                Nota = 7.0
            };

            Guid matriculaAluno = Guid.NewGuid();
            Guid codigoTurma = Guid.NewGuid();

            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            try
            {
                _ = await _alunoTurmaService.AlterarNota(matriculaAluno, codigoTurma, changeNotaDto);
                Assert.Fail();
            }
            catch (NotFoundException ex)
            {
                Assert.Equal($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_AlunoTurma_Delete_Must_Work()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);
            alunoTurmaRepository.Setup(x => x.DeleteAlunoTurma(It.IsAny<AlunoTurma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            await _alunoTurmaService.DeletarAlunoDaTurma(Guid.NewGuid(), Guid.NewGuid());

            _repositoryManager.VerifyAll();
            alunoTurmaRepository.VerifyAll();
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Delete_Shouldnt_Work_AlunoTurma_NotFound()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            Guid alunoMatricula = Guid.NewGuid(), codigoTurma = Guid.NewGuid();

            try
            {
                await _alunoTurmaService.DeletarAlunoDaTurma(alunoMatricula, codigoTurma);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O aluno com matrícula: {alunoMatricula} na turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Get_Must_Work()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
                Nota = 7
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            AlunoTurmaDto alunoTurmaDto = await _alunoTurmaService.ObterAlunoDaTurma(Guid.NewGuid(), Guid.NewGuid());

            Assert.True(alunoTurmaDto.Match(alunoTurma));
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Get_Shouldnt_Work_AlunoTurma_NotFound()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            Guid alunoMatricula = Guid.NewGuid(), codigoTurma = Guid.NewGuid();

            try
            {
                _ = await _alunoTurmaService.ObterAlunoDaTurma(alunoMatricula, codigoTurma);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O aluno com matrícula: {alunoMatricula} na turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Get_By_Code_Must_Work()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
                Nota = 9
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaPorCodigoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            AlunoTurmaDto alunoTurmaDto = await _alunoTurmaService.ObterAlunoDatTurmaPorCodigo(Guid.NewGuid());

            Assert.True(alunoTurmaDto.Match(alunoTurma));
        }
        
        [Fact]
        public async Task Test_AlunoTurma_Get_By_Code_Shouldnt_Work_AlunoTurma_NotFound()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaPorCodigoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            Guid codigo = Guid.NewGuid();

            try
            {
                _ = await _alunoTurmaService.ObterAlunoDatTurmaPorCodigo(codigo);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"O aluno na turma com código: {codigo}  não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }    
}