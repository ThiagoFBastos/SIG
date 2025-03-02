using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using API.Controllers;
using Moq;
using AutoMapper;
using Services;
using Domain.Repositories;
using Services.Mappers;
using Domain.Entities;
using Domain.Entities.Enums;
using Shared.Pagination;
using Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

namespace UnitTests
{
    public class TurmaUnitTest
    {
        private readonly TurmaService _turmaService;
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryManager> _repositoryManager;
        private readonly ITestOutputHelper _output;

        public TurmaUnitTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new MapperConfiguration(cfg => cfg.AddProfile<TurmaProfile>());
            _mapper = config.CreateMapper();

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());

            var logger = factory.CreateLogger<TurmaService>();

            _repositoryManager = new Mock<IRepositoryManager>();

            _turmaService = new TurmaService(_repositoryManager.Object, logger, _mapper);
        }

        [Fact]
        public async Task Test_Create_Turma_Must_Work()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

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

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), It.IsAny<GetProfessorOptions>())).ReturnsAsync(professor);

            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            TurmaForCreateDto turma = new TurmaForCreateDto
            {
                 ProfessorMatricula = professor.Matricula,
                 Disciplina = "Matemática",
                 AnoEscolar = (int)Periodo.EF_9,
                 DataInicio = new DateTime(2024, 3, 1),
                 DataFim = new DateTime(2024, 12, 31),
                 HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
            };

            var turmaRepository = new Mock<ITurmaRepository>();
            turmaRepository.Setup(x => x.AddTurma(It.IsAny<Turma>())).Verifiable();
            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            Guid codigo = await _turmaService.CadastrarTurma(turma);

            turmaRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }
        
        [Fact]
        public async Task Test_Create_Turma_Shouldnt_Work_Professor_NotExists()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), It.IsAny<GetProfessorOptions>())).ReturnsAsync((Professor?)null);

            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Guid professorMatricula = Guid.NewGuid();
            TurmaForCreateDto turma = new TurmaForCreateDto
            {
                 ProfessorMatricula = professorMatricula,
                 Disciplina = "Matemática",
                 AnoEscolar = (int)Periodo.EF_9,
                 DataInicio = new DateTime(2024, 3, 1),
                 DataFim = new DateTime(2024, 12, 31),
                 HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
            };

            try
            {
                _ = await _turmaService.CadastrarTurma(turma);
                Assert.Fail();
            }
            catch(BadRequestException ex)
            {
                Assert.Equal($"O professor com matrícula: {professorMatricula} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Update_Turma_Must_Work()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            Professor professor = new Professor
            {
                Matricula = Guid.NewGuid(),
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

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), It.IsAny<GetProfessorOptions>())).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            Turma turma = new Turma
            {
                Codigo = Guid.NewGuid(),
                ProfessorMatricula = professor.Matricula,
                 Disciplina = "Matemática",
                 AnoEscolar = Periodo.EF_9,
                 DataInicio = new DateTime(2024, 3, 1),
                 DataFim = new DateTime(2024, 12, 31),
                 HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
            }; 

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync(turma);
            turmaRepository.Setup(x => x.UpdateTurma(It.IsAny<Turma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);

            Guid codigoTurma = Guid.NewGuid();
            TurmaForUpdateDto turmaForUpdate = new TurmaForUpdateDto
            {
                 ProfessorMatricula = professor.Matricula,
                 DataInicio = new DateTime(2024, 4, 1),
                 DataFim = new DateTime(2024, 10, 31),
                 HorarioAulaInicio = new DateTime(1, 2, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 19, 0, 0),
            };

            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            TurmaDto turmaDto = await _turmaService.AlterarTurma(codigoTurma, turmaForUpdate);

            _repositoryManager.VerifyAll();
            turmaRepository.VerifyAll();

            Assert.True(turmaDto.Match(turma));
        }
        
        [Fact]
        public async Task Test_Update_Turma_Shouldnt_Work_Professor_NotFound()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), It.IsAny<GetProfessorOptions>())).ReturnsAsync((Professor?)null);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            Guid matriculaProfessor = Guid.NewGuid();

            Guid codigoTurma = Guid.NewGuid();
            TurmaForUpdateDto turmaForUpdate = new TurmaForUpdateDto
            {
                 ProfessorMatricula = matriculaProfessor,
                 DataInicio = new DateTime(2024, 4, 1),
                 DataFim = new DateTime(2024, 10, 31),
                 HorarioAulaInicio = new DateTime(1, 2, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 19, 0, 0),
            };

           try
           {
                _ = await _turmaService.AlterarTurma(codigoTurma, turmaForUpdate);
                Assert.Fail();
           }
           catch(BadRequestException ex)
           {
                Assert.Equal($"O professor com matrícula: {matriculaProfessor} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Update_Turma_Shouldnt_Work_Turma_NotFound()
        {
            Mock<IProfessorRepository> professorRepository = new Mock<IProfessorRepository>();
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            Professor professor = new Professor
            {
                Matricula = Guid.NewGuid(),
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

            professorRepository.Setup(x => x.GetProfessorAsync(It.IsAny<Guid>(), It.IsAny<GetProfessorOptions>())).ReturnsAsync(professor);
            _repositoryManager.SetupGet(x => x.ProfessorRepository).Returns(professorRepository.Object);

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync((Turma?)null);
            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);

            Guid matriculaProfessor = Guid.NewGuid();

            Guid codigoTurma = Guid.NewGuid();
            TurmaForUpdateDto turmaForUpdate = new TurmaForUpdateDto
            {
                 ProfessorMatricula = matriculaProfessor,
                 DataInicio = new DateTime(2024, 4, 1),
                 DataFim = new DateTime(2024, 10, 31),
                 HorarioAulaInicio = new DateTime(1, 2, 1, 8, 0, 0),
                 HorarioAulaFim = new DateTime(1, 1, 1, 19, 0, 0),
            };

           try
           {
                _ = await _turmaService.AlterarTurma(codigoTurma, turmaForUpdate);
                Assert.Fail();
           }
           catch(NotFoundException ex)
           {
                Assert.Equal($"A turma com código: {codigoTurma} não foi encontrado", ex.Message);
           }
           catch
           {
                Assert.Fail();
           }
        }
        
        [Fact]
        public async Task Test_Delete_Turma_Must_Work()
        {
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

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

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync(turma);
            turmaRepository.Setup(x => x.DeleteTurma(It.IsAny<Turma>())).Verifiable();

            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);
            _repositoryManager.Setup(x => x.SaveAsync()).Verifiable();

            await _turmaService.DeletarTurma(turma.Codigo);

            turmaRepository.VerifyAll();
            _repositoryManager.VerifyAll();
        }
        
        [Fact]
        public async Task Test_Delete_Turma_Shouldnt_TurmaNotFound()
        {
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync((Turma?)null);

            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);

            Guid codigoTurma = Guid.NewGuid();

            try
            {
                await _turmaService.DeletarTurma(codigoTurma);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"A turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }
        
        [Fact]
        public async Task Test_Get_Turma_Must_Work()
        {
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

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

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync(turma);

            _repositoryManager.Setup(x => x.TurmaRepository).Returns(turmaRepository.Object);

            TurmaDto turmaDto = await _turmaService.ObterTurmaPorCodigo(turma.Codigo);

            Assert.True(turmaDto.Match(turma));
        }

        [Fact]
        public async Task Test_Get_Turma_Shouldnt_Work_Turma_Not_Found()
        {
            Guid codigoTurma = Guid.NewGuid();
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync((Turma?)null);

            _repositoryManager.Setup(x => x.TurmaRepository).Returns(turmaRepository.Object);

            try
            {
                _ = await _turmaService.ObterTurmaPorCodigo(codigoTurma);
                Assert.Fail();
            }
            catch(NotFoundException ex)
            {
                Assert.Equal($"A turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Get_TurmaSemNota_Must_Work()
        {
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

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

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync(turma);

            _repositoryManager.Setup(x => x.TurmaRepository).Returns(turmaRepository.Object);

            TurmaSemNotaDto turmaDto = await _turmaService.ObterTurmaPorCodigoSemNota(turma.Codigo);

            Assert.True(turmaDto.Match(turma));
        }

        [Fact]
        public async Task Test_Get_TurmaSemNota_Shouldnt_Work_Turma_Not_Found()
        {
            Guid codigoTurma = Guid.NewGuid();
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            turmaRepository.Setup(x => x.GetTurmaAsync(It.IsAny<Guid>(), It.IsAny<GetTurmaOptions>())).ReturnsAsync((Turma?)null);

            _repositoryManager.Setup(x => x.TurmaRepository).Returns(turmaRepository.Object);

            try
            {
                _ = await _turmaService.ObterTurmaPorCodigoSemNota(codigoTurma);
                Assert.Fail();
            }
            catch (NotFoundException ex)
            {
                Assert.Equal($"A turma com código: {codigoTurma} não foi encontrado", ex.Message);
            }
            catch
            {
                Assert.Fail();
            }
        }

        [Fact]
        public async Task Test_Filter_Turmas_Must_Work()
        {
            Mock<ITurmaRepository> turmaRepository = new Mock<ITurmaRepository>();

            List<Turma>  turmas = new List<Turma>
            { 
                new Turma
                {
                    Codigo = Guid.NewGuid(),
                    ProfessorMatricula = Guid.NewGuid(),
                    Disciplina = "Matemática",
                    AnoEscolar = Periodo.EF_9,
                    DataInicio = new DateTime(2024, 3, 1),
                    DataFim = new DateTime(2024, 12, 31),
                    HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                    HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
                },
                new Turma
                {
                    Codigo = Guid.NewGuid(),
                    ProfessorMatricula = Guid.NewGuid(),
                    Disciplina = "Portugûes", 
                    AnoEscolar = Periodo.EF_9,
                    DataInicio = new DateTime(2024, 3, 1),
                    DataFim = new DateTime(2024, 12, 31),
                    HorarioAulaInicio = new DateTime(1, 1, 1, 8, 0, 0),
                    HorarioAulaFim = new DateTime(1, 1, 1, 18, 0, 0),
                }
            };

            turmaRepository.Setup(x => x.GetTurmasAsync(It.IsAny<GetTurmasOptions>())).ReturnsAsync(turmas);
            _repositoryManager.SetupGet(x => x.TurmaRepository).Returns(turmaRepository.Object);

            Pagination<TurmaDto> pagination = await _turmaService.ObterTurmas(new GetTurmasOptions() { });

            for(int i = 0; i < pagination.Items.Count; ++i)
                Assert.True(pagination.Items[i].Match(turmas[i]));
        }
    }
}