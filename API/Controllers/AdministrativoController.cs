using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdministrativoController : ControllerBase
    {
        private readonly IAdministrativoService _administrativoService;
 
        public AdministrativoController(IAdministrativoService administrativoService)
        {
            _administrativoService = administrativoService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AdministrativoForCreateDto administrativo)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(administrativo);

            Guid matricula = await _administrativoService.CadastrarAdmnistrativo(administrativo);
            return Ok(new GuidResponseDto(matricula));
        }

        [HttpGet("{matricula}")] 
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorMatricula(matricula, opcoes);
            return Ok(administrativo);
        }

        [HttpGet("by/cpf/{cpf}")]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorCPF(cpf, opcoes);
            return Ok(administrativo);
        }

        [HttpGet("by/rg/{rg}")]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorRG(rg, opcoes);
            return Ok(administrativo);
        }

        [HttpGet("by/celular/{celular}")]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPeloCelular(celular, opcoes);
            return Ok(administrativo);
        }

        [HttpGet("by/email/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPeloEmail(email, opcoes);
            return Ok(administrativo);
        }

        [HttpPut("{matricula}")]
        public async Task<IActionResult> Update(Guid matricula, AdministrativoForUpdateDto administrativo)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(administrativo);

            AdministrativoDto administrativoDto = await _administrativoService.AlterarAdministrativo(matricula, administrativo);
            return Ok(administrativoDto);
        }

        [HttpDelete("{matricula}")]
        public async Task<IActionResult> Delete(Guid matricula)
        {
            await _administrativoService.DeletarAdministrativo(matricula);
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] GetAdministrativosOptions opcoes)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(opcoes);
                
            Pagination<AdministrativoDto> paginacao = await _administrativoService.ObterAdministrativos(opcoes);

            if(!paginacao.Items.Any())
                return NoContent();

            return Ok(paginacao);
        }
    } 
}