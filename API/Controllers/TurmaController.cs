using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações com turmas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _turmaService;
        private readonly ILogger<TurmaController> _logger;

        /// <summary>
        /// Construtor do controlador TurmaController
        /// </summary>
        /// <param name="turmaService">Serviço responsável pelos casos de uso com turmas</param>
        /// <param name="logger">Logger para o controlador</param>
        public TurmaController(ITurmaService turmaService, ILogger<TurmaController> logger)
        { 
            _turmaService = turmaService;
            _logger = logger;
        }

        /// <summary>
        /// Registra uma nova turma
        /// </summary>
        /// <param name="turma">Parâmetros necessários para a criação de uma turma</param>
        /// <response code="200">Turma cadastrada com sucesso</response>
        /// <response code="400">Parâmetros inválidos passados</response>
        /// <returns>Código da turma</returns>
        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] TurmaForCreateDto turma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(turma);

            Guid codigo = await _turmaService.CadastrarTurma(turma);

            return Ok(new GuidResponseDto(codigo));
        }

        /// <summary>
        /// Atualiza os dados de uma turma
        /// </summary>
        /// <param name="codigo">Código da turma que se deseja atualizar os dados</param>
        /// <param name="turma">Parâmetros necessários para se atualizar uma turma</param>
        /// <response code="200">Turma atualizada com sucesso</response>
        /// <response code="400">Parâmetros inválidos passados</response>
        /// <response code="404">Turma não encontrada</response>
        /// <returns>Dados da turma</returns>
        [HttpPut("{codigo}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(TurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid codigo, [FromBody] TurmaForUpdateDto turma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(turma);

            TurmaDto turmaDto = await _turmaService.AlterarTurma(codigo, turma);

            return Ok(turmaDto);
        }

        /// <summary>
        /// Exclui a turma indicada do sistema
        /// </summary>
        /// <param name="codigo">Código da turma</param>
        /// <response code="204">Turma excluída com sucesso</response>
        /// <response code="404">Turma não encontrada</response>
        /// <returns>Nenhum conteúdo</returns>
        [HttpDelete("{codigo}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid codigo)
        {
            await _turmaService.DeletarTurma(codigo);
            return NoContent();
        }

        /// <summary>
        /// Recupera os dados de uma turma
        /// </summary>
        /// <param name="codigo">Código da turma</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados da turma requisitada</response>
        /// <response code="404">Turma não encontrada</response>
        /// <returns>Os dados da turma</returns>
        [HttpGet("{codigo}")]
        [Authorize(Roles = "admin,administrativo,professor")]
        [ProducesResponseType(typeof(TurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid codigo, [FromQuery] GetTurmaOptions? opcoes = null)
        {
            TurmaDto turma = await _turmaService.ObterTurmaPorCodigo(codigo, opcoes);
 
            return Ok(turma);
        }

        /// <summary>
        /// Recupera os dados de uma turma (sem incluir as notas dos alunos)
        /// </summary>
        /// <param name="codigo">Código da turma</param>
        /// <param name="opcoes">Parâmetros adicionais de requisição</param>
        /// <response code="200">Os dados da turma requisitada</response>
        /// <response code="404">Turma não encontrada</response>
        /// <returns>Os dados da turma</returns>
        [HttpGet("sensitive/{codigo}")]
        [Authorize(Roles = "aluno")]
        [ProducesResponseType(typeof(TurmaSemNotaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWithoutGrade([FromRoute] Guid codigo, [FromQuery] GetTurmaOptions? opcoes = null)
        {
            TurmaSemNotaDto turma = await _turmaService.ObterTurmaPorCodigoSemNota(codigo, opcoes);

            return Ok(turma);
        }

        /// <summary>
        /// Filtra as turmas de acordo com os parâmetros informados durante a requisição
        /// </summary>
        /// <param name="opcoes">Parâmetros de filtragem</param>
        /// <response code="200">Turmas filtradas</response>
        /// <response code="204">Nenhuma turma foi encontrada</response>
        /// <returns>Lista paginada com as turmas filtradas</returns>
        [HttpGet("filter")]
        [Authorize(Roles = "admin,administrativo,professor")]
        [ProducesResponseType(typeof(Pagination<TurmaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Filter([FromQuery] GetTurmasOptions opcoes)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(opcoes);

            Pagination<TurmaDto> pagination = await _turmaService.ObterTurmas(opcoes);

            if(!pagination.Items.Any())
                return NoContent();

            return Ok(pagination);
        }

        /// <summary>
        /// Filtra as turmas de acordo com os parâmetros informados durante a requisição (sem incluir as notas dos alunos)
        /// </summary>
        /// <param name="opcoes">Parâmetros de filtragem</param>
        /// <response code="200">Turmas filtradas</response>
        /// <response code="204">Nenhuma turma foi encontrada</response>
        /// <returns>Lista paginada com as turmas filtradas</returns>
        [HttpGet("sensitive/filter")]
        [Authorize(Roles = "aluno")]
        [ProducesResponseType(typeof(Pagination<TurmaSemNotaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> FilterWithoutGrade([FromQuery] GetTurmasOptions opcoes)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(opcoes);

            Pagination<TurmaSemNotaDto> pagination = await _turmaService.ObterTurmasSemNota(opcoes);

            if (!pagination.Items.Any())
                return NoContent();

            return Ok(pagination);
        }
    }
}