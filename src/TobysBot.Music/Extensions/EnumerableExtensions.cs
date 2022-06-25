namespace TobysBot.Music.Extensions;

public static class EnumerableExtensions
{
    public static TimeSpan Sum(this IEnumerable<TimeSpan> collection)
    {
        return new TimeSpan(collection.Sum(x => x.Ticks));
    }

    public static TimeSpan Sum<T>(this IEnumerable<T> collection, Func<T, TimeSpan> selector)
    {
        return collection.Select(selector).Sum();
    }
}