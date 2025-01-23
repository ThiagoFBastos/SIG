using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Shared.Dtos.Abstract;

namespace Shared.Dtos
{
    public record class UsuarioAdminDto: UsuarioDto
    {
        public bool Match(object? admin)
        {
            var usuarioAdmin = admin as UsuarioAdmin;

            if (usuarioAdmin is null)
                return false;

            return Id == usuarioAdmin.Id && Email == usuarioAdmin.Email;
        }
    }
}