using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Page;

public class ContentPageTranslationDtoValidator : AbstractValidator<ContentPageTranslationDto>
{
    public ContentPageTranslationDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(localizer[ValidationResourceKeys.TitleRequiredIfTranslationNotEmpty])
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage(localizer[ValidationResourceKeys.SlugRequiredIfTranslationNotEmpty])
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Title)
            .MaximumLength(256)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, 256]);

        RuleFor(x => x.Slug)
            .MaximumLength(256)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, 256]);
    }

    private bool IsTranslationEmpty(ContentPageTranslationDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Title) &&
               string.IsNullOrWhiteSpace(dto.Slug) &&
               string.IsNullOrWhiteSpace(dto.BodyHtml);
    }
}

public class ContentPageEditDtoValidator : AbstractValidator<ContentPageEditDto>
{
    public ContentPageEditDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.PageKey)
            .NotEmpty()
            .WithMessage(localizer[ValidationResourceKeys.PageKeyRequired])
            .MaximumLength(256)
            .WithMessage(localizer[ValidationResourceKeys.PageKeyMaxLength, 256]);

        RuleForEach(x => x.Translations).SetValidator(new ContentPageTranslationDtoValidator(localizer));
    }
}
