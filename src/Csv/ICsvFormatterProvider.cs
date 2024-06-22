namespace Csv;

public interface ICsvFormatterProvider
{
    public ICsvFormatter<T>? GetFormatter<T>();
}

internal static class CsvFormatterProviderExtensions
{
    public static ICsvFormatter<T> GetFormatterWithVarify<T>(this ICsvFormatterProvider formatterProvider)
    {
        var formatter = formatterProvider.GetFormatter<T>();
        if (formatter == null) CsvSerializationException.ThrowFormatterNotRegistered(typeof(T));
        return formatter;
    }
}
