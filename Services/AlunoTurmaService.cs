using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace Services
{
    public class AlunoTurmaService: IAlunoTurmaService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AlunoTurmaService> _logger;
        private readonly IMapper _mapper;

        public AlunoTurmaService(IRepositoryManager repositoryManager, ILogger<AlunoTurmaService> logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }
         
        public async Task<Guid> CadastrarAlunoNaTurma(AlunoTurmaForCreateDto alunoTurma)
        {
            Guid matriculaAluno = alunoTurma.AlunoMatricula;
 
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoAsync(matriculaAluno);

            if(aluno is null)
            {
                _logger.LogError($"O aluno com matrícula: {matriculaAluno} não foi encontrado");
                throw new BadRequestException($"O aluno com matrícula: {matriculaAluno} não foi encontrado");
            }

            Guid codigoTurma = alunoTurma.TurmaCodigo;

            Turma? turma = await _repositoryManager.TurmaRepository.GetTurmaAsync(codigoTurma);

            if(turma is null)
            {
                _logger.LogError($"A turma com código: {codigoTurma} não foi encontrada");
                throw new BadRequestException($"A turma com código: {codigoTurma} não foi encontrada");
            }

            AlunoTurma alunoTurmaReal = _mapper.Map<AlunoTurma>(alunoTurma);
            
            _repositoryManager.AlunoTurmaRepository.AddAlunoTurma(alunoTurmaReal);
            await _repositoryManager.SaveAsync();

            return alunoTurmaReal.Codigo;
        }

        public async Task<AlunoTurmaDto> AlterarAlunoNaTurma(Guid matriculaAluno, Guid codigoTurma, AlunoTurmaForUpdateDto alunoTurma)
        {
            AlunoTurma? alunoTurmaReal = await _repositoryManager.AlunoTurmaRepository.GetAlunoTurmaAsync(matriculaAluno, codigoTurma);

            if(alunoTurmaReal is null)
            {
                _logger.LogError($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
            }

            _mapper.Map(alunoTurma, alunoTurmaReal, typeof(AlunoTurma), typeof(AlunoTurmaForUpdateDto));

            _repositoryManager.AlunoTurmaRepository.UpdateAlunoTurma(alunoTurmaReal);
            await _repositoryManager.SaveAsync();

            AlunoTurmaDto alunoTurmaRetornado = _mapper.Map<AlunoTurmaDto>(alunoTurmaReal);

            return alunoTurmaRetornado;
        }

        public async Task DeletarAlunoDaTurma(Guid matriculaAluno, Guid codigoTurma)
        {
            AlunoTurma? alunoTurma = await _repositoryManager.AlunoTurmaRepository.GetAlunoTurmaAsync(matriculaAluno, codigoTurma);

            if(alunoTurma is null)
            {
                _logger.LogError($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
            }

            _repositoryManager.AlunoTurmaRepository.DeleteAlunoTurma(alunoTurma);
            await _repositoryManager.SaveAsync();
        }

        public async Task<AlunoTurmaDto> ObterAlunoDaTurma(Guid matriculaAluno, Guid codigoTurma, GetAlunoTurmaOptions? opcoes = null)
        {
            AlunoTurma? alunoTurma = await _repositoryManager.AlunoTurmaRepository.GetAlunoTurmaAsync(matriculaAluno, codigoTurma, opcoes);

            if(alunoTurma is null)
            {
                _logger.LogError($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"O aluno com matrícula: {matriculaAluno} na turma com código: {codigoTurma} não foi encontrado");
            }

            AlunoTurmaDto alunoturmaRetornado = _mapper.Map<AlunoTurmaDto>(alunoTurma);

            return alunoturmaRetornado;
        }

        public async Task<AlunoTurmaDto> ObterAlunoDatTurmaPorCodigo(Guid codigo, GetAlunoTurmaOptions? opcoes = null)
        {
            AlunoTurma? alunoTurma = await _repositoryManager.AlunoTurmaRepository.GetAlunoTurmaPorCodigoAsync(codigo, opcoes);

            if(alunoTurma is null)
            {
                _logger.LogError($"O aluno na turma com código: {codigo}  não foi encontrado");
                throw new NotFoundException($"O aluno na turma com código: {codigo}  não foi encontrado");
            }

            AlunoTurmaDto alunoturmaRetornado = _mapper.Map<AlunoTurmaDto>(alunoTurma);

            return alunoturmaRetornado;
        }
    }
} 