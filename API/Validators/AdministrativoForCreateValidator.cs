using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;
using Domain.Entities.Enums;

namespace API.Validators
{
    public class AdministrativoForCreateValidator: AbstractValidator<AdministrativoForCreateDto>
    {
        public AdministrativoForCreateValidator()
        {
            RuleFor(a => a.Cargo)
                .NotNull().WithMessage("o cargo é obrigatório")
                .Length(min: 3, max: 50).WithMessage("o cargo possui entre 3 e 50 caracteres");

            RuleFor(a => a.Salario)
                .NotNull().WithMessage("o salário é obrigatório")
                .GreaterThanOrEqualTo(0).WithMessage("o salário não pode ser negativo");

            RuleFor(a => a.Banco)
                .NotNull().WithMessage("o banco é obrigatório")
                .Length(min: 1, max: 50).WithMessage("o banco possui entre 1 e 50 caracteres");

            RuleFor(a => a.ContaCorrente)
                .NotNull().WithMessage("a conta corrente é obrigatória")
                .Length(min: 1, max: 50).WithMessage("a conta corrente possui entre 1 e 50 caracteres");

            RuleFor(e => e.Status)
                .NotNull().WithMessage("o status do emprego é obrigatório")
                .Must(status => Enum.IsDefined(typeof(StatusEmprego), status)).WithMessage("o status deve ser válido");

            RuleFor(e => e.HorarioInicioExpediente)
                .NotNull().WithMessage("o horário de inicio é obrigatório");

            RuleFor(e => e.HorarioFimExpediente)
                .NotNull().WithMessage("o horário de fim do expediente é obrigatório")
                .Must((a, horarioFimExpediente) => a.HorarioInicioExpediente < horarioFimExpediente).WithMessage("o horário de inicio é antes do horário de fim do expediente");

            RuleFor(e => e.CPF)
                .NotNull().WithMessage("o cpf é obrigatório")
                .Must(CustomValidations.CPFValido).WithMessage("o cpf deve ser válido");

            RuleFor(e => e.RG)
                .NotNull().WithMessage("o rg é obrigatório")
                .Must(CustomValidations.RGValido).WithMessage("o rg deve ser válido");

            RuleFor(e => e.NomeCompleto)
                .NotNull().WithMessage("o nome é obrigatório")
                .Length(min: 3, max: 255).WithMessage("o nome possui entre 3 e 255 caracteres");

            RuleFor(e => e.Email)
                .NotNull().WithMessage("o email é obrigatório")
                .Length(min: 1, max: 255).WithMessage("o email possui até 255 caracteres")
                .EmailAddress().WithMessage("o email deve ser válido");

            RuleFor(e => e.Celular)
                .NotNull().WithMessage("o celular é obrigatório")
                .Must(CustomValidations.CelularValido).WithMessage("o celular deve ser válido");

            RuleFor(e => e.DataNascimento)
                .NotNull().WithMessage("a data de nascimento é obrigatória")
                .LessThan(DateTime.Now).WithMessage("a data de nascimento é no passado");

            RuleFor(e => e.Sexo)
                .NotNull().WithMessage("o sexo é obrigatório")
                .Must(sexo => Enum.IsDefined(typeof(Sexo), sexo)).WithMessage("o sexo deve ser válido");
        }
    }
} 