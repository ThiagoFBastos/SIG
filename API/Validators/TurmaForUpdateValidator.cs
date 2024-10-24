using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class TurmaForUpdateValidator: AbstractValidator<TurmaForUpdateDto>
    {
        public TurmaForUpdateValidator()
        {
             RuleFor(t => t.ProfessorMatricula)
                .NotNull().WithMessage("a matrícula do professor é obrigatório")
                .NotEqual(Guid.Empty);

            RuleFor(t => t.DataInicio)
                .NotNull().WithMessage("a data de inicio da turma é obrigatória");

            RuleFor(t => t.DataFim)
                .NotNull().WithMessage("a data de término da turma é obrigatória")
                .Must((t, DataFim) => t.DataInicio < DataFim).WithMessage("a data de incio da turma é antes da data de término");

            RuleFor(t => t.HorarioAulaInicio)
                .NotNull().WithMessage("o horário de inicio da aula é obrigatório");

            RuleFor(t => t.DataFim)
                .NotNull().WithMessage("o horário de inicio da aula é obrigatório")
                .Must((t, DataFim) => t.DataInicio < DataFim).WithMessage("o horário de inicio é antes do horário de término");
        }
    }
}