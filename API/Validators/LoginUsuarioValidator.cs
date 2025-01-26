using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class LoginUsuarioValidator: AbstractValidator<LoginUsuarioDto>
    {
        public LoginUsuarioValidator() 
        {
            RuleFor(lu => lu.Email)
                .NotNull().WithMessage("o email não pode ser nulo")
                .EmailAddress().WithMessage("o email deve ser válido");

            RuleFor(lu => lu.Password)
                .NotNull().WithMessage("a senha não pode ser nula");
        }
    }
}
