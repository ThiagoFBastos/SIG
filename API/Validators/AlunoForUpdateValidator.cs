using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;
using Domain.Entities.Enums;

namespace API.Validators
{
    public class AlunoForUpdateValidator: AbstractValidator<AlunoForUpdateDto>
    {
        public AlunoForUpdateValidator()
        {
             RuleFor(a => a.AnoEscolar)
                .NotNull().WithMessage("o ano escolar é obrigatório")
                .Must(ano => Enum.IsDefined(typeof(Periodo), ano));

            RuleFor(a => a.Status)
                .NotNull().WithMessage("o status é necessário")
                .Must(status => Enum.IsDefined(typeof(StatusMatricula), status));

            RuleFor(a => a.Turno)
                .NotNull().WithMessage("o turno é necessário")
                .Must(turno => Enum.IsDefined(typeof(Turno), turno));

            RuleFor(a => a.Email)
                .NotNull().WithMessage("o email é obrigatório")
                .NotEmpty().WithMessage("o email não pode ser vazio")
                .Length(min: 1, max: 255).WithMessage("o email possui até 255 caracteres")
                .EmailAddress().WithMessage("o email deve ser válido");

            RuleFor(a => a.Celular)
                .NotNull().WithMessage("o celular é obrigatório")
                .Must(CustomValidations.CelularValido).WithMessage("o número do celular deve ser válido");
        }
    }
}