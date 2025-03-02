using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination
{
    public record class GetAdministrativoOptions
    {
        [FromQuery(Name = "incluir_endereco")]
        public bool IncluirEndereco { get; init; } = false;
    }
}