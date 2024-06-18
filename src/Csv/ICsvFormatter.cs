namespace Csv;

public interface ICsvFormatter<T>
{
    void Serialize(ref CsvWriter writer, T value);
    T Deserialize(ref CsvReader reader);
}