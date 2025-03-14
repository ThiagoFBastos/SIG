using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações com endereços
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,administrativo")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        /// <summary>
        /// Construtor do EnderecoController
        /// </summary>
        /// <param name="enderecoService">Serviço responsável pelos casos de uso com os endereços</param>
        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        /// <summary>
        /// Altera um endereço
        /// </summary>
        /// <param name="id">Identificador do endereço</param>
        /// <param name="endereco">Parâmetros necessários para alterar um endereço</param>
        /// <response code="200">Endereço atualizado com sucesso</response>
        /// <response code="400">Parâmetros inválidos passados</response>
        /// <returns>Um endereço atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EnderecoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EnderecoForUpdateDto endereco)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(endereco);

            EnderecoDto enderecoDto = await _enderecoService.AtualizarEndereco(id, endereco);

            return Ok(enderecoDto);
        }

        /// <summary>
        /// Obtém um endereço dado seu identificador
        /// </summary>
        /// <param name="id">Identificador do endereço</param>
        /// <response code="200">Endereço requisitado com sucesso</response>
        /// <response code="404">Endereço não encontrado</response>
        /// <returns>Endereço requisitado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnderecoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            EnderecoDto endereco = await _enderecoService.ObterEnderecoPorId(id);
            return Ok(endereco);
        }
    }
}