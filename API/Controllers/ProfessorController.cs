using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,administrativo")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService)
        {
            _professorService = professorService;
        } 

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProfessorForCreateDto professor)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(professor);

            Guid matricula = await _professorService.CadastrarProfessor(professor);

            return Ok(new GuidResponseDto(matricula));
        }

        [HttpPut("{matricula}")]
        public async Task<IActionResult> Update([FromRoute] Guid matricula, [FromBody] ProfessorForUpdateDto professor)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(professor);

            ProfessorDto professorDto = await _professorService.AlterarProfessor(matricula, professor);

            return Ok(professorDto);
        }

        [HttpDelete("{matricula}")]
        public async Task<IActionResult> Delete([FromRoute] Guid matricula)
        {
            await _professorService.DeletarProfessor(matricula);
            return NoContent();
        }

        [HttpGet("{matricula}")]
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorMatricula(matricula, opcoes);
            return Ok(professor); 
        }

        [HttpGet("by/cpf/{cpf}")]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorCPF(cpf, opcoes);
            return Ok(professor); 
        }

        [HttpGet("by/rg/{rg}")]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPorRG(rg, opcoes);
            return Ok(professor); 
        }

        [HttpGet("by/email/{email}")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPeloEmail(email, opcoes);
            return Ok(professor); 
        }

        [HttpGet("by/celular/{celular}")]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetProfessorOptions? opcoes = null)
        {
            ProfessorDto professor = await _professorService.ObterProfessorPeloCelular(celular, opcoes);
            return Ok(professor); 
        }

        [HttpGet("filter")]
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