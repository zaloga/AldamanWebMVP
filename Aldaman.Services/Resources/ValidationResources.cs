namespace Aldaman.Services.Resources;

/// <summary>
/// Validation resources marker class for IStringLocalizer&lt;ValidationResources&gt; and constants for validation resource keys.
/// </summary>
public sealed class ValidationResources
{
    public const string EmailRequired = nameof(EmailRequired);
    public const string EmailOrPhoneRequired = nameof(EmailOrPhoneRequired);
    public const string EmailInvalid = nameof(EmailInvalid);
    public const string EmailOrPhoneMaxLength = nameof(EmailOrPhoneMaxLength);
    public const string MessageRequired = nameof(MessageRequired);
    public const string MessageMaxLength = nameof(MessageMaxLength);
    public const string PasswordRequired = nameof(PasswordRequired);
    public const string TitleMinLength = nameof(TitleMinLength);
    public const string TitleMaxLength = nameof(TitleMaxLength);
    public const string AltTextMinLength = nameof(AltTextMinLength);
    public const string AltTextMaxLength = nameof(AltTextMaxLength);
    public const string TitleRequiredIfTranslationNotEmpty = nameof(TitleRequiredIfTranslationNotEmpty);
    public const string SlugRequiredIfTranslationNotEmpty = nameof(SlugRequiredIfTranslationNotEmpty);
    public const string GenericMaxLength = nameof(GenericMaxLength);
    public const string ImageFileRequired = nameof(ImageFileRequired);
    public const string ImageFileEmpty = nameof(ImageFileEmpty);
    public const string TargetWidthGreaterThanZero = nameof(TargetWidthGreaterThanZero);
    public const string TargetHeightGreaterThanZero = nameof(TargetHeightGreaterThanZero);
    public const string OnlyOneContentGroupAllowedOnHomePage = nameof(OnlyOneContentGroupAllowedOnHomePage);
}
