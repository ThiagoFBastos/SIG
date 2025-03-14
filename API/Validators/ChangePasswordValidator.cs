using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class ChangePasswordValidator: AbstractValidator<ChangeUsuarioPasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(p => p.NewPassword)
                .NotNull().WithMessage("a nova senha é obrigatória")
                .Must(CustomValidations.PasswordValido).WithMessage("a senha deve conter pelo menos: uma letra minúscula, uma letra maiúscula, um digito e um caracter especial");

            RuleFor(p => p.OldPassword)
                .NotNull().WithMessage("a senha antiga é obrigatória");
        }
    }
}
