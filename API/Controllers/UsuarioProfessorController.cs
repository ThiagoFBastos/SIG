using Domain.Entities.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Contracts;
using Shared.Dtos;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioProfessorController : ControllerBase
    {
        private readonly IUsuarioProfessorService _usuarioProfessorService;

        public UsuarioProfessorController(IUsuarioProfessorService usuarioProfessorService)
        {
            _usuarioProfessorService = usuarioProfessorService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string jwtToken = await _usuarioProfessorService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jwtToken });
        }

        [HttpPut("updatePassword")]
        [Authorize(Roles = "professor")]
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


        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "administrativo,admin,professor")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
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

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessor(id);
            return Ok(usuarioDto);
        }

        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "administrativo,admin,professor")]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            if (User.IsInRole("professor"))
            {
                string? emailClaim = User.Claims.FirstOrDefault(u => u.Type == "Email")?.Value;

                if (emailClaim is null)
                    throw new BadRequestException("o token é inválido");

                if (emailClaim != email)
                    return Unauthorized();
            }

            UsuarioProfessorDto usuarioDto = await _usuarioProfessorService.ObterUsuarioProfessorPorEmail(email);
            return Ok(usuarioDto);
        }
    }
}
