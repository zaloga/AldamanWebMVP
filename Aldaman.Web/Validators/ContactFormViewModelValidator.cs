using Aldaman.Web.ViewModels;
using Aldaman.Persistence.Entities;
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
            .MaximumLength(ContactMessageEntity.NameMaxLength).WithMessage(localizer["NameMaxLength", ContactMessageEntity.NameMaxLength]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalid"])
            .MaximumLength(ContactMessageEntity.EmailMaxLength).WithMessage(localizer["EmailMaxLength", ContactMessageEntity.EmailMaxLength]);

        RuleFor(x => x.Subject)
            .MaximumLength(ContactMessageEntity.SubjectMaxLength).WithMessage(localizer["SubjectMaxLength", ContactMessageEntity.SubjectMaxLength]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(localizer["MessageRequired"])
            .MaximumLength(ContactMessageEntity.MessageMaxLength).WithMessage(localizer["MessageMaxLength", ContactMessageEntity.MessageMaxLength]);
    }
}
