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
    /// Controlador respons�vel pelas opera��es com endere�os
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
        /// <param name="enderecoService">Servi�o respons�vel pelos casos de uso com os endere�os</param>
        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        /// <summary>
        /// Altera um endere�o
        /// </summary>
        /// <param name="id">Identificador do endere�o</param>
        /// <param name="endereco">Par�metros necess�rios para alterar um endere�o</param>
        /// <response code="200">Endere�o atualizado com sucesso</response>
        /// <response code="400">Par�metros inv�lidos passados</response>
        /// <returns>Um endere�o atualizado</returns>
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
        /// Obt�m um endere�o dado seu identificador
        /// </summary>
        /// <param name="id">Identificador do endere�o</param>
        /// <response code="200">Endere�o requisitado com sucesso</response>
        /// <response code="404">Endere�o n�o encontrado</response>
        /// <returns>Endere�o requisitado</returns>
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