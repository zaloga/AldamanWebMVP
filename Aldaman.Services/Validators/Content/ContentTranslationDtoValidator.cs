using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Content;

public class ContentTranslationDtoValidator : AbstractValidator<ContentTranslationDto>
{
    public ContentTranslationDtoValidator(IStringLocalizer<ValidationResources> localizer)
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
            .MaximumLength(ContentTranslationEntity.TitleMaxLength)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, ContentTranslationEntity.TitleMaxLength]);

        RuleFor(x => x.Slug)
            .MaximumLength(ContentTranslationEntity.SlugMaxLength)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, ContentTranslationEntity.SlugMaxLength]);

        RuleFor(x => x.Perex)
            .MaximumLength(ContentTranslationEntity.PerexMaxLength)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, ContentTranslationEntity.PerexMaxLength]);
    }

    private static bool IsTranslationEmpty(ContentTranslationDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Title) &&
               string.IsNullOrWhiteSpace(dto.Slug) &&
               string.IsNullOrWhiteSpace(dto.Perex) &&
               string.IsNullOrWhiteSpace(dto.BodyHtml);
    }
}
