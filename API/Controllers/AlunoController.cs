using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AlunoForCreateDto aluno)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(aluno);

            Guid matricula = await _alunoService.CadastrarAluno(aluno);

            return Ok(new GuidResponseDto(matricula));
        }

        [HttpPut("{matricula}")]
        public async Task<IActionResult> Update([FromRoute] Guid matricula, [FromBody] AlunoForUpdateDto aluno)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(aluno);

            AlunoDto alunoDto = await _alunoService.AlterarAluno(matricula, aluno);

            return Ok(alunoDto);
        }

        [HttpDelete("{matricula}")]
        public async Task<IActionResult> Delete([FromRoute] Guid matricula)
        {
            await _alunoService.DeletarAluno(matricula);
            return NoContent();
        }

        [HttpGet("{matricula}")]
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorMatricula(matricula, opcoes);
            return Ok(aluno);
        }

        [HttpGet("by/cpf/{cpf}")]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorCPF(cpf, opcoes);
            return Ok(aluno);
        }

        [HttpGet("by/rg/{rg}")]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorRG(rg, opcoes);
            return Ok(aluno);
        }

        [HttpGet("by/email/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPeloEmail(email, opcoes);
            return Ok(aluno);
        }

        [HttpGet("by/celular/{celular}")]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPeloCelular(celular, opcoes);
            return Ok(aluno);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] GetAlunosOptions opcoes)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(opcoes);

            Pagination<AlunoDto> pagination = await _alunoService.ObterAlunos(opcoes);

            if(!pagination.Items.Any())
                return NoContent();

            return Ok(pagination);
        }
    }
}