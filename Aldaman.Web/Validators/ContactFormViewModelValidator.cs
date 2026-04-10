using Aldaman.Persistence.Entities;
using Aldaman.Services.Resources;
using Aldaman.Web.ViewModels;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators;

public class ContactFormViewModelValidator : AbstractValidator<ContactFormViewModel>
{
    public ContactFormViewModelValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(localizer[ValidationResourceKeys.NameRequired])
            .MaximumLength(ContactMessageEntity.NameMaxLength).WithMessage(localizer[ValidationResourceKeys.NameMaxLength, ContactMessageEntity.NameMaxLength]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer[ValidationResourceKeys.EmailRequired])
            .EmailAddress().WithMessage(localizer[ValidationResourceKeys.EmailInvalid])
            .MaximumLength(ContactMessageEntity.EmailMaxLength).WithMessage(localizer[ValidationResourceKeys.EmailMaxLength, ContactMessageEntity.EmailMaxLength]);

        RuleFor(x => x.Subject)
            .MaximumLength(ContactMessageEntity.SubjectMaxLength).WithMessage(localizer[ValidationResourceKeys.SubjectMaxLength, ContactMessageEntity.SubjectMaxLength]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(localizer[ValidationResourceKeys.MessageRequired])
            .MaximumLength(ContactMessageEntity.MessageMaxLength).WithMessage(localizer[ValidationResourceKeys.MessageMaxLength, ContactMessageEntity.MessageMaxLength]);
    }
}
