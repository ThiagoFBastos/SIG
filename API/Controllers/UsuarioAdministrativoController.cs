using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioAdministrativoController : ControllerBase
    {
        private readonly IUsuarioAdministrativoService _usuarioAdministrativoService;

        public UsuarioAdministrativoController(IUsuarioAdministrativoService usuarioAdministrativoService)
        {
            _usuarioAdministrativoService = usuarioAdministrativoService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string jwtToken = await _usuarioAdministrativoService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jwtToken });
        }


        [HttpPut("updatePassword")]
        [Authorize(Roles = "administrativo")]
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

        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "administrativo,admin")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
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

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativo(id);
            return Ok(usuarioDto);
        }

        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "administrativo,admin")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            if(User.IsInRole("administrativo"))
            {
                string? emailClaim = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

                if (emailClaim == null)
                    throw new BadRequestException("token inválido");

                if(email != emailClaim)
                    return Unauthorized();
            }

            UsuarioAdministrativoDto usuarioDto = await _usuarioAdministrativoService.ObterUsuarioAdministrativoPorEmail(email);
            return Ok(usuarioDto);
        }
    }
}
