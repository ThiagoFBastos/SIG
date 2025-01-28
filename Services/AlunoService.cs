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
    public class AlunoService: IAlunoService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AlunoService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnderecoService _enderecoService;
        public AlunoService(IRepositoryManager repositoryManager, ILogger<AlunoService> logger, IMapper mapper, IEnderecoService enderecoService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
            _enderecoService = enderecoService;
        }

        public async Task<Guid> CadastrarAluno(AlunoForCreateDto aluno)
        {
            if(await _repositoryManager.AlunoRepository.GetAlunoPorCPFAsync(aluno.CPF) is not null)
            {
                _logger.LogError($"Já existe um aluno com cpf: {aluno.CPF}");
                throw new BadRequestException($"Já existe um aluno com cpf: {aluno.CPF}");
            }

            if(await _repositoryManager.AlunoRepository.GetAlunoPorRGAsync(aluno.RG) is not null)
            {
                _logger.LogError($"Já existe um aluno com rg: {aluno.RG}");
                throw new BadRequestException($"Já existe um aluno com rg: {aluno.RG}");
            }

            if(await _repositoryManager.AlunoRepository.GetAlunoPeloEmailAsync(aluno.Email) is not null)
            {
                _logger.LogError($"Já existe um aluno com email: {aluno.Email}");
                throw new BadRequestException($"Já existe um aluno com email: {aluno.Email}");
            }

            if(await _repositoryManager.AlunoRepository.GetAlunoPeloCelularAsync(aluno.Celular) is not null)
            {
                _logger.LogError($"Já existe um aluno com celular: {aluno.Celular}");
                throw new BadRequestException($"Já existe um aluno com celular: {aluno.Celular}");
            }

            aluno.EnderecoId = await _enderecoService.CadastrarEndereco(aluno.Endereco);

            Aluno alunoReal = _mapper.Map<Aluno>(aluno); 

            _repositoryManager.AlunoRepository.AddAluno(alunoReal);
            await _repositoryManager.SaveAsync();

            return alunoReal.Matricula;
        }
  
        public async Task<AlunoDto> AlterarAluno(Guid matriculaAluno, AlunoForUpdateDto aluno)
        {
            Aluno? alunoReal = await _repositoryManager.AlunoRepository.GetAlunoAsync(matriculaAluno);

            if(alunoReal is null)
            {
                _logger.LogError($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
                throw new NotFoundException($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
            }

            _mapper.Map(aluno, alunoReal, typeof(AlunoForUpdateDto), typeof(Aluno));

            _repositoryManager.AlunoRepository.UpdateAluno(alunoReal);
            await _repositoryManager.SaveAsync();

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(alunoReal);

            return alunoRetornado;
        }
        public async Task DeletarAluno(Guid matriculaAluno)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoAsync(matriculaAluno);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
                throw new NotFoundException($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
            }

            _repositoryManager.AlunoRepository.DeleteAluno(aluno);
            await _repositoryManager.SaveAsync();
            await _enderecoService.DeletarEndereco(aluno.EnderecoId);
        }
        public async Task<AlunoDto> ObterAlunoPorMatricula(Guid matriculaAluno, GetAlunoOptions? opcoes = null)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoAsync(matriculaAluno, opcoes);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
                throw new NotFoundException($"Aluno com matrícula: {matriculaAluno} não foi encontrado");
            }

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(aluno);

            return alunoRetornado;
        }
        public async Task<AlunoDto> ObterAlunoPorCPF(string cpf, GetAlunoOptions? opcoes = null)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoPorCPFAsync(cpf, opcoes);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com cpf: {cpf} não foi encontrado");
                throw new NotFoundException($"Aluno com cpf: {cpf} não foi encontrado");
            }

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(aluno);

            return alunoRetornado;
        }

        public async Task<AlunoDto> ObterAlunoPorRG(string rg, GetAlunoOptions? opcoes = null)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoPorRGAsync(rg, opcoes);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com rg: {rg} não foi encontrado");
                throw new NotFoundException($"Aluno com rg: {rg} não foi encontrado");
            }

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(aluno);

            return alunoRetornado;
        }

        public async Task<AlunoDto> ObterAlunoPeloEmail(string email, GetAlunoOptions? opcoes = null)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoPeloEmailAsync(email, opcoes);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com email: {email} não foi encontrado");
                throw new NotFoundException($"Aluno com email: {email} não foi encontrado");
            }

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(aluno);

            return alunoRetornado;
        }
        public async Task<AlunoDto> ObterAlunoPeloCelular(string celular, GetAlunoOptions? opcoes = null)
        {
            Aluno? aluno = await _repositoryManager.AlunoRepository.GetAlunoPeloCelularAsync(celular, opcoes);

            if(aluno is null)
            {
                _logger.LogError($"Aluno com celular: {celular} não foi encontrado");
                throw new NotFoundException($"Aluno com celular: {celular} não foi encontrado");
            }

            AlunoDto alunoRetornado = _mapper.Map<AlunoDto>(aluno);

            return alunoRetornado;
        }

        public async Task<Pagination<AlunoDto>> ObterAlunos(GetAlunosOptions opcoes)
        {
            List<Aluno> alunos = await _repositoryManager.AlunoRepository.GetAlunosAsync(opcoes);

            Pagination<AlunoDto> pagination = new Pagination<AlunoDto>
            {
                Items = _mapper.Map<List<AlunoDto>>(alunos),
                CurrentPage = opcoes.ComecarApartirDe
            };

            return pagination;
        }
    }
}