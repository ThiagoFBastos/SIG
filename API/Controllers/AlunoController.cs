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
    /// <summary>
    /// Controlador responsável pelas operações dos alunos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        /// <summary>
        /// Construtor do AlunoController
        /// </summary>
        /// <param name="alunoService">Serviço responsável pelos casos de uso realizados pelos alunos</param>
        public AlunoController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Cadastra um novo aluno
        /// </summary>
        /// <param name="aluno">Dados de um aluno para o cadastro</param>
        /// <response code="200">Aluno foi cadastrado com sucesso</response>
        /// <response code="400">Algum parâmetro inválido foi passado</response>
        /// <returns>A matrícula do novo aluno cadastrado</returns>
        [HttpPost]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AlunoForCreateDto aluno)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(aluno);

            Guid matricula = await _alunoService.CadastrarAluno(aluno);

            return Ok(new GuidResponseDto(matricula));
        }

        /// <summary>
        /// Altera os dados de um aluno dado sua matrícula e seus novos dados
        /// </summary>
        /// <param name="matricula">Matrícula do aluno que terá seus dados alterados</param>
        /// <response code="200">Aluno foi alterado com sucesso</response>
        /// <response code="400">Algum parâmetro inválido foi passado</response>
        /// <response code="404">O Aluno não foi encontrado</response>
        /// <returns>A matrícula do novo aluno cadastrado</returns>
        [HttpPut("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid matricula, [FromBody] AlunoForUpdateDto aluno)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(aluno);

            AlunoDto alunoDto = await _alunoService.AlterarAluno(matricula, aluno);

            return Ok(alunoDto);
        }

        /// <summary>
        /// Exclui um aluno dado a sua matrícula
        /// </summary>
        /// <param name="matricula">Matrícula do aluno que se deseja excluir</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid matricula)
        {
            await _alunoService.DeletarAluno(matricula);
            return NoContent();
        }

        /// <summary>
        /// Obtém o aluno com a matrícula requisitada
        /// </summary>
        /// <param name="matricula">Matrícula do aluno</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <returns>Um aluno</returns>
        [HttpGet("{matricula}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid matricula, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorMatricula(matricula, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém o aluno com o CPF requisitado
        /// </summary>
        /// <param name="cpf">CPF do aluno</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <returns>Um aluno</returns>
        [HttpGet("by/cpf/{cpf}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCPF([FromRoute] string cpf, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorCPF(cpf, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém o aluno com o RG requisitado
        /// </summary>
        /// <param name="rg">RG do aluno</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <returns>Um aluno</returns>
        [HttpGet("by/rg/{rg}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRG([FromRoute] string rg, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPorRG(rg, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém o aluno com o email requisitado
        /// </summary>
        /// <param name="email">Email do aluno</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <returns>Um aluno</returns>
        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPeloEmail(email, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém o aluno com o celular requisitado
        /// </summary>
        /// <param name="celular">Celular do aluno</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <returns>Um aluno</returns>
        [HttpGet("by/celular/{celular}")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(AlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCelular([FromRoute] string celular, [FromQuery] GetAlunoOptions? opcoes = null)
        {
            AlunoDto aluno = await _alunoService.ObterAlunoPeloCelular(celular, opcoes);
            return Ok(aluno);
        }

        /// <summary>
        /// Obtém uma lista de alunos com base em parâmetros de filtragem
        /// </summary>
        /// <param name="opcoes">Parâmetros de filtragem</param>
        /// <returns>Uma lista paginada de alunos</returns>
        [HttpGet("filter")]
        [Authorize(Roles = "admin,administrativo")]
        [ProducesResponseType(typeof(Pagination<AlunoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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