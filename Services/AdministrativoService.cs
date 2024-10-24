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
    public class AdministrativoService: IAdministrativoService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<AdministrativoService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnderecoService _enderecoService;

        public AdministrativoService(IRepositoryManager repositoryManager, ILogger<AdministrativoService> logger, IMapper mapper, IEnderecoService enderecoService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
            _enderecoService = enderecoService;
        }

        public async Task<Guid> CadastrarAdmnistrativo(AdministrativoForCreateDto administrativo)
        {
            if(await _repositoryManager.AdministrativoRepository.GetAdministrativoPorCPFAsync(administrativo.CPF) is not null)
            {
                _logger.LogError($"Já existe um administrativo com o cpf: {administrativo.CPF}");
                throw new BadRequestException($"Já existe um administrativo com o cpf: {administrativo.CPF}");
            }

            if(await _repositoryManager.AdministrativoRepository.GetAdministrativoPorRGAsync(administrativo.RG) is not null)
            {
                _logger.LogError($"Já existe um administrativo com o rg: {administrativo.RG}");
                throw new BadRequestException($"Já existe um administrativo com o rg: {administrativo.RG}");    
            }

            if(await _repositoryManager.AdministrativoRepository.GetAdministrativoPeloEmailAsync(administrativo.Email) is not null)
            {
                _logger.LogError($"Já existe um administrativo com o email: {administrativo.Email}");
                throw new BadRequestException($"Já existe um administrativo com o email: {administrativo.Email}");    
            }

            if(await _repositoryManager.AdministrativoRepository.GetAdministrativoPeloCelularAsync(administrativo.Celular) is not null)
            {
                _logger.LogError($"Já existe um administrativo com o celular: {administrativo.Celular}");
                throw new BadRequestException($"Já existe um administrativo com o celular: {administrativo.Celular}");    
            }

            administrativo.EnderecoId = await _enderecoService.CadastrarEndereco(administrativo.Endereco); 

            Administrativo administrativoReal = _mapper.Map<Administrativo>(administrativo);
            _repositoryManager.AdministrativoRepository.AddAdministrativo(administrativoReal);
            await _repositoryManager.SaveAsync();

            return administrativoReal.Matricula;
        }

        public async Task<AdministrativoDto> AlterarAdministrativo(Guid administrativoMatricula, AdministrativoForUpdateDto administrativo)
        {
            Administrativo? administrativoReal = await _repositoryManager.AdministrativoRepository.GetAdministrativoAsync(administrativoMatricula);

            if(administrativoReal is null)
            {
                _logger.LogError($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
                throw new NotFoundException($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
            }

            _mapper.Map(administrativo, administrativoReal, typeof(AdministrativoForUpdateDto), typeof(Administrativo));
            _repositoryManager.AdministrativoRepository.UpdateAdministrativo(administrativoReal);
            await _repositoryManager.SaveAsync();

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativoReal);

            return administrativoRetornado;
        }

        public async Task DeletarAdministrativo(Guid administrativoMatricula)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoAsync(administrativoMatricula);

            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
                throw new NotFoundException($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
            }

            _repositoryManager.AdministrativoRepository.DeleteAdministrativo(administrativo);
            await _repositoryManager.SaveAsync();
        }

        public async Task<AdministrativoDto> ObterAdministrativoPorMatricula(Guid administrativoMatricula, GetAdministrativoOptions? opcoes = null)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoAsync(administrativoMatricula, opcoes);
 
            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
                throw new NotFoundException($"O administrativo com matrícula: {administrativoMatricula} não foi encontrado");
            }

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativo);

            return administrativoRetornado;
        }

        public async Task<AdministrativoDto> ObterAdministrativoPorCPF(string cpf, GetAdministrativoOptions? opcoes = null)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoPorCPFAsync(cpf, opcoes);

            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com cpf: {cpf} não foi encontrado");
                throw new NotFoundException($"O administrativo com cpf: {cpf} não foi encontrado");
            }

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativo);

            return administrativoRetornado;
        }

        public async Task<AdministrativoDto> ObterAdministrativoPorRG(string rg, GetAdministrativoOptions? opcoes = null)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoPorRGAsync(rg, opcoes);

            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com rg: {rg} não foi encontrado");
                throw new NotFoundException($"O administrativo com rg: {rg} não foi encontrado");
            }

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativo);

            return administrativoRetornado;
        }

        public async Task<AdministrativoDto> ObterAdministrativoPeloEmail(string email, GetAdministrativoOptions? opcoes = null)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoPeloEmailAsync(email, opcoes);

            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com email: {email} não foi encontrado");
                throw new NotFoundException($"O administrativo com email: {email} não foi encontrado");
            }

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativo);

            return administrativoRetornado;
        }

        public async Task<AdministrativoDto> ObterAdministrativoPeloCelular(string celular, GetAdministrativoOptions? opcoes = null)
        {
            Administrativo? administrativo = await _repositoryManager.AdministrativoRepository.GetAdministrativoPeloCelularAsync(celular, opcoes);

            if(administrativo is null)
            {
                _logger.LogError($"O administrativo com celular: {celular} não foi encontrado");
                throw new NotFoundException($"O administrativo com celular: {celular} não foi encontrado");
            }

            AdministrativoDto administrativoRetornado = _mapper.Map<AdministrativoDto>(administrativo);

            return administrativoRetornado;
        }

        public async Task<Pagination<AdministrativoDto>> ObterAdministrativos(GetAdministrativosOptions opcoes)
        {
            List<Administrativo> administrativos = await _repositoryManager.AdministrativoRepository.GetAdministrativosAsync(opcoes);

            Pagination<AdministrativoDto> pagination = new Pagination<AdministrativoDto>
            {
                Items = _mapper.Map<List<AdministrativoDto>>(administrativos),
                CurrentPage = opcoes.ComecarApartirDe
            };

            return pagination;
        }
    }
}