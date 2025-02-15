using FluentValidation;
using Shared.Dtos;

namespace API.Validators
{
    public class AlunoTurmaChangeNotaValidator: AbstractValidator<AlunoTurmaChangeNotaDto>
    {
        public AlunoTurmaChangeNotaValidator()
        {
            RuleFor(at => at.Nota)
                .NotNull().WithMessage("a nota é obrigatória")
                .GreaterThanOrEqualTo(0).WithMessage("a nota não pode ser menor que 0")
                .LessThanOrEqualTo(10).WithMessage("a nota não pode ser maior que 10")
                .WithName("nota");
        }
    }
}
