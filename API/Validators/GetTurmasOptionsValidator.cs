using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;
using FluentValidation;
using Shared.Pagination;

namespace API.Validators
{
    public class GetTurmasOptionsValidator: AbstractValidator<GetTurmasOptions>
    {
        public GetTurmasOptionsValidator()
        {
            RuleFor(gto => gto.PrefixoDisciplina)
                .Length(min: 3, max: 50).WithMessage("o parâmetro \"prefixo_disciplina\" possui entre 3 e 50 caracteres")
                .When(gto => gto.PrefixoDisciplina is not null);

            RuleFor(gto => gto.AnoEscolar)
                .Must(anoEscolar => anoEscolar != null && Enum.IsDefined(typeof(Periodo), anoEscolar)).WithMessage("o parâmetro \"ano_escolar\" deve ser válido")
                .When(gto => gto.AnoEscolar is not null);

            RuleFor(gto => gto.AteData)
                .Must((gto, ateData) => gto.ApartirData < ateData).WithMessage("a data de inicio deve ser antes da data de término");

            RuleFor(gto => gto.ComecarApartirDe)
                .GreaterThan(-1).WithMessage("o parâmetro \"comeco\" é não-negativo");

            RuleFor(gto => gto.LimiteDeResultados)
                .GreaterThanOrEqualTo(1).WithMessage("o parÂmetro \"limite\" deve ser positivo");
        }
    }
}