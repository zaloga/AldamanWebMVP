using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

public class ContactMessageEntity : BaseEntity
{
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
