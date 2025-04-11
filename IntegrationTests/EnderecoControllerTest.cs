using API.Controllers;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Persistence.Repositories;
using Services;
using Services.Contracts;
using Services.Mappers;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class EnderecoControllerTest: IClassFixture<DataFixture>, IAsyncLifetime
    {
        private readonly EnderecoController _controller;
        private readonly RepositoryManager _repository;
        private IDbContextTransaction? _transaction;

        public EnderecoControllerTest(DataFixture fixture)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<EnderecoProfile>());
            var mapper = config.CreateMapper();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EnderecoService>();

           IEnderecoService enderecoService = new EnderecoService(fixture.RepositoryManager, logger, mapper);

           _controller = new EnderecoController(enderecoService);
           _repository = fixture.RepositoryManager;
        }

        public async Task InitializeAsync()
        {
            _transaction = await _repository.BeginTransactionAsync();
        }

        public async Task DisposeAsync()
        {
            if(_transaction is not null)
                await _transaction.RollbackAsync();
        }

        [Fact]
        public async Task Test_Get_Endereco_Must_Work()
        {
            Endereco endereco = new Endereco
            {
                Cidade = "Rio de Janeiro",
                Estado = Estado.RJ,
                Rua = "Rua Sete de Setembro",
                CEP = "21100412",
                Casa = 10,
                Complemento = "Fundos casa 3"
            };

            _repository.EnderecoRepository.AddEndereco(endereco);
            await _repository.SaveAsync();

            var response = await _controller.Get(endereco.Id);

            response.Should().NotBeNull().And.BeOfType<OkObjectResult>().Which.Value.Should().Be(
                new EnderecoDto
                {
                    Cidade = "Rio de Janeiro",
                    Estado = (int)Estado.RJ,
                    Rua = "Rua Sete de Setembro",
                    CEP = "21100412",
                    Casa = 10,
                    Complemento = "Fundos casa 3",
                    Id = endereco.Id
                }
            );
        }

        [Fact]
        public async Task Test_Get_Endereco_Must_Not_Find()
        {
            Guid enderecoId = Guid.NewGuid();

            try
            {
                _ = await _controller.Get(enderecoId);
            }
            catch(Exception ex)
            {
                ex.Should().BeOfType<NotFoundException>().Which.Message.Should().Be($"O endereço com id: {enderecoId} não foi encontrado");
            }
        }

        [Fact]
        public async Task Test_Update_Endereco_Must_Work()
        {
            Endereco endereco = new Endereco
            {
                Cidade = "Rio de Janeiro",
                Estado = Estado.RJ,
                Rua = "Rua Sete de Setembro",
                CEP = "21100412",
                Casa = 10,
                Complemento = "Fundos casa 3"
            };

            EnderecoForUpdateDto enderecoForUpdate = new EnderecoForUpdateDto
            {
                Cidade = "São Paulo",
                Estado = (int)Estado.SP,
                Rua = "Rua Vila do Chaves",
                CEP = "11100412",
                Casa = 71
            };

            _repository.EnderecoRepository.AddEndereco(endereco);
            await _repository.SaveAsync();

            var response = await _controller.Update(endereco.Id, enderecoForUpdate);

            response.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(
                new EnderecoDto
                {
                    Cidade = "São Paulo",
                    Estado = (int)Estado.SP,
                    Rua = "Rua Vila do Chaves",
                    CEP = "11100412",
                    Casa = 71,
                    Id = endereco.Id
                }
             );
        }

        [Fact]
        public async Task Test_Update_Endereco_Must_Not_Find()
        {
            Guid enderecoId = Guid.NewGuid();

            EnderecoForUpdateDto enderecoForUpdate = new EnderecoForUpdateDto
            {
                Cidade = "São Paulo",
                Estado = (int)Estado.SP,
                Rua = "Rua Vila do Chaves",
                CEP = "11100412",
                Casa = 71
            };

            try
            {
                _ = await _controller.Update(enderecoId, enderecoForUpdate);
            }
            catch(Exception ex)
            {
                ex.Should().BeOfType<NotFoundException>().Which.Message.Should().Be($"O endereço com id: {enderecoId} não foi encontrado");
            }
        }
    }
}
