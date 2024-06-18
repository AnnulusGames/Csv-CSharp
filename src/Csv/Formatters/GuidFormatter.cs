namespace Csv.Formatters;

public sealed class GuidFormatter : ICsvFormatter<Guid>
{
    public Guid Deserialize(ref CsvReader reader)
    {
        var str = reader.ReadString();
        if (str == null) return default;
        return Guid.Parse(str);
    }

    public void Serialize(ref CsvWriter writer, Guid value)
    {
        writer.WriteUtf16(value.ToString());
    }
}