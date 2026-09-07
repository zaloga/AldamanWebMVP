using Aldaman.Persistence.Entities;
using Aldaman.Services.Resources;
using Aldaman.Web.ViewModels;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators.ContactForm;

public class ContactFormViewModelValidator : AbstractValidator<ContactFormViewModel>
{
    public ContactFormViewModelValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.EmailOrPhone)
            .NotEmpty().WithMessage(localizer[ValidationResources.EmailOrPhoneRequired])
            .MaximumLength(ContactMessageEntity.EmailOrPhoneMaxLength).WithMessage(localizer[ValidationResources.EmailOrPhoneMaxLength, ContactMessageEntity.EmailOrPhoneMaxLength]);

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(localizer[ValidationResources.MessageRequired])
            .MaximumLength(ContactMessageEntity.MessageMaxLength).WithMessage(localizer[ValidationResources.MessageMaxLength, ContactMessageEntity.MessageMaxLength]);
    }
}
