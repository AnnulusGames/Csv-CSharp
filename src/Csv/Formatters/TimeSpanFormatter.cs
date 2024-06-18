namespace Csv.Formatters;

public sealed class TimeSpanFormatter(string? format = null, IFormatProvider? formatProvider = null) : ICsvFormatter<TimeSpan>
{
    public TimeSpan Deserialize(ref CsvReader reader)
    {
        var str = reader.ReadString();
        if (str == null) return default;
        return TimeSpan.Parse(str, formatProvider);
    }

    public void Serialize(ref CsvWriter writer, TimeSpan value)
    {
        writer.WriteUtf16(value.ToString(format, formatProvider));
    }
}