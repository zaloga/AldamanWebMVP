using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class ContactService : IContactService
{
    private AppDbContext Context { get; }

    public ContactService(AppDbContext context)
    {
        Context = context;
    }

    public async Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query)
    {
        var dbQuery = Context.ContactMessages.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Name.Contains(query.SearchTerm) || p.Email.Contains(query.SearchTerm) || (p.Subject != null && p.Subject.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "Name" => query.SortDescending ? dbQuery.OrderByDescending(p => p.Name) : dbQuery.OrderBy(p => p.Name),
            "CreatedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "State" => query.SortDescending ? dbQuery.OrderByDescending(p => p.State) : dbQuery.OrderBy(p => p.State),
            _ => dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContactMessageDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                Subject = p.Subject,
                Message = p.Message,
                CreatedAtUtc = p.CreatedAtUtc,
                SentAtUtc = p.SentAtUtc,
                State = (ContactMessageState)p.State
            })
            .ToListAsync();

        return new PagedResultDto<ContactMessageDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent) => throw new NotImplementedException();
    public Task MarkAsHandledAsync(Guid id) => throw new NotImplementedException();

    public async Task DeleteMessageAsync(Guid id)
    {
        var message = await Context.ContactMessages.FindAsync(id);
        if (message != null)
        {
            message.IsDeleted = true;
            message.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<ContactMessageDto>> GetRecentMessagesAsync() => throw new NotImplementedException();
}
