using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class AlunoTurmaForUpdateValidator: AbstractValidator<AlunoTurmaForUpdateDto>
    {
        public AlunoTurmaForUpdateValidator()
        {
            RuleFor(at => at.TurmaCodigo)
                .NotNull().WithMessage("o código da turma é obrigatório")
                .NotEqual(Guid.Empty).WithMessage("o código da turma é obrigatório");

            RuleFor(at => at.Nota)
                .GreaterThanOrEqualTo(0).WithMessage("a nota do aluno é maior ou igual a zero");
        }
    }
}