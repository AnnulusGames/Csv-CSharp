using System.Runtime.CompilerServices;
using Csv.Formatters;

namespace Csv;

public record CsvOptions
{
    public bool HasHeader { get; init; } = true;
    public bool AllowComments { get; init; } = true;
    public NewLineType NewLine { get; init; } = NewLineType.LF;
    public SeparatorType Separator { get; init; } = SeparatorType.Comma;
    public QuoteMode QuoteMode { get; init; } = QuoteMode.Minimal;
    public ICsvFormatterProvider FormatterProvider { get; init; } = StandardFormatterProvider.Instance;
}

public enum NewLineType : byte
{
    LF,
    CR,
    CRLF,
}

public enum SeparatorType : byte
{
    Comma,
    Semicolon,
    Tab,
    Pipe,
}

public enum QuoteMode
{
    All,
    NonNumeric,
    Minimal,
    None,
}

internal static class CsvOptionsEnumEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToUtf8(this SeparatorType separatorType)
    {
        return separatorType switch
        {
            SeparatorType.Comma => (byte)',',
            SeparatorType.Semicolon => (byte)';',
            SeparatorType.Tab => (byte)'\t',
            SeparatorType.Pipe => (byte)'|',
            _ => default,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> ToUtf8(this NewLineType newLineType)
    {
        return newLineType switch
        {
            NewLineType.LF => "\n"u8,
            NewLineType.CR => "\r"u8,
            NewLineType.CRLF => "\r\n"u8,
            _ => default,
        };
    }
}