using Aldaman.Integrations.Email.Options;
using Aldaman.Integrations.Email.Services;
using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class ContactService : IContactService
{
    private AppDbContext Context { get; }
    private IEmailService EmailService { get; }
    private EmailOptions EmailOptions { get; }

    public ContactService(
        AppDbContext context,
        IEmailService emailService,
        IOptions<EmailOptions> emailOptions)
    {
        Context = context;
        EmailService = emailService;
        EmailOptions = emailOptions.Value;
    }

    public async Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query)
    {
        var dbQuery = Context.ContactMessages.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Name.Contains(query.SearchTerm) || p.EmailOrPhone.Contains(query.SearchTerm) || (p.Subject != null && p.Subject.Contains(query.SearchTerm)));
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
                EmailOrPhone = p.EmailOrPhone,
                Phone = p.Phone,
                Subject = p.Subject,
                Message = p.Message,
                CreatedAtUtc = p.CreatedAtUtc,
                SentAtUtc = p.SentAtUtc,
                State = p.State
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

    public async Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent)
    {
        var entity = new ContactMessageEntity
        {
            Name = dto.Name,
            EmailOrPhone = dto.EmailOrPhone,
            Phone = dto.Phone,
            Subject = dto.Subject,
            Message = dto.Message,
            ClientIp = clientIp,
            UserAgent = userAgent,
            State = ContactMessageState.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        Context.ContactMessages.Add(entity);
        await Context.SaveChangesAsync();

        try
        { // TODO...
            string body = $"""
                <h3>New Contact Message</h3>
                <p><strong>Name:</strong> {entity.Name}</p>
                <p><strong>Email/Phone:</strong> {entity.EmailOrPhone}</p>
                <p><strong>Phone:</strong> {entity.Phone ?? "N/A"}</p>
                <p><strong>Subject:</strong> {entity.Subject ?? "N/A"}</p>
                <p><strong>Message:</strong></p>
                <p>{entity.Message.Replace("\n", "<br/>")}</p>
            """;

            await EmailService.SendEmailAsync(
                EmailOptions.AdminEmail,
                $"Contact Form: {entity.Subject ?? "New Message"}",
                body,
                isHtml: true);

            entity.State = ContactMessageState.Sent;
            entity.SentAtUtc = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            entity.State = ContactMessageState.Failed;
            entity.FailureReason = ex.Message;
        }

        await Context.SaveChangesAsync();
    }

    public async Task MarkAsHandledAsync(Guid id)
    {
        var message = await Context.ContactMessages.FindAsync(id);
        if (message != null)
        {
            message.State = ContactMessageState.Handled;
            await Context.SaveChangesAsync();
        }
    }

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

    public async Task<IEnumerable<ContactMessageDto>> GetRecentMessagesAsync(int count = 5)
    {
        return await Context.ContactMessages
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(count)
            .Select(p => new ContactMessageDto
            {
                Id = p.Id,
                Name = p.Name,
                EmailOrPhone = p.EmailOrPhone,
                Phone = p.Phone,
                Subject = p.Subject,
                Message = p.Message,
                CreatedAtUtc = p.CreatedAtUtc,
                SentAtUtc = p.SentAtUtc,
                State = p.State
            })
            .ToListAsync();
    }
}
