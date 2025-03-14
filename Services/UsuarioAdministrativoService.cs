using AutoMapper;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using System.Security.Claims;
using Shared.Pagination;

namespace Services
{
    public class UsuarioAdministrativoService: IUsuarioAdministrativoService
    {
        private readonly ILogger<UsuarioAdministrativoService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHash _passwordHash;
        private readonly IRepositoryManager _repositoryManaher;
        private readonly ITokensService _tokensService;
        public UsuarioAdministrativoService(ILogger<UsuarioAdministrativoService> logger, IMapper mapper, IPasswordHash passwordHash, IRepositoryManager repositoryManager, ITokensService tokensService)
        {
            _logger = logger;
            _mapper = mapper;
            _passwordHash = passwordHash;
            _repositoryManaher = repositoryManager;
            _tokensService = tokensService;
        }

        public async Task<string> Login(LoginUsuarioDto loginUsuarioDto)
        {
            UsuarioAdministrativo? usuario = await _repositoryManaher.UsuarioAdministrativoRepository.GetAdminstrativoByEmailAsync(loginUsuarioDto.Email);

            if(usuario is null)
            {
                _logger.LogError($"o usuário: {loginUsuarioDto.Email} com senha: {loginUsuarioDto.Password} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            else if(!_passwordHash.Decrypt(loginUsuarioDto.Password, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"o usuário: {loginUsuarioDto.Email} com senha: {loginUsuarioDto.Password} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Email", usuario.Email),
                new Claim("AdministrativoMatricula", usuario.AdministrativoMatricula.ToString()),
                new Claim(ClaimTypes.Role, "administrativo")
            };

            string jwt = _tokensService.JwtToken(claims);

            return jwt;
        }

        public async Task CadastraUsuarioAdministrativo(UsuarioAdministrativoForCreateDto usuarioAdministrativoDto)
        {
            if(await _repositoryManaher.UsuarioAdministrativoRepository.GetAdminstrativoByEmailAsync(usuarioAdministrativoDto.Email) is not null)
            {
                _logger.LogError($"o usuário com email: {usuarioAdministrativoDto.Email} já está cadastrado");
                throw new BadRequestException("email já existente");
            }

            UsuarioAdministrativo usuario = _mapper.Map<UsuarioAdministrativo>(usuarioAdministrativoDto);

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(usuarioAdministrativoDto.Password, out saltString);
            usuario.SalString = saltString;

            _repositoryManaher.UsuarioAdministrativoRepository.AddUsuarioAdministrativo(usuario);
            await _repositoryManaher.SaveAsync();
        }

        public async Task AlteraSenhaUsuarioAdministrativo(Guid id, ChangeUsuarioPasswordDto changePasswordDto)
        {
            UsuarioAdministrativo? usuario = await _repositoryManaher.UsuarioAdministrativoRepository.GetAdministrativoAsync(id);

            if(usuario is null)
            {
                _logger.LogError($"o usuário com id: {id} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            else if(!_passwordHash.Decrypt(changePasswordDto.OldPassword, usuario.PasswordHash, usuario.SalString))
            {
                _logger.LogError($"o usuário: {usuario.Email} não possui senha: {changePasswordDto.OldPassword} não foi encontrado");
                throw new UnauthorizedException("email e/ou senha incorretos");
            }

            string saltString;

            usuario.PasswordHash = _passwordHash.Encrypt(changePasswordDto.NewPassword, out saltString);

            usuario.SalString = saltString;

            _repositoryManaher.UsuarioAdministrativoRepository.UpdateUsuarioAdministrativo(usuario);
            await _repositoryManaher.SaveAsync();
        }

        public async Task<UsuarioAdministrativoDto> ObterUsuarioAdministrativo(Guid id, GetUsuarioAdministrativoOptions? opcoes = null)
        {
            UsuarioAdministrativo? usuario = await _repositoryManaher.UsuarioAdministrativoRepository.GetAdministrativoAsync(id, opcoes);

            if(usuario is null)
            {
                _logger.LogError($"o usuário com id: {id} não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioAdministrativoDto usuarioDto = _mapper.Map<UsuarioAdministrativoDto>(usuario);

            return usuarioDto;
        }

       public async Task<UsuarioAdministrativoDto> ObterUsuarioAdministrativoPorEmail(string email, GetUsuarioAdministrativoOptions? opcoes = null)
        {
            UsuarioAdministrativo? usuario = await _repositoryManaher.UsuarioAdministrativoRepository.GetAdminstrativoByEmailAsync(email, opcoes);

            if (usuario is null)
            {
                _logger.LogError($"o usuário com email: email não foi encontrado");
                throw new NotFoundException("usuário não encontrado");
            }

            UsuarioAdministrativoDto usuarioDto = _mapper.Map<UsuarioAdministrativoDto>(usuario);

            return usuarioDto;
        }
    }
}
