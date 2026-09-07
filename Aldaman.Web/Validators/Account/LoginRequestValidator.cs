using Aldaman.Web.Models.Account;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators.Account;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer[ValidationResources.EmailRequired])
            .EmailAddress().WithMessage(localizer[ValidationResources.EmailInvalid]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer[ValidationResources.PasswordRequired]);
    }
}
