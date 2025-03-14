using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Pagination.Abstract
{
    public abstract record class GetFuncionariosOptions: GetIntegranteOptions
    {
        [FromQuery(Name = "prefixo_cargo")]
        public string? PrefixoCargo { get; init; }

        [FromQuery(Name = "status")]
        public int? Status { get; init; }

        [FromQuery(Name = "salario_minimo")]
        public decimal SalarioMinimo { get; init; } = 0;

        [FromQuery(Name = "salario_maximo")]
        public decimal SalarioMaximo { get; init; } = decimal.MaxValue;
    }
}