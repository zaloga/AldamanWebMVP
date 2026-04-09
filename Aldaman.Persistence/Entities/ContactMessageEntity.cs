using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

public class ContactMessageEntity : BaseEntity
{
    public const int LanguageCodeMaxLength = 16;
    public const int NameMaxLength = 128;
    public const int EmailMaxLength = 256;
    public const int PhoneMaxLength = 32;
    public const int SubjectMaxLength = 256;
    public const int FailureReasonMaxLength = 1024;
    public const int ClientIpMaxLength = 64;
    public const int UserAgentMaxLength = 1024;
    public const int StateMaxLength = 16;

    public string LanguageCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string? Subject { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool ConsentToProcessing { get; set; }

    public ContactMessageState State { get; set; } = ContactMessageState.Pending;

    public DateTime? SentAtUtc { get; set; }

    public string? FailureReason { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }
}
