namespace Csv;

public interface ICsvSerializer<T>
{
    void Serialize(ref CsvWriter writer, ReadOnlySpan<T> values);
    void Serialize(ref CsvWriter writer, IEnumerable<T> values);
    int Deserialize(ref CsvReader reader, Span<T> destination);
    T[] Deserialize(ref CsvReader reader);
}

public interface ICsvSerializerRegister
{
    // TODO: Implementing static abstract registration for .NET 7.0 or greater
    // #if NET7_0_OR_GREATER
    //     static abstract void RegisterSerializer();
    // #endif
}