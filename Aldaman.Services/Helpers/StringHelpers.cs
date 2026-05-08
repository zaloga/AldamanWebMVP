using System.Net;
using System.Text.RegularExpressions;

namespace Aldaman.Services.Helpers;

public static class StringHelpers
{
    /// <summary>
    /// Removes HTML tags from a string and replaces them with a single space.
    /// Also decodes HTML entities and normalizes whitespace.
    /// </summary>
    /// <param name="html">The HTML content to strip.</param>
    /// <param name="maxLength">Optional maximum length of the resulting string.</param>
    /// <returns>A plain text string.</returns>
    public static string? StripHtml(string? html, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return html;
        }

        // Replace HTML tags with a space to avoid merging text from adjacent tags (e.g. </div><div>)
        string plainText = Regex.Replace(html, "<[^>]*>", " ");

        // Decode HTML entities (e.g. &nbsp; -> space, &amp; -> &)
        plainText = WebUtility.HtmlDecode(plainText);
        
        // Normalize whitespace: replace multiple spaces/newlines with a single space and trim
        plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

        if (maxLength.HasValue && plainText.Length > maxLength.Value)
        {
            plainText = plainText.Substring(0, maxLength.Value);
        }

        return plainText;
    }

    /// <summary>
    /// Extracts all media paths (starting with /uploads/) from HTML content.
    /// </summary>
    /// <param name="html">The HTML content to parse.</param>
    /// <returns>A collection of unique relative paths.</returns>
    public static IEnumerable<string> ExtractMediaPaths(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return Enumerable.Empty<string>();
        }

        // Match any string starting with /uploads/ inside quotes (common for src and href)
        var matches = Regex.Matches(html, @"[""'](/uploads/[^""']+)[""']", RegexOptions.IgnoreCase);
        return matches.Select(m => m.Groups[1].Value).Distinct();
    }
}
