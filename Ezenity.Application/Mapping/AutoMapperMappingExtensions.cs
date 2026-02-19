using AutoMapper;

namespace Ezenity.Application.Mapping;

/// <summary>
/// Mapping helpers to keep profiles clean.
/// </summary>
internal static class AutoMapperMappingExtensions
{
    /// <summary>
    /// Ignore null values and ignore empty/whitespace strings on updates (patch-style behavior).
    /// </summary>
    public static IMappingExpression<TSource, TDestination> IgnoreNullOrEmptyStrings<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> map)
    {
        map.ForAllMembers(opt =>
            opt.Condition((src, dest, member) =>
            {
                if (member is null) return false;
                if (member is string s) return !string.IsNullOrWhiteSpace(s);
                return true;
            }));

        return map;
    }
}
