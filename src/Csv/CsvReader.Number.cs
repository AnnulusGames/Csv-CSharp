using System.Buffers;
using System.Buffers.Text;

namespace Csv;

partial struct CsvReader
{
    public sbyte ReadSByte()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 4; // -128

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out sbyte result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public byte ReadByte()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 3; // 255

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out byte result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public short ReadInt16()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 6; // -32768

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out short result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public int ReadInt32()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 11; // -2147483648;

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out int result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public long ReadInt64()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 21; // -9223372036854775808

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out long result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public ushort ReadUInt16()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 5; // 65535

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out ushort result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public uint ReadUInt32()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 10; // 4294967295

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out uint result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public ulong ReadUInt64()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 20; // 18446744073709551615

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out ulong result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public float ReadSingle()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 20;

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out float result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }

    public double ReadDouble()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 20;

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out double result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }


    public decimal ReadDecimal()
    {
        if (IsNextSeparatorOrNewline()) return default;
        TrySkipQuotation();

        const int MaxLength = 30;

        scoped ReadOnlySpan<byte> dest = reader.CurrentSpan[reader.CurrentSpanIndex..];
        if (dest.Length < MaxLength)
        {
            Span<byte> span = stackalloc byte[MaxLength];
            reader.Sequence.Slice(reader.Consumed, Math.Min(MaxLength, reader.Remaining)).CopyTo(span);
            dest = span;
        }

        if (!Utf8Parser.TryParse(dest, out decimal result, out var bytesConsumed))
        {
            CsvSerializationException.ThrowFailedEncoding();
        }

        reader.Advance(bytesConsumed);

        TrySkipQuotation();

        return result;
    }
}