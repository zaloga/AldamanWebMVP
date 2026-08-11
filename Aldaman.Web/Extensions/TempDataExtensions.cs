using Aldaman.Services.Constants;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aldaman.Web.Extensions;

public static class TempDataExtensions
{
    public static void SetSuccessMessage(this ITempDataDictionary tempData, string message)
    {
        tempData[TempDataKeys.SuccessMessage] = message;
    }

    public static string? GetSuccessMessage(this ITempDataDictionary tempData)
    {
        return tempData[TempDataKeys.SuccessMessage] as string;
    }

    public static void SetErrorMessage(this ITempDataDictionary tempData, string message)
    {
        tempData[TempDataKeys.ErrorMessage] = message;
    }

    public static string? GetErrorMessage(this ITempDataDictionary tempData)
    {
        return tempData[TempDataKeys.ErrorMessage] as string;
    }

    public static void SetInfoMessage(this ITempDataDictionary tempData, string message)
    {
        tempData[TempDataKeys.InfoMessage] = message;
    }

    public static string? GetInfoMessage(this ITempDataDictionary tempData)
    {
        return tempData[TempDataKeys.InfoMessage] as string;
    }

    public static void SetShowTranslationMissingToast(this ITempDataDictionary tempData, bool show = true)
    {
        tempData[TempDataKeys.ShowTranslationMissingToast] = show;
    }

    public static bool GetShowTranslationMissingToast(this ITempDataDictionary tempData)
    {
        return tempData[TempDataKeys.ShowTranslationMissingToast] as bool? == true;
    }
}
