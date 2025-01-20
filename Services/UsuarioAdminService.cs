using AutoMapper;
using Domain.Entities.Users;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Shared.Dtos;
using Shared.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UsuarioAdminService: IUsuarioAdminService
    {
        private readonly ILogger<UsuarioAdminService> _logger;

        private readonly ITokensService _tokensService;

        private readonly IRepositoryManager _repositoryManager;

        private readonly IMapper _mapper;

        private readonly IPasswordHash _passwordHash;
        public UsuarioAdminService(ILogger<UsuarioAdminService> logger, ITokensService tokensService, IRepositoryManager repositoryManager, IMapper mapper, IPasswordHash passwordHash)
        {
            _logger = logger;
            _tokensService = tokensService;
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _passwordHash = passwordHash;
        }

        public async Task<string> Login(LoginUsuarioDto loginDto)
        {
            UsuarioAdmin? usuarioAdmin = await _repositoryManager.UsuarioAdminRepository.GetUsuarioAdminByEmailAsync(loginDto.Email);

            if (usuarioAdmin is null)
            {
                _logger.LogError($"o usuário com email: {loginDto.Email} não existe");
                throw new UnauthorizedException($"email e/ou senha incorretos");
            }

            if(!_passwordHash.Decrypt(loginDto.Password, usuarioAdmin.PasswordHash, usuarioAdmin.SalString))
            {
                _logger.LogError($"o email: {loginDto.Email} e a senha: {loginDto.Password} não correspondem a um admin");
                throw new UnauthorizedException($"email e/ou senha incorretos");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", usuarioAdmin.Id.ToString()),
                new Claim("Email", usuarioAdmin.Email),
                new Claim(ClaimTypes.Role, "admin")
            };

            string jwtToken = _tokensService.JwtToken(claims);

            return jwtToken;
        }

        public async Task<Guid> CadastraUsuarioAdmin(UsuarioAdminForCreateDto usuarioAdminDto)
        {
            if(await _repositoryManager.UsuarioAdminRepository.GetUsuarioAdminByEmailAsync(usuarioAdminDto.Email) is not null)
            {
                _logger.LogError($"o email: {usuarioAdminDto.Email} já está sendo usado");
                throw new BadRequestException("o email já está sendo usado");
            }

            UsuarioAdmin usuarioAdmin = _mapper.Map<UsuarioAdmin>(usuarioAdminDto);
            string saltString;

            usuarioAdmin.PasswordHash = _passwordHash.Encrypt(usuarioAdminDto.Password, out saltString);
            usuarioAdmin.SalString = saltString;

            _repositoryManager.UsuarioAdminRepository.AddUsuarioAdmin(usuarioAdmin);
            await _repositoryManager.SaveAsync();

            return usuarioAdmin.Id;
        }

        public async Task DeletarUsuarioAdmin(string email)
        {
            UsuarioAdmin? usuarioAdmin = await _repositoryManager.UsuarioAdminRepository.GetUsuarioAdminByEmailAsync(email);

            if (usuarioAdmin == null)
            {
                _logger.LogError($"O email: {email} não corresponde a nenhum administrador");
                throw new NotFoundException("email não encontrado");
            }

            _repositoryManager.UsuarioAdminRepository.DeleteUsuarioAdmin(usuarioAdmin);
            await _repositoryManager.SaveAsync();
        }

        public async Task AlterarUsuarioAdminSenha(Guid id, ChangeUsuarioPasswordDto changePasswordDto)
        {
            UsuarioAdmin? usuarioAdmin = await _repositoryManager.UsuarioAdminRepository.GetUsuarioAdminByIdAsync(id);

            if(usuarioAdmin == null)
            {
                _logger.LogError($"o usuário com id: {id} não existe");
                throw new NotFoundException("usuário não encontrado");
            }

            if(!_passwordHash.Decrypt(changePasswordDto.OldPassword, usuarioAdmin.PasswordHash, usuarioAdmin.SalString))
            {
                _logger.LogError($"o usuário de email: {usuarioAdmin.Email} não possui a senha: {changePasswordDto.OldPassword}");
                throw new UnauthorizedException("senha incorreta");
            }

            string saltString;
            usuarioAdmin.PasswordHash = _passwordHash.Encrypt(changePasswordDto.NewPassword, out saltString);
            usuarioAdmin.SalString = saltString;

            _repositoryManager.UsuarioAdminRepository.UpdateUsuarioAdmin(usuarioAdmin);

            await _repositoryManager.SaveAsync();
        }

        public async Task<List<UsuarioAdminDto>> ObterListaDeUsuariosAdmin()
        {
            List<UsuarioAdmin> usuariosAdmin = await _repositoryManager.UsuarioAdminRepository.GetAllUsuarioAdmin();
            return _mapper.Map<List<UsuarioAdminDto>>(usuariosAdmin);
        }
    }
}
