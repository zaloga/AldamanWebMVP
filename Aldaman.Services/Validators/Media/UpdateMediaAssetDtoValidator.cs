using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Media;

public class UpdateMediaAssetDtoValidator : AbstractValidator<UpdateMediaAssetDto>
{
    public UpdateMediaAssetDtoValidator(IStringLocalizer<ValidationResources> localizer)
    {
        RuleFor(x => x.Title)
            .MinimumLength(MediaAssetEntity.TitleMinLength).WithMessage(localizer[ValidationResourceKeys.TitleMinLength, MediaAssetEntity.TitleMinLength])
            .MaximumLength(MediaAssetEntity.TitleMaxLength).WithMessage(localizer[ValidationResourceKeys.TitleMaxLength, MediaAssetEntity.TitleMaxLength]);

        RuleFor(x => x.AltText)
            .MinimumLength(MediaAssetEntity.AltTextMinLength).WithMessage(localizer[ValidationResourceKeys.AltTextMinLength, MediaAssetEntity.AltTextMinLength])
            .MaximumLength(MediaAssetEntity.AltTextMaxLength).WithMessage(localizer[ValidationResourceKeys.AltTextMaxLength, MediaAssetEntity.AltTextMaxLength]);
    }
}
