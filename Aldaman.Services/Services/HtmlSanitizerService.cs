using Aldaman.Services.Interfaces;
using Ganss.Xss;

namespace Aldaman.Services.Services;

/// <summary>
/// Secure HTML sanitizer service utilizing Ganss.Xss HtmlSanitizer configured for Quill RTE output.
/// </summary>
public sealed class HtmlSanitizerService : IHtmlSanitizerService
{
    private HtmlSanitizer Sanitizer { get; }

    public HtmlSanitizerService()
    {
        Sanitizer = new HtmlSanitizer();

        ConfigureAllowedTags();
        ConfigureAllowedAttributes();
        ConfigureAllowedCssProperties();
        ConfigureAllowedSchemes();
    }

    /// <inheritdoc />
    public string Sanitize(string? rawHtml)
    {
        if (string.IsNullOrWhiteSpace(rawHtml))
        {
            return string.Empty;
        }

        return Sanitizer.Sanitize(rawHtml);
    }

    private void ConfigureAllowedTags()
    {
        // Allowed structure & typography tags
        Sanitizer.AllowedTags.Add("p");
        Sanitizer.AllowedTags.Add("br");
        Sanitizer.AllowedTags.Add("div");
        Sanitizer.AllowedTags.Add("span");
        Sanitizer.AllowedTags.Add("strong");
        Sanitizer.AllowedTags.Add("b");
        Sanitizer.AllowedTags.Add("em");
        Sanitizer.AllowedTags.Add("i");
        Sanitizer.AllowedTags.Add("u");
        Sanitizer.AllowedTags.Add("s");
        Sanitizer.AllowedTags.Add("sub");
        Sanitizer.AllowedTags.Add("sup");
        Sanitizer.AllowedTags.Add("h1");
        Sanitizer.AllowedTags.Add("h2");
        Sanitizer.AllowedTags.Add("h3");
        Sanitizer.AllowedTags.Add("h4");
        Sanitizer.AllowedTags.Add("h5");
        Sanitizer.AllowedTags.Add("h6");
        Sanitizer.AllowedTags.Add("blockquote");
        Sanitizer.AllowedTags.Add("pre");
        Sanitizer.AllowedTags.Add("code");
        Sanitizer.AllowedTags.Add("ol");
        Sanitizer.AllowedTags.Add("ul");
        Sanitizer.AllowedTags.Add("li");
        Sanitizer.AllowedTags.Add("a");

        // Allowed media tags
        Sanitizer.AllowedTags.Add("img");

        // Explicitly allowed table tags for Quill table module
        Sanitizer.AllowedTags.Add("table");
        Sanitizer.AllowedTags.Add("tbody");
        Sanitizer.AllowedTags.Add("thead");
        Sanitizer.AllowedTags.Add("tfoot");
        Sanitizer.AllowedTags.Add("tr");
        Sanitizer.AllowedTags.Add("td");
        Sanitizer.AllowedTags.Add("th");
        Sanitizer.AllowedTags.Add("colgroup");
        Sanitizer.AllowedTags.Add("col");
    }

    private void ConfigureAllowedAttributes()
    {
        // Explicitly allowed attributes for formatting, image wrapping & tables
        Sanitizer.AllowedAttributes.Add("class");
        Sanitizer.AllowedAttributes.Add("style");
        Sanitizer.AllowedAttributes.Add("src");
        Sanitizer.AllowedAttributes.Add("alt");
        Sanitizer.AllowedAttributes.Add("title");
        Sanitizer.AllowedAttributes.Add("width");
        Sanitizer.AllowedAttributes.Add("height");
        Sanitizer.AllowedAttributes.Add("href");
        Sanitizer.AllowedAttributes.Add("target");
        Sanitizer.AllowedAttributes.Add("rel");
        Sanitizer.AllowedAttributes.Add("align");
        Sanitizer.AllowedAttributes.Add("colspan");
        Sanitizer.AllowedAttributes.Add("rowspan");
        Sanitizer.AllowedAttributes.Add("cellpadding");
        Sanitizer.AllowedAttributes.Add("cellspacing");
        Sanitizer.AllowedAttributes.Add("border");
        Sanitizer.AllowedAttributes.Add("data-language");
    }

    private void ConfigureAllowedCssProperties()
    {
        // Explicitly allowed CSS properties required for image wrapping floats and table styles
        Sanitizer.AllowedCssProperties.Add("float");
        Sanitizer.AllowedCssProperties.Add("margin");
        Sanitizer.AllowedCssProperties.Add("margin-left");
        Sanitizer.AllowedCssProperties.Add("margin-right");
        Sanitizer.AllowedCssProperties.Add("margin-top");
        Sanitizer.AllowedCssProperties.Add("margin-bottom");
        Sanitizer.AllowedCssProperties.Add("display");
        Sanitizer.AllowedCssProperties.Add("text-align");
        Sanitizer.AllowedCssProperties.Add("width");
        Sanitizer.AllowedCssProperties.Add("height");
        Sanitizer.AllowedCssProperties.Add("max-width");
        Sanitizer.AllowedCssProperties.Add("border");
        Sanitizer.AllowedCssProperties.Add("border-collapse");
        Sanitizer.AllowedCssProperties.Add("border-color");
        Sanitizer.AllowedCssProperties.Add("border-style");
        Sanitizer.AllowedCssProperties.Add("border-width");
        Sanitizer.AllowedCssProperties.Add("padding");
        Sanitizer.AllowedCssProperties.Add("padding-left");
        Sanitizer.AllowedCssProperties.Add("padding-right");
        Sanitizer.AllowedCssProperties.Add("padding-top");
        Sanitizer.AllowedCssProperties.Add("padding-bottom");
        Sanitizer.AllowedCssProperties.Add("vertical-align");
        Sanitizer.AllowedCssProperties.Add("background-color");
        Sanitizer.AllowedCssProperties.Add("color");
        Sanitizer.AllowedCssProperties.Add("clear");
    }

    private void ConfigureAllowedSchemes()
    {
        Sanitizer.AllowedSchemes.Add("http");
        Sanitizer.AllowedSchemes.Add("https");
        Sanitizer.AllowedSchemes.Add("data");
    }
}
