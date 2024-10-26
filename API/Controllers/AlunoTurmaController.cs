using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
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
        public async Task<IActionResult> Add([FromBody] AlunoTurmaForCreateDto alunoTurma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(alunoTurma);

            Guid codigo = await _alunoTurmaService.CadastrarAlunoNaTurma(alunoTurma);

            return Ok(codigo);
        }

        [HttpPut("student/{matricula}/from/{codigoTurma}")]
        public async Task<IActionResult> Update([FromRoute] Guid matricula, [FromRoute] Guid codigoTurma, [FromBody] AlunoTurmaForUpdateDto alunoTurma)
        {
            AlunoTurmaDto resultado = await _alunoTurmaService.AlterarAlunoNaTurma(matricula, codigoTurma, alunoTurma);
            return Ok(resultado);
        }

        [HttpDelete("remove/{alunoMatricula}/from/{codigoTurma}")]
        public async Task<IActionResult> Delete([FromRoute] Guid alunoMatricula, [FromRoute] Guid codigoTurma)
        {
            await _alunoTurmaService.DeletarAlunoDaTurma(alunoMatricula, codigoTurma);
            return NoContent();
        }

        [HttpGet("find/{alunoMatricula}/from/{codigoTurma}")]
        public async Task<IActionResult> Get([FromRoute] Guid alunoMatricula, [FromRoute] Guid codigoTurma, [FromQuery] GetAlunoTurmaOptions? opcoes = null)
        {
            AlunoTurmaDto aluno = await _alunoTurmaService.ObterAlunoDaTurma(alunoMatricula, codigoTurma, opcoes);
            return Ok(aluno);
        }

        [HttpGet("find/{codigoAlunoTurma}")]
        public async Task<IActionResult> Get([FromRoute] Guid codigoAlunoTurma, [FromQuery] GetAlunoTurmaOptions? opcoes = null)
        {
            AlunoTurmaDto aluno = await _alunoTurmaService.ObterAlunoDatTurmaPorCodigo(codigoAlunoTurma, opcoes);
            return Ok(aluno);
        }
    }
}