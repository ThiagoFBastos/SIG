using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Controlador para o entidade que relaciona aluno e turma
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AlunoTurmaController : ControllerBase
    {
        private readonly IAlunoTurmaService _alunoTurmaService;

        /// <summary>
        /// Construtor do controller AlunoTurmaController
        /// </summary>
        /// <param name="alunoTurmaService">Servi�o respons�vel pelos casos de uso da entidade AlunoTurma</param>
        public AlunoTurmaController(IAlunoTurmaService alunoTurmaService)
        {
            _alunoTurmaService = alunoTurmaService;
        }

        /// <summary>
        /// Cria uma novo relacionamento entre aluno e turma
        /// </summary>
        /// <param name="alunoTurma">Dados usados para a cria��o do relacionamento entre aluno e turma</param>
        /// <response code="200">Relacionamento entre aluno e turma criado com sucesso</response>
        /// <response code="400">Algum par�metro passado � inv�lido</response>
        /// <returns>C�digo da entidade AlunoTurma</returns>
        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AlunoTurmaForCreateDto alunoTurma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(alunoTurma);

            Guid codigo = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);

            return Ok(new GuidResponseDto(codigo));
        }

        /// <summary>
        /// Altera a turma de um aluno no relacionamento entre aluno e turma
        /// </summary>
        /// <param name="matricula">Matr�cula do aluno</param>
        /// <param name="codigoTurma">C�digo da turma</param>
        /// <param name="changeTurma">Par�metros usados para trocar a turma</param>
        /// <response code="200">Turma alterada com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma n�o encontrado</response>
        /// <returns>Relacionamento entre aluno e turma</returns>

        [HttpPut("changeTurma/{matricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoTurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTurma([FromRoute] Guid matricula, [FromRoute] Guid codigoTurma, [FromBody] AlunoTurmaChangeTurmaDto changeTurma)
        {
            AlunoTurmaDto alunoTurma = await _alunoTurmaService.AlterarTurma(matriculaAluno: matricula, codigoTurma: codigoTurma, changeTurma: changeTurma);
            return Ok(alunoTurma);
        }

        /// <summary>
        /// Altera a nota de um aluno no relacionamento entre aluno e turma
        /// </summary>
        /// <param name="matricula">Matr�cula do aluno</param>
        /// <param name="codigoTurma">C�digo da turma</param>
        /// <response code="200">Nota alterada com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma n�o encontrado</response>
        /// <response code="400">Par�metros inv�lidos</response>
        /// <param name="changeNota">Par�metros necess�rios para alterar a nota do aluno</param>
        /// <returns>Relacionamento entre aluno e turma</returns>
        [HttpPut("changeNota/{matricula}/from/{codigoTurma}")]
        [Authorize(Roles = "professor")]
        [ProducesResponseType(typeof(AlunoTurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNota([FromRoute] Guid matricula, [FromRoute] Guid codigoTurma, [FromBody] AlunoTurmaChangeNotaDto changeNota)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(changeNota);

            AlunoTurmaDto alunoTurma = await _alunoTurmaService.AlterarNota(matriculaAluno: matricula, codigoTurma: codigoTurma, changeTurma: changeNota);
            return Ok(alunoTurma);
        }

        /// <summary>
        /// Exclui o relacionamento entre um aluno e uma turma
        /// </summary>
        /// <param name="alunoMatricula">Matr�cula do aluno</param>
        /// <param name="codigoTurma">C�digo da turma</param>
        /// <response code="200">Relacionamento entre aluno e turma exclu�do com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma n�o encontrado</response>
        /// <returns>Nenhum conte�do</returns>
        [HttpDelete("remove/{alunoMatricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid alunoMatricula, [FromRoute] Guid codigoTurma)
        {
            await _alunoTurmaService.DeletarAlunoDaTurma(alunoMatricula, codigoTurma);
            return NoContent();
        }

        /// <summary>
        /// Obt�m o relacionamento entre um aluno e turma dados a matr�cula do aluno e o c�digo da turma
        /// </summary>
        /// <param name="alunoMatricula">Matr�cula do aluno</param>
        /// <param name="codigoTurma">C�digo da turma</param>
        /// <param name="opcoes">Op��es adicionais de requisi��o do relacionamento</param>
        /// <response code="200">Relacionamento entre aluno e turma recuperado com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma n�o encontrado</response>
        /// <response code="400">Par�metros inv�lidos</response>
        /// <response code="401">O usu�rio n�o est� autorizado a ter as informa��es</response>
        /// <returns>Relacionamento entre aluno e turma</returns>
        /// <exception cref="BadHttpRequestException">Exce��o gerada caso o token seja inv�lido</exception>
        [HttpGet("find/{alunoMatricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
        [ProducesResponseType(typeof(AlunoTurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] Guid alunoMatricula, [FromRoute] Guid codigoTurma, [FromQuery] GetAlunoTurmaOptions? opcoes = null)
        {
            if(User.IsInRole("aluno"))
            {
                string? alunoMatriculaClaim = User.Claims.FirstOrDefault(c => c.Type == "AlunoMatricula")?.Value;

                if (alunoMatriculaClaim == null)
                    throw new BadHttpRequestException("token inv�lido");

                Guid reqId;

                if (!Guid.TryParse(alunoMatriculaClaim, out reqId))
                    throw new BadHttpRequestException("token inv�lido");

                if (alunoMatricula != reqId)
                    return Unauthorized();
            }

            AlunoTurmaDto aluno = await _alunoTurmaService.ObterAlunoDaTurma(alunoMatricula, codigoTurma, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obt�m o relacionamento entre um aluno e turma dado o c�digo do relacionamento
        /// </summary>
        /// <param name="codigoAlunoTurma">C�digo do relacionamento</param>
        /// <param name="opcoes">Op��es adicionais de requisi��o do relacionamento</param>
        /// <response code="200">Relacionamento entre aluno e turma recuperado com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma n�o encontrado</response>
        /// <response code="400">Par�metros inv�lidos</response>
        /// <response code="401">O usu�rio n�o est� autorizado a ter as informa��es</response>
        /// <returns>Relacionamento entre aluno e turma</returns>
        /// <exception cref="BadHttpRequestException">Exce��o gerada caso o token seja inv�lido</exception>
        [HttpGet("find/{codigoAlunoTurma}")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
        [ProducesResponseType(typeof(AlunoTurmaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get([FromRoute] Guid codigoAlunoTurma, [FromQuery] GetAlunoTurmaOptions? opcoes = null)
        {
            AlunoTurmaDto aluno = await _alunoTurmaService.ObterAlunoDatTurmaPorCodigo(codigoAlunoTurma, opcoes);

            if(User.IsInRole("aluno"))
            {
                string? alunoMatriculaClaim = User.Claims.FirstOrDefault(c => c.Type == "AlunoMatricula")?.Value;

                if (alunoMatriculaClaim == null)
                    throw new BadHttpRequestException("token inv�lido");

                Guid reqId;

                if (!Guid.TryParse(alunoMatriculaClaim, out reqId))
                    throw new BadHttpRequestException("token inv�lido");

                if (aluno.AlunoMatricula != reqId)
                    return Unauthorized();
            }

            return Ok(aluno);
        }
    }
}