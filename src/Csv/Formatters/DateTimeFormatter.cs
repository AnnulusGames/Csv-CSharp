namespace Csv.Formatters;

// TODO: optimize

public sealed class DateTimeFormatter(string? format = null, IFormatProvider? formatProvider = null) : ICsvFormatter<DateTime>
{
    public DateTime Deserialize(ref CsvReader reader)
    {
        var str = reader.ReadString();
        if (str == null) return default;
        return DateTime.Parse(str, formatProvider);
    }

    public void Serialize(ref CsvWriter writer, DateTime value)
    {
        writer.WriteString(value.ToString(format, formatProvider));
    }
}