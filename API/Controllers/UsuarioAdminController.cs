using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioAdminController : ControllerBase
    {
        private readonly IUsuarioAdminService _usuarioAdminService;
        private readonly ILogger<UsuarioAdminController> _logger;

        public UsuarioAdminController(IUsuarioAdminService usuarioAdminService, ILogger<UsuarioAdminController> logger)
        {
            _logger = logger;
            _usuarioAdminService = usuarioAdminService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string token = await _usuarioAdminService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = token });
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAll()
        {
            List<UsuarioAdminDto> admins = await _usuarioAdminService.ObterListaDeUsuariosAdmin();
            return Ok(admins);
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] string email)
        {
            await _usuarioAdminService.DeletarUsuarioAdmin(email);
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([FromBody] UsuarioAdminForCreateDto adminDto)
        {
            if(!ModelState.IsValid)
                return UnprocessableEntity(adminDto);

            Guid id = await _usuarioAdminService.CadastraUsuarioAdmin(adminDto);

            return Ok(new GuidResponseDto(id));
        }

        [HttpPut("changepassword")]
        [Authorize(Roles = "admin")]
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
