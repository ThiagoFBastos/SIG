using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Shared.Dtos;
using Domain.Entities.Enums;

namespace API.Validators
{
    public class EnderecoForCreateValidator: AbstractValidator<EnderecoForCreateDto>
    {
        public EnderecoForCreateValidator()
        {
            RuleFor(e => e.Cidade)
                .NotNull().WithMessage("a cidade é necessária")
                .Length(min: 2, max: 50).WithMessage("a cidade possui entre 1 e 50 caracteres");

            RuleFor(e => e.Estado)
                .NotNull().WithMessage("o estado é necessário")
                .Must(estado => Enum.IsDefined(typeof(Estado), estado)).WithMessage("o estado é um código válido");

            RuleFor(e => e.CEP)
                .NotNull().WithMessage("o cep é necessário")
                .Length(8).WithMessage("o cep possui 8 digitos");

            RuleFor(e => e.Rua)
                .NotNull().WithMessage("a rua é necessária")
                .Length(min: 3, max: 300).WithMessage("a rua possui entre 3 e 300 caracteres");

            RuleFor(e => e.Casa)
                .NotNull().WithMessage("o número da casa é necessário")
                .GreaterThanOrEqualTo(1).WithMessage("o número da casa é positivo");

            RuleFor(e => e.Complemento)
                .Length(min: 1, max: 50).WithMessage("o complemento possui até 50 caracteres")
                .When(e => e.Complemento is not null);
        }
    }
}