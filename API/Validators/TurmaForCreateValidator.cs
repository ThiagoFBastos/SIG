using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;
using Domain.Entities.Enums;

namespace API.Validators
{
    public class TurmaForCreateValidator: AbstractValidator<TurmaForCreateDto>
    {
        public TurmaForCreateValidator()
        {
            RuleFor(t => t.ProfessorMatricula)
                .NotNull().WithMessage("a matrícula do professor é obrigatório")
                .NotEqual(Guid.Empty);

            RuleFor(t => t.Disciplina)
                .NotNull().WithMessage("a disciplina é obrigatória")
                .Length(min: 3, max: 50).WithMessage("a disciplina possui entre 3 e 50 caracteres");

            RuleFor(t => t.AnoEscolar)
                .NotNull().WithMessage("o ano escolar é obrigatório")
                .Must(anoEscolar => Enum.IsDefined(typeof(Periodo), anoEscolar)).WithMessage("o ano escolar é um código válido");

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