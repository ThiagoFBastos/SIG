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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] EnderecoForUpdateDto endereco)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(endereco);

            EnderecoDto enderecoDto = await _enderecoService.AtualizarEndereco(id, endereco);

            return Ok(enderecoDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            EnderecoDto endereco = await _enderecoService.ObterEnderecoPorId(id);
            return Ok(endereco);
        }
    }
}