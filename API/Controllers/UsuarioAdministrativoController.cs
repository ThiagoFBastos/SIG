using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Shared.Pagination;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelos usuários do administrativo
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioAdministrativoController : ControllerBase
    {
        private readonly IUsuarioAdministrativoService _usuarioAdministrativoService;

        /// <summary>
        /// Construtor do controlador UsuarioAdministrativoController
        /// </summary>
        /// <param name="usuarioAdministrativoService">Serviço responsável pelos casos de uso envolvendo os administrativos</param>
        public UsuarioAdministrativoController(IUsuarioAdministrativoService usuarioAdministrativoService)
        {
            _usuarioAdministrativoService = usuarioAdministrativoService;
        }

        /// <summary>
        /// Autentica um usuário administrativo na api retornando um token
        /// </summary>
        /// <response code="200">Token gerado com sucesso</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <param name="loginDto">Dados necessários para se realizar o login como email e senha</param>
        /// <returns>Retorna um token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string jwtToken = await _usuarioAdministrativoService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jwtToken });
        }

        /// <summary>
        /// Altera a senha do administrativo logado no sistema
        /// </summary>
        /// <response code="204">Senha alterada com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="401">Senha incorreta</response>
        /// <param name="changeUsuarioPassword">Parâmetros necessários para se mudar a senha</param>
        /// <returns></returns>
        /// <exception cref="BadRequestException">Exceção causada por parâmetros inválidos</exception>
        [HttpPut("updatePassword")]
        [Authorize(Roles = "administrativo")]
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

            await _usuarioAdministrativoService.AlteraSenhaUsuarioAdministrativo(id, changeUsuarioPassword);

            return NoContent();
        }

        /// <summary>
        /// Obtém-se o usuário pelo seu identificador
        /// </summary>
        /// <param name="id">identificador do usuário</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <response code="200">Dados do usuário</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <returns>Dados do usuário requisitado</returns>
        /// <exception cref="BadRequestException">Exceção causada por algum parâmetro inválido passado</exception>
        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "administrativo,admin")]
        [ProducesResponseType(typeof(UsuarioAdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] GetUsuarioAdministrativoOptions? opcoes = null)
        {
            if(User.IsInRole("administrativo"))
            {
                string? idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                if (idClaim == null)
                    throw new BadRequestException("token inválido");

                Guid reqId;

                if (!Guid.TryParse(idClaim, out reqId))
                    throw new BadRequestException("token inválido");

                if (id != reqId)
                    return Unauthorized();
            }

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativo(id, opcoes);
            return Ok(usuarioDto);
        }

        /// <summary>
        /// Obtém-se o usuário pelo email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="opcoes">Opções adicionais de requisição</param>
        /// <response code="200">Dados do usuário</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <response code="401">Usuário não autorizado</response>
        /// <returns>Dados do usuário requisitado</returns>
        /// <exception cref="BadRequestException">Exceção causada por algum parâmetro inválido passado</exception>
        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "administrativo,admin")]
        [ProducesResponseType(typeof(UsuarioAdministrativoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email, [FromQuery] GetUsuarioAdministrativoOptions? opcoes = null)
        {
            if(User.IsInRole("administrativo"))
            {
                string? emailClaim = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

                if (emailClaim == null)
                    throw new BadRequestException("token inválido");

                if(email != emailClaim)
                    return Unauthorized();
            }

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativoPorEmail(email, opcoes);
            return Ok(usuarioDto);
        }
    }
}
