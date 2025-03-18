using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelos usuários alunos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioAlunoController : ControllerBase
    {
        private readonly IUsuarioAlunoService _usuarioAlunoService;

        /// <summary>
        /// Construtor do controlador UsuarioAlunoController
        /// </summary>
        /// <param name="usuarioAlunoService">Serviço responsável pelos casos de uso envolvendo os usuários alunos</param>
        public UsuarioAlunoController(IUsuarioAlunoService usuarioAlunoService)
        {
            _usuarioAlunoService = usuarioAlunoService;
        }

        /// <summary>
        /// Gera um token de autenticação para o usuário aluno
        /// </summary>
        /// <param name="loginDto">Parâmetros necessários para se autenticar um usuário como email e senha</param>
        /// <response code="200">Token gerado com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Email e/ou senha incorretos</response>
        /// <returns>Um token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string jqwToken = await _usuarioAlunoService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jqwToken });
        }

        /// <summary>
        /// Altera a senha do usuário aluno
        /// </summary>
        /// <param name="changePassword">Parâmetros necessários para se alterar a senha do usuário</param>
        /// <response code="204">Senha alterada com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="401">Senha incorreta</response>
        /// <returns></returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpPut("updatePassword")]
        [Authorize(Roles = "aluno")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangeUsuarioPasswordDto changePassword)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(changePassword);

            string? idClaim = User.Claims.FirstOrDefault(u => u.Type == "Id")?.Value;

            if (idClaim is null)
                throw new BadRequestException("o token do usuário está incorreto");

            Guid id;

            if(!Guid.TryParse(idClaim, out id))
                throw new BadRequestException("o token do usuário está incorreto");

            await _usuarioAlunoService.AlteraSenhaUsuarioAluno(id, changePassword);
            return NoContent();
        }

        /// <summary>
        /// Obtém-se os dados do usuário aluno pelo identificador
        /// </summary>
        /// <param name="id">Identificador do usuário aluno</param>
        /// <param name="opcoes">Parâmetros adicionais para a requisição</param>
        /// <response code="200">Dados do usuário aluno retornados com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Dados do usuário aluno</returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "admin,administrativo,aluno")]
        [ProducesResponseType(typeof(UsuarioAlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] GetUsuarioAlunoOptions? opcoes = null)
        {
            if(User.IsInRole("aluno"))
            {
                string? idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (idClaim is null)
                    throw new BadRequestException("o token é inválido");

                Guid req_id;

                if(!Guid.TryParse(idClaim, out req_id))
                    throw new BadRequestException("o token é inválido");

                if (id != req_id)
                    return Unauthorized();
            }

            UsuarioAlunoDto usuario = await _usuarioAlunoService.ObterUsuarioAluno(id, opcoes);
            return Ok(usuario);
        }

        /// <summary>
        /// Obtém-se os dados do usuário aluno pelo email
        /// </summary>
        /// <param name="email">Email do usuário aluno</param>
        /// <param name="opcoes">Parâmetros adicionais para a requisição</param>
        /// <response code="200">Dados do usuário aluno retornados com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Dados do usuário aluno</returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "admin,administrativo,aluno")]
        [ProducesResponseType(typeof(UsuarioAlunoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetUsuarioAlunoOptions? opcoes = null)
        {
            if(User.IsInRole("aluno"))
            {
                string? emailClaim = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

                if (emailClaim is null)
                    throw new BadRequestException("o token é inválido");


                if (emailClaim != email)
                    return Unauthorized();
            }

            UsuarioAlunoDto usuario = await _usuarioAlunoService.ObterUsuarioAlunoPorEmail(email, opcoes);
            return Ok(usuario);
        }
    }
}
