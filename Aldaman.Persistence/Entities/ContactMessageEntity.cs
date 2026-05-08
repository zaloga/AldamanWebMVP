using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

public class ContactMessageEntity : BaseEntityCreatableSoftDel
{
    public const int EmailOrPhoneMaxLength = 256;
    public const int SubjectMaxLength = 256;
    public const int MessageMaxLength = 2048;
    public const int FailureReasonMaxLength = 1024;
    public const int ClientIpMaxLength = 64;
    public const int UserAgentMaxLength = 1024;
    public const int StateMaxLength = 16;

    public string EmailOrPhone { get; set; } = string.Empty;

    public string Subject { get; set; } = "Kontakt z webu Aldaman";

    public string Message { get; set; } = string.Empty;

    public ContactMessageState State { get; set; } = ContactMessageState.Pending;
    
    public DateTime? SentAtUtc { get; set; }

    public string? FailureReason { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }
}
