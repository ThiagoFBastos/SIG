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
         * Dados o email e senha do usuário cria um Json Web Token e o retorna 
         * 
         * @param loginDto são as credenciais do usuário
         * @throws UnauthorizedException se o email e/ou senha estão incorretos
        */
        Task<string> Login(LoginUsuarioDto loginDto);

        /**
         * Cadastra o usuário no banco de dados dados email e senha
         * 
         * @param UsuarioAdminForCreateDto são os dados do usuário para a criação de uma nova instância
         * @throws BadRequestException se o email já está em uso
         */
        Task<Guid> CadastraUsuarioAdmin(UsuarioAdminForCreateDto usuarioAdminDto);

        /**
         * Deleta o usuário que possui o email fornecido 
         * 
         * @param email é o endereço eletrônico do usuário
         * @throws NotFoundException se o email não pertence a nenhum usuário admin
        */
        Task DeletarUsuarioAdmin(string email);

        /**
         * Altera a senha do usuário para a nova senha fornecida
         * 
         * @param id é o identificador único do usuário
         * @param  changePasswordDto são as senhas, antiga e nova, que o usuário fornece
         * @throws NotFoundException se o id não pertence a nenhum usuário admin
         * @throws UnauthorizedException se a senha antiga está incorreta
        */
        Task AlterarUsuarioAdminSenha(Guid id, ChangeUsuarioPasswordDto changePasswordDto);

        /**
         * Obtém a lista completa de usuários admin
        */
        Task<List<UsuarioAdminDto>> ObterListaDeUsuariosAdmin();
    }
}