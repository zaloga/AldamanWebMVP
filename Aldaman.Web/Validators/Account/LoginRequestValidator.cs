using Aldaman.Web.Models.Account;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators.Account;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(IStringLocalizer<LoginRequestValidator> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalid"]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"]);
    }
}
