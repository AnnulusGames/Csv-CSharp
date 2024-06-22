using System.Buffers;

namespace Csv;

public static partial class CsvSerializer
{
    public static CsvDocument ConvertToDocument(byte[] bytes, CsvOptions? options = default)
    {
        return new CsvDocument(new(bytes), options ?? DefaultOptions);
    }

    public static CsvDocument ConvertToDocument(ReadOnlySequence<byte> sequence, CsvOptions? options = default)
    {
        return new CsvDocument(sequence, options ?? DefaultOptions);
    }
}