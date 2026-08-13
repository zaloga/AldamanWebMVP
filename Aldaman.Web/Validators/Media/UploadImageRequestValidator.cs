using Aldaman.Services.Resources;
using Aldaman.Web.Models.Media;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Validators.Media;

/// <summary>
/// Validator for UploadImageRequest ensuring file validity and positive target dimensions.
/// </summary>
public class UploadImageRequestValidator : AbstractValidator<UploadImageRequest>
{
    public UploadImageRequestValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage(localizer[ValidationResourceKeys.ImageFileRequired])
            .Must(file => file != null && file.Length > 0).WithMessage(localizer[ValidationResourceKeys.ImageFileEmpty]);

        RuleFor(x => x.TargetWidth)
            .GreaterThan(0).WithMessage(localizer[ValidationResourceKeys.TargetWidthGreaterThanZero]);

        When(x => x.TargetHeight.HasValue, () =>
        {
            RuleFor(x => x.TargetHeight!.Value)
                .GreaterThan(0).WithMessage(localizer[ValidationResourceKeys.TargetHeightGreaterThanZero]);
        });
    }
}
