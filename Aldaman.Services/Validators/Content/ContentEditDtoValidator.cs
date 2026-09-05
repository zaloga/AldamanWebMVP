using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Content;

public class ContentEditDtoValidator : AbstractValidator<ContentEditDto>
{
    public ContentEditDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleForEach(x => x.Translations).SetValidator(new ContentTranslationDtoValidator(localizer));
    }
}
