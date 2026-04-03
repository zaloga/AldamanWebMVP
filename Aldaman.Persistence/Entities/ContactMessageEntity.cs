using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

public class ContactMessageEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

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

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    // Navigation properties
    public virtual AppUser? DeletedByUser { get; set; }
    public virtual AppUser? UpdatedByUser { get; set; }
}
