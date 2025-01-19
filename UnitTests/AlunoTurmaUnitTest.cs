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
        //_
        private readonly AlunoTurmaController _alunoTurmaController;
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

            AlunoTurmaService alunoTurmaService = new AlunoTurmaService(_repositoryManager.Object, logger, _mapper);

            _alunoTurmaController = new AlunoTurmaController(alunoTurmaService);
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

            var response = await _alunoTurmaController.Add(alunoTurma);

            _repositoryManager.VerifyAll();
            alunoTurmaRepository.VerifyAll();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            Assert.True(okResponse.Value is GuidResponseDto);
            Assert.Equal(200, okResponse.StatusCode);
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
                var response = await _alunoTurmaController.Add(alunoTurma);
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
                var response = await _alunoTurmaController.Add(alunoTurma);
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
        public async Task Test_AlunoTurma_Update_Must_Work()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            AlunoTurma alunoTurma = new AlunoTurma
            {
                AlunoMatricula = Guid.NewGuid(),
                TurmaCodigo = Guid.NewGuid(),
            };

            AlunoTurmaForUpdateDto alunoTurmaForUpdate = new AlunoTurmaForUpdateDto
            {
                TurmaCodigo = Guid.NewGuid(),
                Nota = 10
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);
            alunoTurmaRepository.Setup(x => x.UpdateAlunoTurma(It.IsAny<AlunoTurma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            var response = await _alunoTurmaController.Update(Guid.NewGuid(), Guid.NewGuid(), alunoTurmaForUpdate);

            _repositoryManager.VerifyAll();
            alunoTurmaRepository.VerifyAll();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);
            Assert.IsType<AlunoTurmaDto>(okResponse.Value);

            AlunoTurmaDto? result = okResponse.Value as AlunoTurmaDto;

            Assert.NotNull(result);

            Assert.Equal(200, okResponse.StatusCode);
        }

        [Fact]
        public async Task Test_AlunoTurma_Update_Shouldnt_Work_AlunoTurma_NotFound()
        {
            Mock<IAlunoTurmaRepository> alunoTurmaRepository = new Mock<IAlunoTurmaRepository>();

            AlunoTurmaForUpdateDto alunoTurmaForUpdate = new AlunoTurmaForUpdateDto
            {
                TurmaCodigo = Guid.NewGuid(),
                Nota = 10
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync((AlunoTurma?)null);
            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            Guid alunoMatricula = Guid.NewGuid(), codigoTurma = Guid.NewGuid();

            try
            {
                var response = await _alunoTurmaController.Update(alunoMatricula, codigoTurma, alunoTurmaForUpdate);
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

            var response = await _alunoTurmaController.Delete(Guid.NewGuid(), Guid.NewGuid());

            _repositoryManager.VerifyAll();
            alunoTurmaRepository.VerifyAll();

            Assert.NotNull(response);
            Assert.IsType<NoContentResult>(response);

            var noContentResponse = response as NoContentResult;

            Assert.NotNull(noContentResponse);

            Assert.Equal(204, noContentResponse.StatusCode);
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
                var response = await _alunoTurmaController.Delete(alunoMatricula, codigoTurma);
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
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            var response = await _alunoTurmaController.Get(Guid.NewGuid(), Guid.NewGuid());

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoTurmaDto>(okResponse.Value);

            AlunoTurmaDto? result = okResponse.Value as AlunoTurmaDto;

            Assert.NotNull(result);

            Assert.True(result.Match(alunoTurma));

            Assert.Equal(200, okResponse.StatusCode);
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
                var response = await _alunoTurmaController.Get(alunoMatricula, codigoTurma);
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
            };

            alunoTurmaRepository.Setup(x => x.GetAlunoTurmaPorCodigoAsync(It.IsAny<Guid>(), It.IsAny<GetAlunoTurmaOptions>())).ReturnsAsync(alunoTurma);

            _repositoryManager.SetupGet(x => x.AlunoTurmaRepository).Returns(alunoTurmaRepository.Object);

            var response = await _alunoTurmaController.Get(Guid.NewGuid());

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response);

            var okResponse = response as OkObjectResult;

            Assert.NotNull(okResponse);

            Assert.IsType<AlunoTurmaDto>(okResponse.Value);

            AlunoTurmaDto? result = okResponse.Value as AlunoTurmaDto;

            Assert.NotNull(result);

            Assert.True(result.Match(alunoTurma));

            Assert.Equal(200, okResponse.StatusCode);
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
                var response = await _alunoTurmaController.Get(codigo);
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