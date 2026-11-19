using FluentValidation;
using PasswordKeeper.ViewModels;

namespace PasswordKeeper.Validation;

public sealed class UnlockViewModelValidator : AbstractValidator<UnlockViewModel>
{
    public UnlockViewModelValidator()
    {
        RuleFor(vm => vm.MasterPassword)
            .NotEmpty().WithMessage("Master password is required");

        RuleFor(vm => vm.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(vm => vm.MasterPassword).WithMessage("Passwords don't match")
            .When(vm => vm.IsFirstRun);
    }
}
