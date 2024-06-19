using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Csv;

[StructLayout(LayoutKind.Auto)]
public ref partial struct CsvWriter
{
    public CsvWriter(IBufferWriter<byte> bufferWriter, CsvOptions options)
    {
        this.writer = bufferWriter;
        this.options = options;

        separator = options.Separator.ToUtf8();
        newLine = options.NewLine.ToUtf8();
    }

    readonly IBufferWriter<byte> writer;
    readonly CsvOptions options;
    readonly byte separator;
    readonly ReadOnlySpan<byte> newLine;

    public IBufferWriter<byte> BufferWriter => writer;
    public CsvOptions Options => options;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> GetSpan(int sizeHint)
    {
        return writer.GetSpan(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        writer.Advance(count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteRaw(byte code)
    {
        GetSpan(1)[0] = code;
        Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteRaw(scoped ReadOnlySpan<byte> bytes)
    {
        var span = GetSpan(bytes.Length);
        bytes.CopyTo(span);
        Advance(bytes.Length);
    }

    public void WriteBoolean(bool value)
    {
        if (options.QuoteMode is QuoteMode.Minimal or QuoteMode.None)
        {
            if (value)
            {
                var span = GetSpan(4);
                span[0] = (byte)'t';
                span[1] = (byte)'r';
                span[2] = (byte)'u';
                span[3] = (byte)'e';
                Advance(4);
            }
            else
            {
                var span = GetSpan(5);
                span[0] = (byte)'f';
                span[1] = (byte)'a';
                span[2] = (byte)'l';
                span[3] = (byte)'s';
                span[4] = (byte)'e';
                Advance(5);
            }
        }
        else
        {
            if (value)
            {
                var span = GetSpan(6);
                span[0] = (byte)'"';
                span[1] = (byte)'t';
                span[2] = (byte)'r';
                span[3] = (byte)'u';
                span[4] = (byte)'e';
                span[5] = (byte)'"';
                Advance(6);
            }
            else
            {
                var span = GetSpan(7);
                span[0] = (byte)'"';
                span[1] = (byte)'f';
                span[2] = (byte)'a';
                span[3] = (byte)'l';
                span[4] = (byte)'s';
                span[5] = (byte)'e';
                span[6] = (byte)'"';
                Advance(7);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteChar(char value)
    {
        WriteString([value]);
    }
    
    public void WriteString(scoped ReadOnlySpan<char> value)
    {
        // TODO: optimize

        var shouldQuote = options.QuoteMode is not (QuoteMode.Minimal or QuoteMode.None);
        var separator = (char)this.separator;
#if NET8_0_OR_GREATER
        shouldQuote |= value.ContainsAny(['"', '\n', '\r', separator]);
#else
        for (int i = 0; i < value.Length; i++)
        {
            var item = value[i];
            shouldQuote |= item is '"' or '\n' or '\r';
            shouldQuote |= item == separator;
        }
#endif

        var max = Encoding.UTF8.GetMaxByteCount(value.Length);
        if (shouldQuote) max += 2;

        var span = GetSpan(max);

        var offset = 0;
        var from = 0;

        if (shouldQuote)
        {
            span[offset++] = (byte)'"';
        }

        for (int i = 0; i < value.Length; i++)
        {
            byte escapeChar;
            switch (value[i])
            {
                case '"':
                    escapeChar = (byte)'"';
                    break;
                default:
                    continue;
            }

            max++;
            if (span.Length < max) span = GetSpan(max);

            offset += Encoding.UTF8.GetBytes(value[from..i], span[offset..]);
            from = i + 1;
            span[offset++] = (byte)'"';
            span[offset++] = escapeChar;
        }

        if (from != value.Length)
        {
            offset += Encoding.UTF8.GetBytes(value[from..], span[offset..]);
        }

        if (shouldQuote)
        {
            span[offset++] = (byte)'"';
        }

        Advance(offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSeparator()
    {
        WriteRaw(separator);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEndOfLine()
    {
        WriteRaw(newLine);
    }
}