using System.Security.Claims;
using Aldaman.Persistence.Interfaces;

namespace Aldaman.Web.Infrastructure;

public sealed class WebUserContext : IUserContext
{
    private IHttpContextAccessor HttpContextAccessor { get; }

    public WebUserContext(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public Guid? CurrentUserId
    {
        get
        {
            var userIdString = HttpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}
