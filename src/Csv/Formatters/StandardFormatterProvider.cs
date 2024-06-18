namespace Csv.Formatters;

public sealed class StandardFormatterProvider : ICsvFormatterProvider
{
    public static readonly StandardFormatterProvider Instance = new();

    StandardFormatterProvider()
    {
    }

    static class Cache<T>
    {
        public static ICsvFormatter<T>? value;
    }

    public ICsvFormatter<T>? GetFormatter<T>()
    {
        if (Cache<T>.value != null) goto RETURN;

        if (typeof(T).IsEnum)
        {
            Cache<T>.value = (ICsvFormatter<T>)Activator.CreateInstance(typeof(EnumFormatter<>).MakeGenericType(typeof(T)))!;
            goto RETURN;
        }

        if (typeof(T) == typeof(DateTime))
        {
            Cache<T>.value = new DateTimeFormatter() as ICsvFormatter<T>;
            goto RETURN;
        }

        if (typeof(T) == typeof(TimeSpan))
        {
            Cache<T>.value = new TimeSpanFormatter() as ICsvFormatter<T>;
            goto RETURN;
        }

        if (typeof(T) == typeof(Guid))
        {
            Cache<T>.value = new GuidFormatter() as ICsvFormatter<T>;
            goto RETURN;
        }

        if (typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            Cache<T>.value = (ICsvFormatter<T>)Activator.CreateInstance(typeof(NullableFormatter<>).MakeGenericType(typeof(T)))!;
            goto RETURN;
        }

    RETURN:
        return Cache<T>.value;
    }
}