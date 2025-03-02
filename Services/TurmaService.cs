using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;
using Domain.Entities;
using Domain.Exceptions;
using System.Text.Json;

namespace Services
{
    public class TurmaService: ITurmaService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<TurmaService> _logger;
        private readonly IMapper _mapper;
  
        public TurmaService(IRepositoryManager repositoryManager, ILogger<TurmaService> logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }
 
        public async Task<Guid> CadastrarTurma(TurmaForCreateDto turma)
        {
            Guid matriculaProfessor = turma.ProfessorMatricula;
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorAsync(matriculaProfessor);

            if(professor is null)
            {
                _logger.LogError($"O professor com matrícula: {matriculaProfessor} não foi encontrado");
                throw new BadRequestException($"O professor com matrícula: {matriculaProfessor} não foi encontrado");
            }

            Turma turmaReal = _mapper.Map<Turma>(turma);

            _repositoryManager.TurmaRepository.AddTurma(turmaReal);
            await _repositoryManager.SaveAsync();

            return turmaReal.Codigo;
        }

        public async Task<TurmaDto> AlterarTurma(Guid codigoTurma, TurmaForUpdateDto turma)
        {
            Guid matriculaProfessor = turma.ProfessorMatricula;
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorAsync(matriculaProfessor);

            if(professor is null)
            {
                _logger.LogError($"O professor com matrícula: {matriculaProfessor} não foi encontrado");
                throw new BadRequestException($"O professor com matrícula: {matriculaProfessor} não foi encontrado");
            }
            
            Turma? turmaReal = await _repositoryManager.TurmaRepository.GetTurmaAsync(codigoTurma);

            if(turmaReal is null)
            {
                _logger.LogError($"A turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"A turma com código: {codigoTurma} não foi encontrado");
            }

            _mapper.Map(turma, turmaReal, typeof(TurmaForUpdateDto), typeof(Turma));

            _repositoryManager.TurmaRepository.UpdateTurma(turmaReal);
            await _repositoryManager.SaveAsync();

            TurmaDto turmaRetornada = _mapper.Map<TurmaDto>(turmaReal);

            return turmaRetornada;
        }

        public async Task DeletarTurma(Guid codigoTurma)
        {
            Turma? turma = await _repositoryManager.TurmaRepository.GetTurmaAsync(codigoTurma);

            if(turma is null)
            {
                _logger.LogError($"A turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"A turma com código: {codigoTurma} não foi encontrado");
            }

            _repositoryManager.TurmaRepository.DeleteTurma(turma);
            await _repositoryManager.SaveAsync();
        }

        public async Task<TurmaDto> ObterTurmaPorCodigo(Guid codigoTurma, GetTurmaOptions? opcoes = null)
        {
            Turma? turma = await _repositoryManager.TurmaRepository.GetTurmaAsync(codigoTurma, opcoes);

            if(turma is null)
            {
                _logger.LogError($"A turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"A turma com código: {codigoTurma} não foi encontrado");
            }

            TurmaDto turmaRetornada = _mapper.Map<TurmaDto>(turma);

            return turmaRetornada;
        }

        public async Task<TurmaSemNotaDto> ObterTurmaPorCodigoSemNota(Guid codigoTurma, GetTurmaOptions? opcoes = null)
        {
            Turma? turma = await _repositoryManager.TurmaRepository.GetTurmaAsync(codigoTurma, opcoes);

            if (turma is null)
            {
                _logger.LogError($"A turma com código: {codigoTurma} não foi encontrado");
                throw new NotFoundException($"A turma com código: {codigoTurma} não foi encontrado");
            }

            TurmaSemNotaDto turmaRetornada = _mapper.Map<TurmaSemNotaDto>(turma);

            return turmaRetornada;
        }

        public async Task<Pagination<TurmaDto>> ObterTurmas(GetTurmasOptions opcoes)
        {
            List<Turma> turmas = await _repositoryManager.TurmaRepository.GetTurmasAsync(opcoes);

            Pagination<TurmaDto> paginacao = new Pagination<TurmaDto>()
            {
                Items = _mapper.Map<List<TurmaDto>>(turmas),
                CurrentPage = opcoes.ComecarApartirDe
            };
             
            return paginacao;
        }

        public async Task<Pagination<TurmaSemNotaDto>> ObterTurmasSemNota(GetTurmasOptions opcoes)
        {
            List<Turma> turmas = await _repositoryManager.TurmaRepository.GetTurmasAsync(opcoes);

            Pagination<TurmaSemNotaDto> paginacao = new Pagination<TurmaSemNotaDto>()
            {
                Items = _mapper.Map<List<TurmaSemNotaDto>>(turmas),
                CurrentPage = opcoes.ComecarApartirDe
            };

            return paginacao;
        }
    }
}