using System;
using System.Collections.Generic;
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
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _turmaService;

        public TurmaController(ITurmaService turmaService)
        { 
            _turmaService = turmaService;
        }

        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> Add([FromBody] TurmaForCreateDto turma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(turma);

            Guid codigo = await _turmaService.CadastrarTurma(turma);

            return Ok(new GuidResponseDto(codigo));
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> Update([FromRoute] Guid codigo, [FromBody] TurmaForUpdateDto turma)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(turma);

            TurmaDto turmaDto = await _turmaService.AlterarTurma(codigo, turma);

            return Ok(turmaDto);
        }

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "admin,administrativo")]
        public async Task<IActionResult> Delete([FromRoute] Guid codigo)
        {
            await _turmaService.DeletarTurma(codigo);
            return NoContent();
        }

        [HttpGet("{codigo}")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
        /* Todo: esconder dados sensíveis dos alunos ao incluir todos os alunos: como nota quando for um aluno requisitando*/
        public async Task<IActionResult> Get([FromRoute] Guid codigo, [FromQuery] GetTurmaOptions? opcoes = null)
        {
            TurmaDto turma = await _turmaService.ObterTurmaPorCodigo(codigo, opcoes);

            return Ok(turma);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "admin,administrativo,professor,aluno")]
        /* Todo: esconder dados sensíveis dos alunos ao incluir todos os alunos*/
        public async Task<IActionResult> Filter([FromQuery] GetTurmasOptions opcoes)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(opcoes);

            Pagination<TurmaDto> pagination = await _turmaService.ObterTurmas(opcoes);

            if(!pagination.Items.Any())
                return NoContent();

            return Ok(pagination);
        }
    }
}