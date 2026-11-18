using FluentValidation;
using PasswordKeeper.ViewModels;

namespace PasswordKeeper.Validation;

public sealed class PasswordManagerViewModelValidator : AbstractValidator<PasswordManagerViewModel>
{
    public PasswordManagerViewModelValidator()
    {
        RuleFor(vm => vm.Name)
            .NotEmpty().WithMessage("Platform is required");

        RuleFor(vm => vm.Value)
            .NotEmpty().WithMessage("Password is required");
    }
}
