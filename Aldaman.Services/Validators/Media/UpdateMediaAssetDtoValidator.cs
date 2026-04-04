using Aldaman.Services.Dtos.Media;
using FluentValidation;

namespace Aldaman.Services.Validators.Media;

public class UpdateMediaAssetDtoValidator : AbstractValidator<UpdateMediaAssetDto>
{
    public UpdateMediaAssetDtoValidator() // TODO min length for alt text and title from constants from entity config
    {
        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.AltText)
            .MaximumLength(500).WithMessage("Alt text cannot exceed 500 characters.");
    }
}
