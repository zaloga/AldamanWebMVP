namespace Aldaman.Services.Resources;

/// <summary>
/// Constants for UI resource keys to ensure type safety.
/// </summary>
public static class UIResourceKeys
{
    // Layout & Navigation
    public const string Home = nameof(Home);
    public const string Logout = nameof(Logout);
    public const string Login = nameof(Login);
    public const string Admin = nameof(Admin);

    // Account & Auth
    public const string AccessDenied = nameof(AccessDenied);
    public const string AccessDeniedMessage = nameof(AccessDeniedMessage);
    public const string BackToHome = nameof(BackToHome);
    public const string WelcomeToAdmin = nameof(WelcomeToAdmin);
    public const string Email = nameof(Email);
    public const string EmailOrPhone = nameof(EmailOrPhone);
    public const string Password = nameof(Password);
    public const string RememberMe = nameof(RememberMe);
    public const string ForgotPassword = nameof(ForgotPassword);
    public const string SignInButton = nameof(SignInButton);

    // Blog
    public const string Blog = nameof(Blog);
    public const string BlogSubtitle = nameof(BlogSubtitle);
    public const string Article = nameof(Article);
    public const string Administrator = nameof(Administrator);

    // Contact
    public const string Contact = nameof(Contact);
    public const string ContactMe = nameof(ContactMe);
    public const string ContactSubtitle = nameof(ContactSubtitle);
    public const string FullName = nameof(FullName);
    public const string Subject = nameof(Subject);
    public const string Message = nameof(Message);
    public const string SendMessage = nameof(SendMessage);
    public const string MessageSent = nameof(MessageSent);
    public const string ThankYouForMessage = nameof(ThankYouForMessage);
    public const string MessageSentConfirmation = nameof(MessageSentConfirmation);

    // Common & Errors
    public const string Error = nameof(Error);
    public const string ErrorOccurred = nameof(ErrorOccurred);
    public const string RequestId = nameof(RequestId);
    public const string SectionNoContent = nameof(SectionNoContent);
    public const string GenerateSlug = nameof(GenerateSlug);
}
