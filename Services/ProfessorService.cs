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
    public class ProfessorService: IProfessorService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<ProfessorService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnderecoService _enderecoService;
        public ProfessorService(IRepositoryManager repositoryManager, ILogger<ProfessorService> logger, IMapper mapper, IEnderecoService enderecoService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
            _enderecoService = enderecoService;
        } 

        public async Task<Guid> CadastrarProfessor(ProfessorForCreateDto professor)
        {
            if(await _repositoryManager.ProfessorRepository.GetProfessorPorCPFAsync(professor.CPF) is not null)
            {
                _logger.LogError($"Já existe um professor com cpf: {professor.CPF}");
                throw new BadRequestException($"Já existe um professor com cpf: {professor.CPF}");
            }

            if(await _repositoryManager.ProfessorRepository.GetProfessorPorRGAsync(professor.RG) is not null)
            {
                _logger.LogError($"Já existe um professor com rg: {professor.RG}");
                throw new BadRequestException($"Já existe um professor com rg: {professor.RG}");
            }

            if(await _repositoryManager.ProfessorRepository.GetProfessorPeloEmailAsync(professor.Email) is not null)
            {
                _logger.LogError($"Já existe um professor com email: {professor.Email}");
                throw new BadRequestException($"Já existe um professor com email: {professor.Email}");
            }

            if(await _repositoryManager.ProfessorRepository.GetProfessorPeloCelularAsync(professor.Celular) is not null)
            {
                _logger.LogError($"Já existe um professor com celular: {professor.Celular}");
                throw new BadRequestException($"Já existe um professor com celular: {professor.Celular}");
            }

            professor.EnderecoId = await _enderecoService.CadastrarEndereco(professor.Endereco);
            Professor professorReal = _mapper.Map<Professor>(professor);

            _repositoryManager.ProfessorRepository.AddProfessor(professorReal);
            await _repositoryManager.SaveAsync();

            return professorReal.Matricula;
        }

        public async Task<ProfessorDto> AlterarProfessor(Guid matriculaProfessor, ProfessorForUpdateDto professor)
        {
            Professor? professorReal = await _repositoryManager.ProfessorRepository.GetProfessorAsync(matriculaProfessor);

            if(professorReal is null)
            {
                _logger.LogError($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
                throw new NotFoundException($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
            }

            _mapper.Map(professor, professorReal, typeof(ProfessorForUpdateDto), typeof(Professor));
            _repositoryManager.ProfessorRepository.UpdateProfessor(professorReal);
            await _repositoryManager.SaveAsync();

            ProfessorDto profesorRetornado = _mapper.Map<ProfessorDto>(professorReal);

            return profesorRetornado;
        }

        public async Task DeletarProfessor(Guid matriculaProfessor)
        {
            Professor? professorReal = await _repositoryManager.ProfessorRepository.GetProfessorAsync(matriculaProfessor);

            if(professorReal is null)
            {
                _logger.LogError($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
                throw new NotFoundException($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
            }

            _repositoryManager.ProfessorRepository.DeleteProfessor(professorReal);
            await _repositoryManager.SaveAsync();
            await _enderecoService.DeletarEndereco(professorReal.EnderecoId);
        }

        public async Task<ProfessorDto> ObterProfessorPorMatricula(Guid matriculaProfessor, GetProfessorOptions? opcoes = null)
        {
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorAsync(matriculaProfessor, opcoes);

            if(professor is null)
            {
                _logger.LogError($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
                throw new NotFoundException($"Professor com matrícula: {matriculaProfessor} não foi encontrado");
            }

            ProfessorDto professorRetornado = _mapper.Map<ProfessorDto>(professor);

            return professorRetornado;
        }

        public async Task<ProfessorDto> ObterProfessorPorCPF(string cpf, GetProfessorOptions? opcoes = null)
        {
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorPorCPFAsync(cpf, opcoes);

            if(professor is null)
            {
                _logger.LogError($"Professor com cpf: {cpf} não foi encontrado");
                throw new NotFoundException($"Professor com cpf: {cpf} não foi encontrado");
            }

            ProfessorDto professorRetornado = _mapper.Map<ProfessorDto>(professor);

            return professorRetornado;
        }

        public async Task<ProfessorDto> ObterProfessorPorRG(string rg, GetProfessorOptions? opcoes = null)
        {
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorPorRGAsync(rg, opcoes);

            if(professor is null)
            {
                _logger.LogError($"Professor com rg: {rg} não foi encontrado");
                throw new NotFoundException($"Professor com rg: {rg} não foi encontrado");
            }

            ProfessorDto professorRetornado = _mapper.Map<ProfessorDto>(professor);

            return professorRetornado;
        }
 
        public async Task<ProfessorDto> ObterProfessorPeloEmail(string email, GetProfessorOptions? opcoes = null)
        {
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorPeloEmailAsync(email, opcoes);

            if(professor is null)
            {
                _logger.LogError($"Professor com email: {email} não foi encontrado");
                throw new NotFoundException($"Professor com email: {email} não foi encontrado");
            }

            ProfessorDto professorRetornado = _mapper.Map<ProfessorDto>(professor);

            return professorRetornado;
        }

        public async Task<ProfessorDto> ObterProfessorPeloCelular(string celular, GetProfessorOptions? opcoes = null)
        {
            Professor? professor = await _repositoryManager.ProfessorRepository.GetProfessorPeloCelularAsync(celular, opcoes);

            if(professor is null)
            {
                _logger.LogError($"Professor com celular: {celular} não foi encontrado");
                throw new NotFoundException($"Professor com celular: {celular} não foi encontrado");
            }

            ProfessorDto professorRetornado = _mapper.Map<ProfessorDto>(professor);

            return professorRetornado;
        }

        public async Task<Pagination<ProfessorDto>> ObterProfessores(GetProfessoresOptions opcoes)
        {
            List<Professor> professores = await _repositoryManager.ProfessorRepository.GetProfessoresAsync(opcoes);

            Pagination<ProfessorDto> pagination = new Pagination<ProfessorDto>
            {
                Items = _mapper.Map<List<ProfessorDto>>(professores),
                CurrentPage = opcoes.ComecarApartirDe
            };

            return pagination;
        }
    }
}