using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;

namespace API.Controllers
{
    /// <summary>
    /// Controlador responsável pelos usuários administradores
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsuarioAdminController : ControllerBase
    {
        private readonly IUsuarioAdminService _usuarioAdminService;
        private readonly ILogger<UsuarioAdminController> _logger;

        /// <summary>
        /// Construtor do controlador UsuarioAdminController
        /// </summary>
        /// <param name="usuarioAdminService">Serviço responsável pelos casos de uso envolvendo os administradores</param>
        /// <param name="logger">Logger para o controlador</param>
        public UsuarioAdminController(IUsuarioAdminService usuarioAdminService, ILogger<UsuarioAdminController> logger)
        {
            _logger = logger;
            _usuarioAdminService = usuarioAdminService;
        }

        /// <summary>
        /// Autentica o usuário no sistema retornando um token
        /// </summary>
        /// <param name="loginDto">Parâmetros como email e senha usados para se obter o token</param>
        /// <response code="200">Token JWT</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Email e/ou senha incorretos</response>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(JwtTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string token = await _usuarioAdminService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = token });
        }

        /// <summary>
        /// Obtém-se todos os administradores
        /// </summary>
        /// <response code="200">Lista com todos os administradores</response>
        /// <returns>Lista com todos os administradores</returns>
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(List<UsuarioAdminDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            List<UsuarioAdminDto> admins = await _usuarioAdminService.ObterListaDeUsuariosAdmin();
            return Ok(admins);
        }

        /// <summary>
        /// Exclui um administrador do sistema
        /// </summary>
        /// <param name="email">Email do administrador</param>
        /// <response code="204">Usuário excluído com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Nenhum conteúdo</returns>
        [HttpDelete("{email}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string email)
        {
            await _usuarioAdminService.DeletarUsuarioAdmin(email);
            return NoContent();
        }

        /// <summary>
        /// Registra um novo administrador no sistema
        /// </summary>
        /// <param name="adminDto">Parâmetros necessários para se cadastrar um administrador</param>
        /// <response code="200">Administrador cadastrado com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <returns>Identificador do administrador</returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(GuidResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] UsuarioAdminForCreateDto adminDto)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(adminDto);

            Guid id = await _usuarioAdminService.CadastraUsuarioAdmin(adminDto);

            return Ok(new GuidResponseDto(id));
        }

        /// <summary>
        /// Altera a senha do administrador logado no sistema
        /// </summary>
        /// <param name="changePasswordDto">Parâmetros necessários para se alterar a senha do administrador</param>
        /// <response code="204">Senha alterada com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="401">Senha incorreta</response>
        /// <response code="404">Usuário não encontrado</response>
        /// <returns>Nenhum conteúdo</returns>
        /// <exception cref="BadRequestException">Exceção causada por algum parâmetro inválido</exception>
        [HttpPut("changepassword")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangeUsuarioPasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(changePasswordDto);

            string? idClaim = User.Claims.FirstOrDefault(u => u.Type == "Id")?.Value;

            if (idClaim is null)
                throw new BadRequestException("o token do usuário está incorreto");

            Guid id;
            
            if(!Guid.TryParse(idClaim, out id))
                throw new BadRequestException("o token do usuário está incorreto");

            await _usuarioAdminService.AlterarUsuarioAdminSenha(id, changePasswordDto);

            return NoContent();
        }
    }
}
