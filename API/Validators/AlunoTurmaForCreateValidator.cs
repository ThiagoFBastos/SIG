using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class AlunoTurmaForCreateValidator: AbstractValidator<AlunoTurmaForCreateDto>
    {
        public AlunoTurmaForCreateValidator()
        {
             RuleFor(at => at.AlunoMatricula)
                .NotNull().WithMessage("a matrícula do aluno é obrigatória")
                .NotEqual(Guid.Empty).WithMessage("a matrícula do aluno é obrigatória");

            RuleFor(at => at.TurmaCodigo)
                .NotNull().WithMessage("o código da turma é obrigatório")
                .NotEqual(Guid.Empty).WithMessage("o código da turma é obrigatório");
        }
    }
}