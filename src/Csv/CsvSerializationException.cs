using System.Diagnostics.CodeAnalysis;

namespace Csv;

[Serializable]
public class CsvSerializationException : Exception
{
    public CsvSerializationException()
    {
    }

    public CsvSerializationException(string? message)
        : base(message)
    {
    }

    public CsvSerializationException(string? message, Exception? inner)
        : base(message, inner)
    {
    }

    [DoesNotReturn]
    internal static void ThrowMessage(string message)
    {
        throw new CsvSerializationException(message);
    }

    [DoesNotReturn]
    public static void ThrowSerializerNotRegistered(Type type)
    {
        throw new CsvSerializationException($"No serializer is registered for type:{type.FullName}.");
    }

    [DoesNotReturn]
    public static void ThrowSequenceReachedEnd()
    {
        throw new CsvSerializationException($"Sequence reached end, reader can not provide more buffer.");
    }

    [DoesNotReturn]
    public static void ThrowInvalidAdvance()
    {
        throw new CsvSerializationException($"Cannot advance past the end of the buffer.");
    }

    [DoesNotReturn]
    public static void ThrowHeaderRequired()
    {
        throw new CsvSerializationException("A header column is required for string-based serialization.");
    }

    [DoesNotReturn]
    public static void ThrowFailedEncoding()
    {
        throw new CsvSerializationException("Failed in Utf8 encoding/decoding process.");
    }

    [DoesNotReturn]
    internal static void ThrowFailedEncoding<T>(T value)
    {
        throw new CsvSerializationException($"Failed in Utf8 encoding/decoding process. value: {value}");
    }
}