using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UsuarioAlunoService: IUsuarioAlunoService
    {
        private readonly ILogger<UsuarioAlunoService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash;
        private readonly ITokensService _tokensService;
        private readonly IRepositoryManager _repositoryManager;
        public UsuarioAlunoService(ILogger<UsuarioAlunoService> logger, IMapper mapper, IPasswordHash passwordHash, ITokensService tokensService, IRepositoryManager repositoryManager)
        {
            _logger = logger;
            _mapper = mapper;
            _passwordHash = passwordHash;
            _tokensService = tokensService;
            _repositoryManager = repositoryManager;
        }

        public async Task<string> Login(LoginUsuarioDto loginUsuarioDto)
        {
            UsuarioAluno? usuario = await _repositoryManager.UsuarioAlunoRepository.GetAlunoByEmailAsync(loginUsuarioDto.Email);

            if(usuario is null)
            {
                _logger.LogError($"o usuário com email: {loginUsuarioDto.Email} não foi econtrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }
            else if(!_passwordHash.Decrypt(loginUsuarioDto.Password, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"o usuário com email: {loginUsuarioDto.Email} e senha: {loginUsuarioDto.Password} não foi econtrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("AlunoMatricula", usuario.AlunoMatricula.ToString()),
                new Claim(ClaimTypes.Role, "aluno")
            };

            string jwtToken = _tokensService.JwtToken(claims);

            return jwtToken;
        }
        public async Task CadastraUsuarioAluno(UsuarioAlunoForCreateDto usuarioAlunoDto)
        {
            UsuarioAluno usuario = _mapper.Map<UsuarioAluno>(usuarioAlunoDto);

            if(await _repositoryManager.UsuarioAlunoRepository.GetAlunoByEmailAsync(usuarioAlunoDto.Email) is not null)
            {
                _logger.LogError($"o usuário com email: {usuarioAlunoDto.Email} já está cadastrado");
                throw new BadRequestException("usuário com email já existente");
            }

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(usuarioAlunoDto.Password, out saltString);
            usuario.SalString = saltString;

            _repositoryManager.UsuarioAlunoRepository.AddUsuarioAluno(usuario);
            await _repositoryManager.SaveAsync();
        }
        public async Task AlteraSenhaUsuarioAluno(Guid id, ChangeUsuarioPasswordDto changePasswordDto)
        {
            UsuarioAluno? usuario = await _repositoryManager.UsuarioAlunoRepository.GetAlunoAsync(id);

            if(usuario is null)
            {
                _logger.LogError($"usuário com id: {id} não encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            else if(!_passwordHash.Decrypt(changePasswordDto.OldPassword, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"a senha: {changePasswordDto.OldPassword} está incorreta");
                throw new UnauthorizedException("senha incorreta");
            }

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(changePasswordDto.NewPassword, out saltString);
            usuario.SalString = saltString;

            _repositoryManager.UsuarioAlunoRepository.UpdateUsuarioAluno(usuario);
            await _repositoryManager.SaveAsync();
        }
        public async Task<UsuarioAlunoDto> ObterUsuarioAluno(Guid id, GetUsuarioAlunoOptions? opcoes = null)
        {
            UsuarioAluno? usuario = await _repositoryManager.UsuarioAlunoRepository.GetAlunoAsync(id, opcoes);

            if(usuario is null)
            {
                _logger.LogError($"o usuário com id: {id} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioAlunoDto usuarioDto = _mapper.Map<UsuarioAlunoDto>(usuario);

            return usuarioDto;
        }
        public async Task<UsuarioAlunoDto> ObterUsuarioAlunoPorEmail(string email, GetUsuarioAlunoOptions? opcoes = null)
        {
            UsuarioAluno? usuario = await _repositoryManager.UsuarioAlunoRepository.GetAlunoByEmailAsync(email, opcoes);

            if (usuario is null)
            {
                _logger.LogError($"o usuário com email: {email} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioAlunoDto usuarioDto = _mapper.Map<UsuarioAlunoDto>(usuario);

            return usuarioDto;
        }
    }
}
