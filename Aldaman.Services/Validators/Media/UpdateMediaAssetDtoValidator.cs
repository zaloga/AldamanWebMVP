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
            .MinimumLength(MediaAssetEntity.TitleDefaultMinLength).WithMessage(localizer[ValidationResources.TitleMinLength, MediaAssetEntity.TitleDefaultMinLength])
            .MaximumLength(MediaAssetEntity.TitleDefaultMaxLength).WithMessage(localizer[ValidationResources.TitleMaxLength, MediaAssetEntity.TitleDefaultMaxLength]);

        RuleFor(x => x.AltTextDefault)
            .MinimumLength(MediaAssetEntity.AltTextDefaultMinLength).WithMessage(localizer[ValidationResources.AltTextMinLength, MediaAssetEntity.AltTextDefaultMinLength])
            .MaximumLength(MediaAssetEntity.AltTextDefaultMaxLength).WithMessage(localizer[ValidationResources.AltTextMaxLength, MediaAssetEntity.AltTextDefaultMaxLength]);
    }
}
