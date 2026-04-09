using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.Media;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aldaman.Services.Validators.Media;

public class UpdateMediaAssetDtoValidator : AbstractValidator<UpdateMediaAssetDto>
{
    public UpdateMediaAssetDtoValidator(IStringLocalizer<UpdateMediaAssetDtoValidator> localizer)
    {
        RuleFor(x => x.Title)
            .MinimumLength(MediaAssetEntity.TitleMinLength).WithMessage(localizer["TitleMinLength", MediaAssetEntity.TitleMinLength])
            .MaximumLength(MediaAssetEntity.TitleMaxLength).WithMessage(localizer["TitleMaxLength", MediaAssetEntity.TitleMaxLength]);

        RuleFor(x => x.AltText)
            .MinimumLength(MediaAssetEntity.AltTextMinLength).WithMessage(localizer["AltTextMinLength", MediaAssetEntity.AltTextMinLength])
            .MaximumLength(MediaAssetEntity.AltTextMaxLength).WithMessage(localizer["AltTextMaxLength", MediaAssetEntity.AltTextMaxLength]);
    }
}
