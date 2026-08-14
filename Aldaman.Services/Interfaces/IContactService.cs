using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for handling contact messages.
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Submits a message from the public web form.
    /// </summary>
    Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent, CancellationToken ct = default);

    /// <summary>
    /// Lists messages with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query, bool filterDeleted = false, CancellationToken ct = default);

    /// <summary>
    /// Marks a message as handled.
    /// </summary>
    Task MarkAsHandledAsync(Guid id, CancellationToken ct = default);
    
    /// <summary>
    /// Deletes a message from the system (soft delete).
    /// </summary>
    Task DeleteMessageAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Restores a soft-deleted message.
    /// </summary>
    Task RestoreMessageAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Permanently deletes a message from the system.
    /// </summary>
    Task HardDeleteMessageAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets a contact message by its ID.
    /// </summary>
    Task<ContactMessageDto?> GetMessageByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets the most recent contact messages.
    /// </summary>
    Task<IEnumerable<ContactMessageDto>> GetRecentMessagesAsync(int count = 5, CancellationToken ct = default);
}
