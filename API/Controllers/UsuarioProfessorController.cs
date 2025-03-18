using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelos usuários professores
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioProfessorController : ControllerBase
    {
        private readonly IUsuarioProfessorService _usuarioProfessorService;

        /// <summary>
        /// Construtor do controlador UsuarioProfessorController
        /// </summary>
        /// <param name="usuarioProfessorService">Serviço responsável pelos casos de uso envolvendo os usuários professores</param>
        public UsuarioProfessorController(IUsuarioProfessorService usuarioProfessorService)
        {
            _usuarioProfessorService = usuarioProfessorService;
        }

        /// <summary>
        /// Gera um token de autenticação para o usuário professor
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

            string jwtToken = await _usuarioProfessorService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jwtToken });
        }


        /// <summary>
        /// Altera a senha do usuário professor
        /// </summary>
        /// <param name="changeUsuarioPassword">Parâmetros necessários para se alterar a senha do usuário</param>
        /// <response code="204">Senha alterada com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="401">Senha incorreta</response>
        /// <returns></returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpPut("updatePassword")]
        [Authorize(Roles = "professor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangeUsuarioPasswordDto changeUsuarioPassword)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(changeUsuarioPassword);

            string? idClaim = User.Claims.FirstOrDefault(u => u.Type == "Id")?.Value;

            if (idClaim is null)
                throw new BadRequestException("o token do usuário está incorreto");

            Guid id;

            if (!Guid.TryParse(idClaim, out id))
                throw new BadRequestException("o token do usuário está incorreto");

            await _usuarioProfessorService.AlteraSenhaUsuarioProfessor(id, changeUsuarioPassword);

            return NoContent();
        }

        /// <summary>
        /// Obtém-se os dados do usuário professor pelo identificador
        /// </summary>
        /// <param name="id">Identificador do usuário professor</param>
        /// <param name="opcoes">Parâmetros adicionais para a requisição</param>
        /// <response code="200">Dados do usuário professor retornados com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Dados do usuário professor</returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "administrativo,admin,professor")]
        [ProducesResponseType(typeof(UsuarioProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] GetUsuarioProfessorOptions? opcoes = null)
        {
            if(User.IsInRole("professor"))
            {
                string? idClaim = User.Claims.FirstOrDefault(u => u.Type == "Id")?.Value;

                if (idClaim is null)
                    throw new BadRequestException("o token é inválido");

                Guid req_id;

                if (!Guid.TryParse(idClaim, out req_id))
                    throw new BadRequestException("o token é inválido");

                if (req_id != id)
                    return Unauthorized();
            }

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessor(id, opcoes);
            return Ok(usuarioDto);
        }

        /// <summary>
        /// Obtém-se os dados do usuário professor pelo email
        /// </summary>
        /// <param name="email">Email do usuário professor</param>
        /// <param name="opcoes">Parâmetros adicionais para a requisição</param>
        /// <response code="200">Dados do usuário professor retornados com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Dados do usuário professor</returns>
        /// <exception cref="BadRequestException">Exceção causada por passagem de dados inválidos</exception>
        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "administrativo,admin,professor")]
        [ProducesResponseType(typeof(UsuarioProfessorDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetUsuarioProfessorOptions? opcoes)
        {
            if (User.IsInRole("professor"))
            {
                string? emailClaim = User.Claims.FirstOrDefault(u => u.Type == "Email")?.Value;

                if (emailClaim is null)
                    throw new BadRequestException("o token é inválido");

                if (emailClaim != email)
                    return Unauthorized();
            }

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessorPorEmail(email, opcoes);
            return Ok(usuarioDto);
        }
    }
}
