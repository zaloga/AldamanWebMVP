using Aldaman.Services.Dtos.Page;
using FluentValidation;

namespace Aldaman.Services.Validators.Page;

public class ContentPageTranslationDtoValidator : AbstractValidator<ContentPageTranslationDto>
{
    public ContentPageTranslationDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required if you provide any content for this translation.")
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required if you provide any content for this translation.")
            .When(x => !IsTranslationEmpty(x));

        RuleFor(x => x.Title)
            .MaximumLength(256);

        RuleFor(x => x.Slug)
            .MaximumLength(256);
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
    public ContentPageEditDtoValidator()
    {
        RuleFor(x => x.PageKey)
            .NotEmpty()
            .MaximumLength(256);

        RuleForEach(x => x.Translations).SetValidator(new ContentPageTranslationDtoValidator());
    }
}
