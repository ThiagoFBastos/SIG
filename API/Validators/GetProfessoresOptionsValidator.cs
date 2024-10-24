using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;
using FluentValidation;
using Shared.Pagination;

namespace API.Validators
{
    public class GetProfessoresOptionsValidator: AbstractValidator<GetProfessoresOptions> 
    {
        public GetProfessoresOptionsValidator()
        {
            RuleFor(gpo => gpo.PrefixoCargo)
                .Length(min: 3, max: 50).WithMessage("o parâmetro \"prefixo_cargo\" possui entre 3 e 50 caracteres")
                .When(gpo => gpo.PrefixoCargo != null);

            RuleFor(gpo => gpo.PrefixoName)
                .Length(min: 3, max: 255).WithMessage("o parâmetro \"prefixo_name\" possui entre 3 e 255 caracteres")
                .When(gpo => gpo.PrefixoName != null);
            
            RuleFor(gpo => gpo.ComecarApartirDe)
                .GreaterThan(-1).WithMessage("o parâmetro \"comeco\" não pode ser negativo");

            RuleFor(gpo => gpo.LimiteDeResultados)
                .GreaterThan(0).WithMessage("o parâmetro \"limite\" é positivo");

            RuleFor(gpo => gpo.SalarioMinimo)
                .GreaterThanOrEqualTo(0).WithMessage("o parâmetro \"salario_minimo\" não é negativo");

            RuleFor(gpo => gpo.SalarioMaximo)
                .GreaterThanOrEqualTo(0).WithMessage("o parâmetro \"salario_maximo\" não é negativo")
                .Must((gpo, SalarioMaxio) => gpo.SalarioMinimo < SalarioMaxio).WithMessage("o parâmetro \"salario_minimo\" deve ser menor que \"salario_maximo\"");

            RuleFor(gpo => gpo.Status)
                .Must(status => status != null && Enum.IsDefined(typeof(StatusEmprego), status)).WithMessage("o parâmetro \"status\" deve ser válido")
                .When(gpo => gpo.Status != null);
        }
    } 
}