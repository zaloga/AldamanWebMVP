using Aldaman.Services.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Aldaman.Web.TagHelpers;

/// <summary>
/// Generates a clickable column header for sorting tabular data.
/// </summary>
[HtmlTargetElement("th", Attributes = "sort-by")]
public class SortableHeaderTagHelper : TagHelper
{
    private IHtmlGenerator Generator { get; }

    public SortableHeaderTagHelper(IHtmlGenerator generator)
    {
        Generator = generator;
    }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    [HtmlAttributeName("sort-by")]
    public string SortBy { get; set; } = string.Empty;

    [HtmlAttributeName("default-descending")]
    public bool DefaultDescending { get; set; } = false;

    [HtmlAttributeName("action")]
    public string Action { get; set; } = "Index";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        TagHelperContent childContent = await output.GetChildContentAsync();
        string headerText = childContent.GetContent();

        var query = ViewContext.HttpContext.Request.Query;
        string? currentSortBy = query[SortByConstants.SortBy];
        string? currentSortDescStr = query[SortByConstants.SortDescending];

        bool isCurrentSort = string.Equals(SortBy, currentSortBy, StringComparison.OrdinalIgnoreCase);

        bool currentIsDescending;
        if (string.IsNullOrEmpty(currentSortDescStr))
        {
            currentIsDescending = DefaultDescending;
        }
        else
        {
            _ = bool.TryParse(currentSortDescStr, out currentIsDescending);
        }

        // Clicking toggle logic:
        // If current column is active, toggle direction;
        // If new column is clicked, start with ascending (or default if defined).
        bool nextSortDescending = isCurrentSort ? !currentIsDescending : false;

        // Preserve existing query parameters (search, filters, etc.), but reset page to 1
        RouteValueDictionary routeValues = new RouteValueDictionary();
        foreach (var param in query)
        {
            if (string.Equals(param.Key, "page", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            routeValues[param.Key] = param.Value.ToString();
        }

        routeValues[SortByConstants.SortBy] = SortBy;
        routeValues[SortByConstants.SortDescending] = nextSortDescending.ToString().ToLowerInvariant();

        TagBuilder linkBuilder = Generator.GenerateActionLink(
            ViewContext,
            linkText: string.Empty,
            actionName: Action,
            controllerName: null,
            protocol: null,
            hostname: null,
            fragment: null,
            routeValues: routeValues,
            htmlAttributes: new { @class = "text-decoration-none text-reset d-inline-flex align-items-center gap-1 user-select-none" });

        string iconHtml;
        if (isCurrentSort)
        {
            iconHtml = currentIsDescending
                ? "<i class=\"bi bi-arrow-down text-primary fw-bold\"></i>"
                : "<i class=\"bi bi-arrow-up text-primary fw-bold\"></i>";
        }
        else
        {
            iconHtml = "<i class=\"bi bi-arrow-down-up text-muted opacity-25\"></i>";
        }

        linkBuilder.InnerHtml.AppendHtml($"<span>{headerText}</span> {iconHtml}");

        output.Content.SetHtmlContent(linkBuilder);
    }
}
