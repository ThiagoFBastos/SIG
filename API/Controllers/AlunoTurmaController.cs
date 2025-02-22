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
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoTurmaController : ControllerBase
    {
        private readonly IAlunoTurmaService _alunoTurmaService;

        public AlunoTurmaController(IAlunoTurmaService alunoTurmaService)
        {
            _alunoTurmaService = alunoTurmaService;
        }

        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> Add([FromBody] AlunoTurmaForCreateDto alunoTurma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(alunoTurma);

            Guid codigo = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);

            return Ok(new GuidResponseDto(codigo));
        }

        [HttpPut("changeTurma/{matricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> UpdateTurma([FromRoute] Guid matricula, [FromRoute] Guid codigoTurma, [FromBody] AlunoTurmaChangeTurmaDto changeTurma)
        {
            AlunoTurmaDto alunoTurma = await _alunoTurmaService.AlterarTurma(matriculaAluno: matricula, codigoTurma: codigoTurma, changeTurma: changeTurma);
            return Ok(alunoTurma);
        }

        [HttpPut("changeNota/{matricula}/from/{codigoTurma}")]
        [Authorize(Roles = "professor")]
        public async Task<IActionResult> UpdateNota([FromRoute] Guid matricula, [FromRoute] Guid codigoTurma, [FromBody] AlunoTurmaChangeNotaDto changeNota)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(changeNota);

            AlunoTurmaDto alunoTurma = await _alunoTurmaService.AlterarNota(matriculaAluno: matricula, codigoTurma: codigoTurma, changeTurma: changeNota);
            return Ok(alunoTurma);
        }

        [HttpDelete("remove/{alunoMatricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> Delete([FromRoute] Guid alunoMatricula, [FromRoute] Guid codigoTurma)
        {
            await _alunoTurmaService.DeletarAlunoDaTurma(alunoMatricula, codigoTurma);
            return NoContent();
        }

        [HttpGet("find/{alunoMatricula}/from/{codigoTurma}")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
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

        [HttpGet("find/{codigoAlunoTurma}")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
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