using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Repositories;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class UsuarioAdminRepository: RepositoryBase<UsuarioAdmin>, IUsuarioAdminRepository
    {
        public UsuarioAdminRepository(RepositoryContext context): base(context)
        {
            
        }
    }
}