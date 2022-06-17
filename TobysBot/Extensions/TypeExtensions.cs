namespace TobysBot.Extensions;

public static class TypeExtensions
{
    public static bool IsAssignableTo<T>(this Type type)
    {
        return type.IsAssignableTo(typeof(T));
    }
}