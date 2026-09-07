using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.ContentGroup;

public class ContentGroupTranslationDtoValidator : AbstractValidator<ContentGroupTranslationDto>
{
    public ContentGroupTranslationDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(localizer[ValidationResources.TitleRequiredIfTranslationNotEmpty])
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage(localizer[ValidationResources.SlugRequiredIfTranslationNotEmpty])
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Title)
            .MaximumLength(ContentGroupTranslationEntity.TitleMaxLength)
            .WithMessage(localizer[ValidationResources.GenericMaxLength, ContentGroupTranslationEntity.TitleMaxLength]);

        RuleFor(x => x.Slug)
            .MaximumLength(ContentGroupTranslationEntity.SlugMaxLength)
            .WithMessage(localizer[ValidationResources.GenericMaxLength, ContentGroupTranslationEntity.SlugMaxLength]);
    }

    private static bool IsTranslationEmpty(ContentGroupTranslationDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Title) &&
               string.IsNullOrWhiteSpace(dto.Slug);
    }
}

public class ContentGroupEditDtoValidator : AbstractValidator<ContentGroupEditDto>
{
    public ContentGroupEditDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleForEach(x => x.Translations).SetValidator(new ContentGroupTranslationDtoValidator(localizer));
    }
}
