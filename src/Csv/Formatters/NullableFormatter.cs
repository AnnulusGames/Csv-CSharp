namespace Csv.Formatters;

public sealed class NullableFormatter<T> : ICsvFormatter<T?>
    where T : struct
{
    public T? Deserialize(ref CsvReader reader)
    {
        if (reader.IsNextSeparatorOrNewline(false)) return null;
        return reader.Options.FormatterProvider.GetFormatter<T>()!.Deserialize(ref reader);
    }

    public void Serialize(ref CsvWriter writer, T? value)
    {
        if (value == null) return;
        writer.Options.FormatterProvider.GetFormatter<T>()!.Serialize(ref writer, value.Value);
    }
}