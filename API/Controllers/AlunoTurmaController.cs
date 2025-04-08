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
        /// <param name="alunoTurmaService">Serviço responsável pelos casos de uso da entidade AlunoTurma</param>
        public AlunoTurmaController(IAlunoTurmaService alunoTurmaService)
        {
            _alunoTurmaService = alunoTurmaService;
        }

        /// <summary>
        /// Cria uma novo relacionamento entre aluno e turma
        /// </summary>
        /// <param name="alunoTurma">Dados usados para a criação do relacionamento entre aluno e turma</param>
        /// <response code="200">Relacionamento entre aluno e turma criado com sucesso</response>
        /// <response code="400">Algum parâmetro passado é inválido</response>
        /// <returns>Código da entidade AlunoTurma</returns>
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
        /// <param name="matricula">Matrícula do aluno</param>
        /// <param name="codigoTurma">Código da turma</param>
        /// <param name="changeTurma">Parâmetros usados para trocar a turma</param>
        /// <response code="200">Turma alterada com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma não encontrado</response>
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
        /// <param name="matricula">Matrícula do aluno</param>
        /// <param name="codigoTurma">Código da turma</param>
        /// <response code="200">Nota alterada com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma não encontrado</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <param name="changeNota">Parâmetros necessários para alterar a nota do aluno</param>
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
        /// <param name="alunoMatricula">Matrícula do aluno</param>
        /// <param name="codigoTurma">Código da turma</param>
        /// <response code="200">Relacionamento entre aluno e turma excluído com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma não encontrado</response>
        /// <returns>Nenhum conteúdo</returns>
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
        /// Obtém o relacionamento entre um aluno e turma dados a matrícula do aluno e o código da turma
        /// </summary>
        /// <param name="alunoMatricula">Matrícula do aluno</param>
        /// <param name="codigoTurma">Código da turma</param>
        /// <param name="opcoes">Opções adicionais de requisição do relacionamento</param>
        /// <response code="200">Relacionamento entre aluno e turma recuperado com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma não encontrado</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">O usuário não está autorizado a ter as informações</response>
        /// <returns>Relacionamento entre aluno e turma</returns>
        /// <exception cref="BadHttpRequestException">Exceção gerada caso o token seja inválido</exception>
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
                    throw new BadHttpRequestException("token inválido");

                Guid reqId;

                if (!Guid.TryParse(alunoMatriculaClaim, out reqId))
                    throw new BadHttpRequestException("token inválido");

                if (alunoMatricula != reqId)
                    return Unauthorized();
            }

            AlunoTurmaDto aluno = await _alunoTurmaService.ObterAlunoDaTurma(alunoMatricula, codigoTurma, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém o relacionamento entre um aluno e turma dado o código do relacionamento
        /// </summary>
        /// <param name="codigoAlunoTurma">Código do relacionamento</param>
        /// <param name="opcoes">Opções adicionais de requisição do relacionamento</param>
        /// <response code="200">Relacionamento entre aluno e turma recuperado com sucesso</response>
        /// <response code="404">Relacionamento entre aluno e turma não encontrado</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">O usuário não está autorizado a ter as informações</response>
        /// <returns>Relacionamento entre aluno e turma</returns>
        /// <exception cref="BadHttpRequestException">Exceção gerada caso o token seja inválido</exception>
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
                    throw new BadHttpRequestException("token inválido");

                Guid reqId;

                if (!Guid.TryParse(alunoMatriculaClaim, out reqId))
                    throw new BadHttpRequestException("token inválido");

                if (aluno.AlunoMatricula != reqId)
                    return Unauthorized();
            }

            return Ok(aluno);
        }
    }
}