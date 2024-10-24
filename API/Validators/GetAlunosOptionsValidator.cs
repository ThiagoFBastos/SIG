using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;
using FluentValidation;
using Shared.Pagination;

namespace API.Validators
{
    public class GetAlunosOptionsValidator: AbstractValidator<GetAlunosOptions>
    {
        public GetAlunosOptionsValidator()
        {
            RuleFor(gao => gao.AnoEscolar)
                .Must(anoEscolar => anoEscolar != null && Enum.IsDefined(typeof(Periodo), anoEscolar))
                .When(gao => gao.AnoEscolar != null);

            RuleFor(gao => gao.Turno)
                .Must(turno => turno != null && Enum.IsDefined(typeof(Enum), turno))
                .When(gao => gao.Turno != null);

            RuleFor(gao => gao.PrefixoName)
                .Length(min: 3, max: 255).WithMessage("o parâmetro \"prefixo_name\" possui entre 3 e 255 caracteres")
                .When(gao => gao.PrefixoName != null);

            RuleFor(gao => gao.ComecarApartirDe)
                .GreaterThan(-1).WithMessage("o parâmetro \"comeco\" não pode ser negativo");

            RuleFor(gao => gao.LimiteDeResultados)
                .GreaterThan(0).WithMessage("o parâmetro \"limite\" é positivo");
        }
    }
}