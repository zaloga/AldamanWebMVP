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
    Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent);

    /// <summary>
    /// Lists messages with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query);

    /// <summary>
    /// Marks a message as handled.
    /// </summary>
    Task MarkAsHandledAsync(Guid id);
    
    /// <summary>
    /// Deletes a message from the system (soft delete).
    /// </summary>
    Task DeleteMessageAsync(Guid id);

    /// <summary>
    /// Gets the most recent contact messages.
    /// </summary>
    Task<IEnumerable<ContactMessageDto>> GetRecentMessagesAsync(int count = 5);
}
