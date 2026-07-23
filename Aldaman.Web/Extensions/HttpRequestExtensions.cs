using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Aldaman.Web.Extensions;

public static class HttpRequestExtensions
{
    public static string GetPageUrl(this HttpRequest request, int page)
    {
        var query = new QueryBuilder(request.Query.Where(q => q.Key != "page"));
        query.Add("page", page.ToString());
        return request.Path + query.ToQueryString();
    }
}
