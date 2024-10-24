using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Pagination;
using FluentValidation;
using Domain.Entities.Enums;

namespace API.Validators
{
    public class GetAdministrativosOptionsValidator: AbstractValidator<GetAdministrativosOptions>
    {
        public GetAdministrativosOptionsValidator()
        {
            RuleFor(gao => gao.PrefixoCargo)
                .Length(min: 3, max: 50).WithMessage("o parâmetro \"prefixo_cargo\" possui entre 3 e 50 caracteres")
                .When(gao => gao.PrefixoCargo != null);

            RuleFor(gao => gao.PrefixoName)
                .Length(min: 3, max: 255).WithMessage("o parâmetro \"prefixo_name\" possui entre 3 e 255 caracteres")
                .When(gao => gao.PrefixoName != null);
            
            RuleFor(gao => gao.ComecarApartirDe)
                .GreaterThan(-1).WithMessage("o parâmetro \"comeco\" não pode ser negativo");

            RuleFor(gao => gao.LimiteDeResultados)
                .GreaterThan(0).WithMessage("o parâmetro \"limite\" é positivo");

            RuleFor(gao => gao.SalarioMinimo)
                .GreaterThanOrEqualTo(0).WithMessage("o parâmetro \"salario_minimo\" não é negativo");

            RuleFor(gao => gao.SalarioMaximo)
                .GreaterThanOrEqualTo(0).WithMessage("o parâmetro \"salario_maximo\" não é negativo")
                .Must((gao, SalarioMaxio) => gao.SalarioMinimo < SalarioMaxio).WithMessage("o parâmetro \"salario_minimo\" deve ser menor que \"salario_maximo\"");

            RuleFor(gao => gao.Status)
                .Must(status => status != null && Enum.IsDefined(typeof(StatusEmprego), status)).WithMessage("o parâmetro \"status\" deve ser válido")
                .When(gao => gao.Status != null);
        }
    }
}