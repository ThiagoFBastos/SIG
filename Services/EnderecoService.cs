using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Repositories;
using Services.Contracts;
using Shared.Dtos;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Services
{
    public class EnderecoService: IEnderecoService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger<EnderecoService> _logger;
        private readonly IMapper _mapper;
        
        public EnderecoService(IRepositoryManager repositoryManager, ILogger<EnderecoService> logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Guid> CadastrarEndereco(EnderecoForCreateDto endereco)
        {
            Endereco enderecoReal = _mapper.Map<Endereco>(endereco);
            _repositoryManager.EnderecoRepository.AddEndereco(enderecoReal);
            await _repositoryManager.SaveAsync();

            return enderecoReal.Id;
        }

        public async Task<EnderecoDto> AtualizarEndereco(Guid idEndereco, EnderecoForUpdateDto endereco)
        {
            Endereco? enderecoReal = await _repositoryManager.EnderecoRepository.GetEnderecoAsync(idEndereco);

            if(enderecoReal is null)
            {
                _logger.LogError($"O endereço com id: {idEndereco} não foi encontrado");
                throw new NotFoundException($"O endereço com id: {idEndereco} não foi encontrado");
            }

            _mapper.Map(endereco, enderecoReal, typeof(EnderecoForUpdateDto), typeof(Endereco));
            _repositoryManager.EnderecoRepository.UpdateEndereco(enderecoReal);
            await _repositoryManager.SaveAsync();

            EnderecoDto enderecoRetornado = _mapper.Map<EnderecoDto>(enderecoReal);

            return enderecoRetornado;
        }

        public async Task DeletarEndereco(Guid idEndereco)
        {
            Endereco? endereco = await _repositoryManager.EnderecoRepository.GetEnderecoAsync(idEndereco);

            if(endereco is null)
            {
                _logger.LogError($"O endereço com id: {idEndereco} não foi encontrado");
                throw new NotFoundException($"O endereço com id: {idEndereco} não foi encontrado");
            }

            _repositoryManager.EnderecoRepository.DeleteEndereco(endereco);
            await _repositoryManager.SaveAsync();
        }

        public async Task<EnderecoDto> ObterEnderecoPorId(Guid idEndereco)
        {
            Endereco? endereco = await _repositoryManager.EnderecoRepository.GetEnderecoAsync(idEndereco);

            if(endereco is null)
            {
                _logger.LogError($"O endereço com id: {idEndereco} não foi encontrado");
                throw new NotFoundException($"O endereço com id: {idEndereco} não foi encontrado");
            }

            EnderecoDto enderecoRetornado = _mapper.Map<EnderecoDto>(endereco);

            return enderecoRetornado;
        }
    }
}