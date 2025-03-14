using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações com professores
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        /// <summary>
        /// Construtor da classe ProfessorController
        /// </summary>
        /// <param name="professorService">Serviço responsável pelos casos de uso envolvendo professores</param>
        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        }

        /// <summary>
        /// Cadastra um novo professor
        /// </summary>
        /// <param name="professor">Parâmetros necessários para se registrar um novo professor</param>
        /// <response code="200">Professor registrado com sucesso</response>
        /// <response code="400">Parâmetros inválidos passados</response>
        /// <returns>A matrícula do professor</returns>

        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] ProfessorForCreateDto professor)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(professor);

            Guid matricula = await _professorService.CadastrarProfessor(professor);

            return Ok(new GuidResponseDto(matricula));
        }

        /// <summary>
        /// Atualiza as informações de um professor
        /// </summary>
        /// <param name="matricula">Matrícula do professor</param>
        /// <param name="professor">Parâmetros necessários para se atualizar as informações de um professor</param>
        /// <response code="200">Dados do professor atualizados com sucesso</response>
        /// <response code="400">Parâmetros inválidos passados</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpPut("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid matricula, [FromBody] ProfessorForUpdateDto professor)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(professor);

            ProfessorDto professorDto = await _professorService.AlterarProfessor(matricula, professor);

            return Ok(professorDto);
        }

        /// <summary>
        /// Exclui um professor do sistema
        /// </summary>
        /// <param name="matricula">Matrícula do professor</param>
        /// <response code="204">Professor excluído com sucesso</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Nenhum conteúdo</returns>
        [HttpDelete("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid matricula)
        {
            await _professorService.DeletarProfessor(matricula);
            return NoContent();
        }

        /// <summary>
        /// Recupera as informações de um professor pela matrícula
        /// </summary>
        /// <param name="matricula">Matrícula do professor</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados do professor</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpGet("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorMatricula(matricula, opcoes);
            return Ok(professor); 
        }

        /// <summary>
        /// Recupera as informações de um professor pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do professor</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados do professor</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpGet("by/cpf/{cpf}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorCPF(cpf, opcoes);
            return Ok(professor); 
        }

        /// <summary>
        /// Recupera as informações de um professor pelo RG
        /// </summary>
        /// <param name="rg">RG do professor</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados do professor</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpGet("by/rg/{rg}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorRG(rg, opcoes);
            return Ok(professor); 
        }

        /// <summary>
        /// Recupera as informações de um professor pelo Email
        /// </summary>
        /// <param name="email">Email do professor</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados do professor</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPeloEmail(email, opcoes);
            return Ok(professor); 
        }

        /// <summary>
        /// Recupera as informações de um professor pelo celular
        /// </summary>
        /// <param name="celular">Celular do professor</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados do professor</response>
        /// <response code="404">Professor não encontrado</response>
        /// <returns>Os dados do professor</returns>
        [HttpGet("by/celular/{celular}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(ProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPeloCelular(celular, opcoes);
            return Ok(professor); 
        }

        /// <summary>
        /// Filtra resultados de professores
        /// </summary>
        /// <param name="opcoes">Parâmetros de filtragem</param>
        /// <response code="200">Professores retornados com sucesso</response>
        /// <response code="204">Nenhum professor encontrado</response>
        /// <returns>Lista paginada com os professores que respeitam a filtragem</returns>
        [HttpGet("filter")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(Pagination<ProfessorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter([FromQuery] GetProfessoresOptions opcoes)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(opcoes);

            Pagination<ProfessorDto> pagination = await _professorService.ObterProfessores(opcoes);

            if(!pagination.Items.Any())
                return NoContent();

            return Ok(pagination);
        }
    }
}