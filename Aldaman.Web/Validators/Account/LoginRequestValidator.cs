using Aldaman.Web.Models.Account;
using FluentValidation;

namespace Aldaman.Web.Validators.Account;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator() // TODO localizations
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mailová adresa je povinná.")
            .EmailAddress().WithMessage("Zadejte platnou e-mailovou adresu.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Heslo je povinné.");
    }
}
