using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using Aldaman.Persistence.Constants;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Constants;
using Aldaman.Services.Resources;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Helpers;

/// <summary>
/// Helper for generating and caching database-level sorting expressions for PlaceToShowEnum across entities.
/// </summary>
public static class PlacementSortHelper
{
    private static readonly ConcurrentDictionary<string, Expression<Func<ContentGroupEntity, string>>> ContentGroupPlacementSortCache = new(StringComparer.OrdinalIgnoreCase);

    private static readonly MethodInfo CollateMethod = ((MethodCallExpression)((Expression<Func<string>>)(() => EF.Functions.Collate("", ""))).Body).Method;

    private static readonly (PlaceToShowEnum Flag, string ResourceKey)[] FlagDefinitions =
    [
        (PlaceToShowEnum.HomePage, UIResources.HomePage),
        (PlaceToShowEnum.TopNavigation, UIResources.TopNavigation),
        (PlaceToShowEnum.Footer, UIResources.Footer),
        (PlaceToShowEnum.Sidebar, UIResources.Sidebar)
    ];

    private const int TotalCombinations = 16; // 2^4 combinations for PlaceToShowEnum flags

    /// <summary>
    /// Gets or creates a cached sorting expression for ContentGroupEntity.PlaceToShow.
    /// </summary>
    /// <param name="culture">The target culture code (e.g. "cs", "en").</param>
    /// <returns>The compiled LINQ expression with explicit MSSQL collation.</returns>
    public static Expression<Func<ContentGroupEntity, string>> GetOrCreatePlacementSortExpression(string? culture)
    {
        string normalizedCulture = CultureHelper.NormalizeCulture(culture);

        return ContentGroupPlacementSortCache.GetOrAdd(normalizedCulture, static cult =>
        {
            var (labelsByValue, collation) = BuildLabelsAndCollation(cult);

            ParameterExpression parameter = Expression.Parameter(typeof(ContentGroupEntity), "p");
            MemberExpression property = Expression.Property(parameter, nameof(ContentGroupEntity.PlaceToShow));

            // Default fallback is None / NotSet (index 0)
            Expression body = Expression.Constant(labelsByValue[0]);

            // Build nested conditional expressions: p.PlaceToShow == Flag ? Label : ElseBody
            for (int i = TotalCombinations - 1; i >= 1; i--)
            {
                BinaryExpression test = Expression.Equal(
                    property,
                    Expression.Constant((PlaceToShowEnum)i, typeof(PlaceToShowEnum)));

                body = Expression.Condition(test, Expression.Constant(labelsByValue[i]), body);
            }

            // Wrap with EF.Functions.Collate(body, collation)
            MethodCallExpression collateCall = Expression.Call(
                null,
                CollateMethod,
                Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                body,
                Expression.Constant(collation));

            return Expression.Lambda<Func<ContentGroupEntity, string>>(collateCall, parameter);
        });
    }

    private static (string[] LabelsByValue, string Collation) BuildLabelsAndCollation(string normalizedCulture)
    {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(normalizedCulture);
        ResourceManager resourceManager = new(typeof(UIResources));

        string notSet = GetResourceString(resourceManager, UIResources.NotSet, cultureInfo);

        (PlaceToShowEnum Flag, string Label)[] localizedFlags = new (PlaceToShowEnum, string)[FlagDefinitions.Length];
        for (int i = 0; i < FlagDefinitions.Length; i++)
        {
            localizedFlags[i] = (FlagDefinitions[i].Flag, GetResourceString(resourceManager, FlagDefinitions[i].ResourceKey, cultureInfo));
        }

        string[] labelsByValue = new string[TotalCombinations];
        labelsByValue[0] = notSet;

        List<string> parts = new(FlagDefinitions.Length);
        for (int i = 1; i < TotalCombinations; i++)
        {
            PlaceToShowEnum enumValue = (PlaceToShowEnum)i;
            parts.Clear();

            foreach (var (flag, label) in localizedFlags)
            {
                if ((enumValue & flag) == flag)
                {
                    parts.Add(label);
                }
            }

            labelsByValue[i] = string.Join(", ", parts);
        }

        string collation = MsSqlCollationConstants.GetCollationForCulture(normalizedCulture);

        return (labelsByValue, collation);
    }

    private static string GetResourceString(ResourceManager resourceManager, string resourceKey, CultureInfo cultureInfo)
    {
        return resourceManager.GetString(resourceKey, cultureInfo) ?? resourceKey;
    }
}
