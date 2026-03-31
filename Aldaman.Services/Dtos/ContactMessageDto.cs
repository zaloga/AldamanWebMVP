namespace Aldaman.Services.Dtos;

/// <summary>
/// Data representing a contact message submission.
/// </summary>
public class ContactMessageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string State { get; set; } = string.Empty; // Mapped from ContactMessageState (Pending, Handled, Spam, Failure)
}
