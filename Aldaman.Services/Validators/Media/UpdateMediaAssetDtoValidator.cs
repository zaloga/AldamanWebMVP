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
        RuleFor(x => x.TitleDefault)
            .MinimumLength(MediaAssetEntity.TitleDefaultMinLength).WithMessage(localizer[ValidationResourceKeys.TitleMinLength, MediaAssetEntity.TitleDefaultMinLength])
            .MaximumLength(MediaAssetEntity.TitleDefaultMaxLength).WithMessage(localizer[ValidationResourceKeys.TitleMaxLength, MediaAssetEntity.TitleDefaultMaxLength]);

        RuleFor(x => x.AltTextDefault)
            .MinimumLength(MediaAssetEntity.AltTextDefaultMinLength).WithMessage(localizer[ValidationResourceKeys.AltTextMinLength, MediaAssetEntity.AltTextDefaultMinLength])
            .MaximumLength(MediaAssetEntity.AltTextDefaultMaxLength).WithMessage(localizer[ValidationResourceKeys.AltTextMaxLength, MediaAssetEntity.AltTextDefaultMaxLength]);
    }
}
