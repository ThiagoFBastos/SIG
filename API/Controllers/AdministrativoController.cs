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
    /// <summary>
    /// Controlador responsável por efetuar as operações relacionadas ao administrativo
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,administrativo")]
    public class AdministrativoController : ControllerBase
    {
        private readonly IAdministrativoService _administrativoService;
 
        /// <summary>
        /// Construtor do AdministrativoController
        /// </summary>
        /// <param name="administrativoService">O serviço responsável pelos casos de uso dos administrativos</param>
        public AdministrativoController(IAdministrativoService administrativoService)
        {
            _administrativoService = administrativoService;
        }

        /// <summary>
        /// Adiciona um novo administrativo
        /// </summary>
        /// <param name="administrativo">entidade administrativo a ser cadastrada</param>
        /// <response code="200">Ok administrativo cadastrado</response>
        /// <response code="400">Dados inválidos</response>
        /// <returns>Matrícula do administrativo</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AdministrativoForCreateDto administrativo)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(administrativo);

            Guid matricula = await _administrativoService.CadastrarAdmnistrativo(administrativo);
            return Ok(new GuidResponseDto(matricula));
        }

        /// <summary>
        /// Obtém um administrativo pela matrícula
        /// </summary>
        /// <param name="matricula">Matrícula do administrativo</param>
        /// <param name="opcoes">Parâmetros adicionais para requisição</param>
        /// <response code="200">O administrativo com a matrícula requisitada</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpGet("{matricula}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorMatricula(matricula, opcoes);
            return Ok(administrativo);
        }

        /// <summary>
        /// Obtém um administrativo pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do administrativo</param>
        /// <param name="opcoes">Parâmetros adicionais para requisição</param>
        /// <response code="200">O administrativo com o CPF requisitado</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpGet("by/cpf/{cpf}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorCPF(cpf, opcoes);
            return Ok(administrativo);
        }

        /// <summary>
        /// Obtém um administrativo pelo RG
        /// </summary>
        /// <param name="rg">RG do administrativo</param>
        /// <param name="opcoes">Parâmetros adicionais para requisição</param>
        /// <response code="200">O administrativo com o RG requisitado</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpGet("by/rg/{rg}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPorRG(rg, opcoes);
            return Ok(administrativo);
        }

        /// <summary>
        /// Obtém um administrativo pela matrícula
        /// </summary>
        /// <param name="celular">Celular do administrativo</param>
        /// <param name="opcoes">Parâmetros adicionais para requisição</param>
        /// <response code="200">O administrativo com o celular requisitado</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpGet("by/celular/{celular}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPeloCelular(celular, opcoes);
            return Ok(administrativo);
        }

        /// <summary>
        /// Obtém um administrativo pelo email
        /// </summary>
        /// <param name="email">Email do administrativo</param>
        /// <param name="opcoes">Parâmetros adicionais para requisição</param>
        /// <response code="200">O administrativo com o celular requisitado</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpGet("by/email/{email}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetAdministrativoOptions? opcoes = null)
        {
            AdministrativoDto administrativo = await _administrativoService.ObterAdministrativoPeloEmail(email, opcoes);
            return Ok(administrativo);
        }

        /// <summary>
        /// Altera as informações de um administrativo
        /// </summary>
        /// <param name="matricula">Matrícula do administrativo</param>
        /// <param name="administrativo">Dados do administrativo para se atualizar</param>
        /// <response code="200">O administrativo com a matrícula indicada e com os dados atualizados</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Um administrativo</returns>
        [HttpPut("{matricula}")]
        [ProducesResponseType(typeof(AdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid matricula, AdministrativoForUpdateDto administrativo)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(administrativo);

            AdministrativoDto administrativoDto = await _administrativoService.AlterarAdministrativo(matricula, administrativo);
            return Ok(administrativoDto);
        }

        /// <summary>
        /// Exclui um administrativo
        /// </summary>
        /// <param name="matricula">Matrícula do administrativo que se deseja excluir</param>
        /// <response code="204">Administrativo excluído</response>
        /// <response code="404">Administrativo não encontrado</response>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{matricula}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid matricula)
        {
            await _administrativoService.DeletarAdministrativo(matricula);
            return NoContent();
        }

        /// <summary>
        /// Filtra os administrativos com base nos parâmetros passados
        /// </summary>
        /// <param name="opcoes">Parâmetros para a filtragem</param>
        /// <returns>Uma lista paginada de administrativos</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(Pagination<AdministrativoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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