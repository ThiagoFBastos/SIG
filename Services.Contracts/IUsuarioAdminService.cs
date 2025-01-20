using Domain.Entities.Users;
using Shared.Dtos;
using Shared.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IUsuarioAdminService
    {
        /**
         * Dados o email e senha do usu�rio cria um Json Web Token e o retorna 
         * 
         * @param loginDto s�o as credenciais do usu�rio
         * @throws UnauthorizedException se o email e/ou senha est�o incorretos
        */
        Task<string> Login(LoginUsuarioDto loginDto);

        /**
         * Cadastra o usu�rio no banco de dados dados email e senha
         * 
         * @param UsuarioAdminForCreateDto s�o os dados do usu�rio para a cria��o de uma nova inst�ncia
         * @throws BadRequestException se o email j� est� em uso
         */
        Task<Guid> CadastraUsuarioAdmin(UsuarioAdminForCreateDto usuarioAdminDto);

        /**
         * Deleta o usu�rio que possui o email fornecido 
         * 
         * @param email � o endere�o eletr�nico do usu�rio
         * @throws NotFoundException se o email n�o pertence a nenhum usu�rio admin
        */
        Task DeletarUsuarioAdmin(string email);

        /**
         * Altera a senha do usu�rio para a nova senha fornecida
         * 
         * @param id � o identificador �nico do usu�rio
         * @param  changePasswordDto s�o as senhas, antiga e nova, que o usu�rio fornece
         * @throws NotFoundException se o id n�o pertence a nenhum usu�rio admin
         * @throws UnauthorizedException se a senha antiga est� incorreta
        */
        Task AlterarUsuarioAdminSenha(Guid id, ChangeUsuarioPasswordDto changePasswordDto);

        /**
         * Obt�m a lista completa de usu�rios admin
        */
        Task<List<UsuarioAdminDto>> ObterListaDeUsuariosAdmin();
    }
}