using System.Buffers;
using System.Runtime.CompilerServices;

namespace Csv;

internal sealed class SharedBufferWriter : IBufferWriter<byte>
{
    [ThreadStatic] static SharedBufferWriter? shared;
    public static SharedBufferWriter GetWriter() => shared ??= new();

    byte[]? buffer;
    int count;

    public void Advance(int count)
    {
        this.count += count;
    }

    public void Reset()
    {
        count = 0;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        EnsureBuffer(count + sizeHint);
        return buffer.AsMemory(count, sizeHint);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        EnsureBuffer(count + sizeHint);
        return buffer.AsSpan(count, sizeHint);
    }

    public Span<byte> WrittenSpan => buffer.AsSpan(0, count);
    public Memory<byte> WrittenMemory => buffer.AsMemory(0, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void EnsureBuffer(int sizeHint)
    {
        if (buffer == null)
        {
            buffer = ArrayPool<byte>.Shared.Rent(sizeHint);
            return;
        }

        var size = buffer!.Length;
        if (sizeHint <= size) return;

        while (size < sizeHint)
        {
            size = MathEx.NewArrayCapacity(size);
            if (size == MathEx.ArrayMexLength) break;
        }

        var newBuffer = ArrayPool<byte>.Shared.Rent(size);
        WrittenSpan.CopyTo(newBuffer);
        ArrayPool<byte>.Shared.Return(buffer);
        buffer = newBuffer;
    }
}