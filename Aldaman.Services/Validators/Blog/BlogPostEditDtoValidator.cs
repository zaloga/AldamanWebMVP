using Aldaman.Services.Dtos.Blog;
using FluentValidation;

namespace Aldaman.Services.Validators.Blog;

public class BlogPostTranslationDtoValidator : AbstractValidator<BlogPostTranslationDto>
{
    public BlogPostTranslationDtoValidator()
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

        RuleFor(x => x.Perex)
            .MaximumLength(1024);
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
    public BlogPostEditDtoValidator()
    {
        RuleForEach(x => x.Translations).SetValidator(new BlogPostTranslationDtoValidator());
    }
}
