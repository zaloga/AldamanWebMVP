using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.ContactMessage;

/// <summary>
/// Data representing a contact message submission.
/// </summary>
public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public ContactMessageState State { get; set; }
}
