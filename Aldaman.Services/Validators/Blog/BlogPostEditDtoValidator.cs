using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Blog;

public class BlogPostTranslationDtoValidator : AbstractValidator<BlogPostTranslationDto>
{
    public BlogPostTranslationDtoValidator(IStringLocalizer<ValidationResources> localizer)
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

        RuleFor(x => x.Perex)
            .MaximumLength(1024)
            .WithMessage(localizer[ValidationResourceKeys.GenericMaxLength, 1024]);
    }

    private bool IsTranslationEmpty(BlogPostTranslationDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Title) &&
               string.IsNullOrWhiteSpace(dto.Slug) &&
               string.IsNullOrWhiteSpace(dto.Perex) &&
               string.IsNullOrWhiteSpace(dto.BodyHtml);
    }
}

public class BlogPostEditDtoValidator : AbstractValidator<BlogPostEditDto>
{
    public BlogPostEditDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleForEach(x => x.Translations).SetValidator(new BlogPostTranslationDtoValidator(localizer));
    }
}
