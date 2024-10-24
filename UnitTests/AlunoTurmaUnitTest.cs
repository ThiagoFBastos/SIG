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
            Assert.IsType<Guid>(okResponse.Value);

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
    }
}