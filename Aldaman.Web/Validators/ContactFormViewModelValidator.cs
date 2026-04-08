using Aldaman.Web.ViewModels;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators;

public class ContactFormViewModelValidator : AbstractValidator<ContactFormViewModel>
{
    public ContactFormViewModelValidator(IStringLocalizer<ContactFormViewModelValidator> localizer)
    {
        // TODO resx translates via some constants ot proxy class to avoid hardcoding the type name in the resx file
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(localizer["NameRequired"])
            .MaximumLength(100).WithMessage(localizer["NameMaxLength", 100]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalid"])
            .MaximumLength(200).WithMessage(localizer["EmailMaxLength", 200]);

        RuleFor(x => x.Subject)
            .MaximumLength(200).WithMessage(localizer["SubjectMaxLength", 200]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(localizer["MessageRequired"])
            .MaximumLength(2000).WithMessage(localizer["MessageMaxLength", 2000]);
    }
}
