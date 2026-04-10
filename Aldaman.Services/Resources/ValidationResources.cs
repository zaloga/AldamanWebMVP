namespace Aldaman.Services.Resources;

/// <summary>
/// Dummy class used to group validation resources.
/// </summary>
public sealed class ValidationResources
{
}

/// <summary>
/// Constants for validation resource keys to ensure type safety.
/// </summary>
public static class ValidationResourceKeys
{
    public const string NameRequired = nameof(NameRequired);
    public const string NameMaxLength = nameof(NameMaxLength);
    public const string EmailRequired = nameof(EmailRequired);
    public const string EmailInvalid = nameof(EmailInvalid);
    public const string EmailMaxLength = nameof(EmailMaxLength);
    public const string SubjectMaxLength = nameof(SubjectMaxLength);
    public const string MessageRequired = nameof(MessageRequired);
    public const string MessageMaxLength = nameof(MessageMaxLength);
    public const string PasswordRequired = nameof(PasswordRequired);
    public const string TitleMinLength = nameof(TitleMinLength);
    public const string TitleMaxLength = nameof(TitleMaxLength);
    public const string AltTextMinLength = nameof(AltTextMinLength);
    public const string AltTextMaxLength = nameof(AltTextMaxLength);
}
