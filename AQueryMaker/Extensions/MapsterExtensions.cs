using Mapster;

namespace AQueryMaker.Extensions;

// ReSharper disable once UnusedType.Global
public static class MapsterExtensions
{
    public static List<T> MapTo<T>(this List<Dictionary<string, object>> sourceList)
    {
        return sourceList.Adapt<List<T>>();
    }

    public static T MapTo<T>(this Dictionary<string, object> source)
    {
        return source.Adapt<T>();
    }
}




