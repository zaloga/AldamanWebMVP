namespace Aldaman.Services.Resources;

/// <summary>
/// Constants for validation resource keys to ensure type safety.
/// </summary>
public static class ValidationResourceKeys
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
}
