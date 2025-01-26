using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class UsuarioAdminForCreateValidator: AbstractValidator<UsuarioAdminForCreateDto>
    {
        public UsuarioAdminForCreateValidator()
        {
            RuleFor(ua => ua.Email)
                .NotNull().WithMessage("o email é obrigatório")
                .EmailAddress().WithMessage("o email deve ser válido");

            RuleFor(ua => ua.Password)
                .NotNull().WithMessage("a senha é obrigatória");
        }
    }
}
