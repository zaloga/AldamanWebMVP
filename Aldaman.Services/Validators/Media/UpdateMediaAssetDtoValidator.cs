using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.Media;
using FluentValidation;

namespace Aldaman.Services.Validators.Media;

public class UpdateMediaAssetDtoValidator : AbstractValidator<UpdateMediaAssetDto>
{
    public UpdateMediaAssetDtoValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(MediaAssetEntity.TitleMinLength).WithMessage($"Title must be at least {MediaAssetEntity.TitleMinLength} characters.")
            .MaximumLength(MediaAssetEntity.TitleMaxLength).WithMessage($"Title cannot exceed {MediaAssetEntity.TitleMaxLength} characters.");

        RuleFor(x => x.AltText)
            .MinimumLength(MediaAssetEntity.AltTextMinLength).WithMessage($"Alt text must be at least {MediaAssetEntity.AltTextMinLength} characters.")
            .MaximumLength(MediaAssetEntity.AltTextMaxLength).WithMessage($"Alt text cannot exceed {MediaAssetEntity.AltTextMaxLength} characters.");
    }
}
