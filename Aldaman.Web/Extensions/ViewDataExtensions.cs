using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aldaman.Web.Extensions;

public static class ViewDataExtensions
{
    private const string TitleKey = "Title";
    private const string LanguageAlternativesKey = "LanguageAlternatives";
    private const string RelativePathKey = "RelativePath";
    private const string IsImageKey = "IsImage";
    private const string ListTitleKey = "ListTitle";
    private const string DeletedListTitleKey = "DeletedListTitle";
    private const string IsDeletedKey = "IsDeleted";

    public static void SetPageTitle(this ViewDataDictionary viewData, string? title)
    {
        viewData[TitleKey] = title;
    }

    public static string? GetPageTitle(this ViewDataDictionary viewData)
    {
        return viewData[TitleKey]?.ToString();
    }

    public static void SetLanguageAlternatives(this ViewDataDictionary viewData, IDictionary<string, string> alternatives)
    {
        viewData[LanguageAlternativesKey] = alternatives;
    }

    public static IDictionary<string, string>? GetLanguageAlternatives(this ViewDataDictionary viewData)
    {
        return viewData[LanguageAlternativesKey] as IDictionary<string, string>;
    }

    public static void SetMediaPreview(this ViewDataDictionary viewData, string relativePath, bool isImage)
    {
        viewData[RelativePathKey] = relativePath;
        viewData[IsImageKey] = isImage;
    }

    public static (string? RelativePath, bool IsImage) GetMediaPreview(this ViewDataDictionary viewData)
    {
        string? relativePath = viewData[RelativePathKey] as string;
        bool isImage = viewData[IsImageKey] as bool? ?? false;
        return (relativePath, isImage);
    }

    public static void SetBreadcrumbListTitle(this ViewDataDictionary viewData, string listTitle)
    {
        viewData[ListTitleKey] = listTitle;
    }

    public static string? GetBreadcrumbListTitle(this ViewDataDictionary viewData)
    {
        return viewData[ListTitleKey]?.ToString();
    }

    public static void SetBreadcrumbDeletedListTitle(this ViewDataDictionary viewData, string deletedListTitle)
    {
        viewData[DeletedListTitleKey] = deletedListTitle;
    }

    public static string? GetBreadcrumbDeletedListTitle(this ViewDataDictionary viewData)
    {
        return viewData[DeletedListTitleKey]?.ToString();
    }

    public static void SetIsDeleted(this ViewDataDictionary viewData, bool isDeleted)
    {
        viewData[IsDeletedKey] = isDeleted;
    }

    public static bool? GetIsDeleted(this ViewDataDictionary viewData)
    {
        return viewData[IsDeletedKey] as bool?;
    }
}
