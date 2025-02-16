using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioAlunoController : ControllerBase
    {
        private readonly IUsuarioAlunoService _usuarioAlunoService;

        public UsuarioAlunoController(IUsuarioAlunoService usuarioAlunoService)
        {
            _usuarioAlunoService = usuarioAlunoService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(loginDto);

            string jqwToken = await _usuarioAlunoService.Login(loginDto);

            return Ok(new JwtTokenDto { Token = jqwToken });
        }

        [HttpPut("updatePassword")]
        [Authorize(Roles = "aluno")]
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
            return Ok();
        }

        [HttpGet("by/id/{id}")]
        [Authorize(Roles = "admin,administrativo,aluno")]
        /* Todo: incluir entidade aluno com parâmetro opcional*/
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

        [HttpGet("by/email/{email}")]
        [Authorize(Roles = "admin,administrativo,aluno")]
        /* Todo: incluir entidade aluno com parâmetro opcional*/
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
