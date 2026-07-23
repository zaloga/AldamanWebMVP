using Aldaman.Services.Dtos.General;

namespace Aldaman.Web.Models.General;

public class PaginationViewModel
{
    public PagedResultBase Result { get; }
    public string PageParamName { get; }

    public PaginationViewModel(PagedResultBase result, string pageParamName = "Page")
    {
        Result = result;
        PageParamName = pageParamName;
    }
}
