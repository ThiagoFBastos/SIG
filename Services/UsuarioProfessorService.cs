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
    public class UsuarioProfessorService: IUsuarioProfessorService
    {
        private readonly ILogger<UsuarioProfessorService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash;
        private readonly IRepositoryManager _repositoryManaher;
        private readonly ITokensService _tokensService;

        public UsuarioProfessorService(ILogger<UsuarioProfessorService> logger, IMapper mapper, IPasswordHash passwordHash, IRepositoryManager repositoryManaher, ITokensService tokensService)
        {
            _logger = logger;
            _mapper = mapper;
            _passwordHash = passwordHash;
            _repositoryManaher = repositoryManaher;
            _tokensService = tokensService;
        }

        public async Task<string> Login(LoginUsuarioDto loginUsuarioDto)
        {
            UsuarioProfessor? usuario = await _repositoryManaher.UsuarioProfessorRepository.GetProfessorByEmailAsync(loginUsuarioDto.Email);

            if (usuario is null)
            {
                _logger.LogError($"o usuário: {loginUsuarioDto.Email} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            else if (!_passwordHash.Decrypt(loginUsuarioDto.Password, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"o usuário: {loginUsuarioDto.Email} com senha: {loginUsuarioDto.Password} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("ProfessorMatricula", usuario.ProfessorMatricula.ToString()),
                new Claim(ClaimTypes.Role, "professor")
            };

            string jwt = _tokensService.JwtToken(claims);

            return jwt;
        }

        public async Task CadastraUsuarioProfessor(UsuarioProfessorForCreateDto usuarioProfessorDto)
        {
            if (await _repositoryManaher.UsuarioProfessorRepository.GetProfessorByEmailAsync(usuarioProfessorDto.Email) is not null)
            {
                _logger.LogError($"o usuário com email: {usuarioProfessorDto.Email} já está cadastrado");
                throw new BadRequestException("email já existente");
            }

            UsuarioProfessor usuario = _mapper.Map<UsuarioProfessor>(usuarioProfessorDto);

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(usuarioProfessorDto.Password, out saltString);
            usuario.SalString = saltString;

            _repositoryManaher.UsuarioProfessorRepository.AddUsuarioProfessor(usuario);
            await _repositoryManaher.SaveAsync();
        }

        public async Task AlteraSenhaUsuarioProfessor(Guid id, ChangeUsuarioPasswordDto changePasswordDto)
        {
            UsuarioProfessor? usuario = await _repositoryManaher.UsuarioProfessorRepository.GetProfessorAsync(id);

            if (usuario is null)
            {
                _logger.LogError($"o usuário com id: {id} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            else if (!_passwordHash.Decrypt(changePasswordDto.OldPassword, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"o usuário: {usuario.Email} não possui senha: {changePasswordDto.OldPassword} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(changePasswordDto.NewPassword, out saltString);

            usuario.SalString = saltString;

            _repositoryManaher.UsuarioProfessorRepository.UpdateUsuarioProfessor(usuario);
            await _repositoryManaher.SaveAsync();
        }

        public async Task<UsuarioProfessorDto> ObterUsuarioProfessor(Guid id, GetUsuarioProfessorOptions? opcoes = null)
        {
            UsuarioProfessor? usuario = await _repositoryManaher.UsuarioProfessorRepository.GetProfessorAsync(id, opcoes);

            if (usuario is null)
            {
                _logger.LogError($"o usuário com id: {id} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioProfessorDto usuarioDto = _mapper.Map<UsuarioProfessorDto>(usuario);

            return usuarioDto;
        }

        public async Task<UsuarioProfessorDto> ObterUsuarioProfessorPorEmail(string email, GetUsuarioProfessorOptions? opcoes = null)
        {
            UsuarioProfessor? usuario = await _repositoryManaher.UsuarioProfessorRepository.GetProfessorByEmailAsync(email, opcoes);

            if (usuario is null)
            {
                _logger.LogError($"o usuário com email: email não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioProfessorDto usuarioDto = _mapper.Map<UsuarioProfessorDto>(usuario);

            return usuarioDto;
        }
    }
}
