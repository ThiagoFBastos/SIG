using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;
using Domain.Entities.Enums;
using System.Text.RegularExpressions;
using FluentValidation.Validators;

namespace API.Validators
{
    public class AlunoForCreateValidator: AbstractValidator<AlunoForCreateDto>
    {
        public AlunoForCreateValidator()
        {
            RuleFor(a => a.AnoEscolar)
                .NotNull().WithMessage("o ano escolar é obrigatório")
                .Must(ano => Enum.IsDefined(typeof(Periodo), ano)).WithMessage("o ano_escolar deve ser válido");

            RuleFor(a => a.Status)
                .NotNull().WithMessage("o status é necessário")
                .Must(status => Enum.IsDefined(typeof(StatusMatricula), status));

            RuleFor(a => a.Turno)
                .NotNull().WithMessage("o turno é necessário")
                .Must(turno => Enum.IsDefined(typeof(Turno), turno));

            RuleFor(a => a.CPF)
                .NotNull().WithMessage("o cpf é obrigatório")
                .Must(CustomValidations.CPFValido).WithMessage("o cpf deve ser válido");
 
            RuleFor(a => a.RG)
                .NotNull().WithMessage("o rg é obrigatório")
                .Must(CustomValidations.RGValido).WithMessage("o rg contém entre 8 e 9 digitos");

            RuleFor(a => a.NomeCompleto)
                .NotNull().WithMessage("o nome completo é obrigatório")
                .Length(min: 3, max: 255).WithMessage("o nome completo possui entre 3 e 255 caracteres");

            RuleFor(a => a.Email)
                .NotNull().WithMessage("o email é obrigatório")
                .NotEmpty().WithMessage("o email não pode ser vazio")
                .Length(min: 1, max: 255).WithMessage("o email possui até 255 caracteres")
                .EmailAddress().WithMessage("o email deve ser válido");

            RuleFor(a => a.Celular)
                .NotNull().WithMessage("o celular é obrigatório")
                .Must(CustomValidations.CelularValido).WithMessage("o número do celular deve ser válido");

            RuleFor(a => a.DataNascimento)
                .NotNull().WithMessage("a data de nascimento é obrigatória")
                .LessThan(DateTime.Now).WithMessage("a data de nascimento está no passado");

            RuleFor(a => a.Sexo)
                .NotNull().WithMessage("o sexo é obrigatório")
                .Must(sexo => Enum.IsDefined(typeof(Sexo), sexo));

            RuleFor(a => a.Endereco)
                .NotNull().WithMessage("o endereço não pode ser nulo")
                .SetValidator(new EnderecoForCreateValidator());
        }
    }
}