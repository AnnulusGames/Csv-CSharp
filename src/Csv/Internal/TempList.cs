using System.Buffers;
using System.Runtime.CompilerServices;

namespace Csv.Internal;

public struct TempList<T> : IDisposable
{
    T[] buffer;
    int count;

    public int Count => count;

    public TempList() : this(8)
    {
    }

    public TempList(int sizeHint)
    {
        buffer = ArrayPool<T>.Shared.Rent(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        if (buffer.Length == count)
        {
            var newArray = ArrayPool<T>.Shared.Rent(count * 2);
            buffer.AsSpan().CopyTo(newArray);
            ArrayPool<T>.Shared.Return(buffer);
            buffer = newArray;
        }

        buffer[count++] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear(bool clearArray = true)
    {
        if (clearArray) buffer.AsSpan().Clear();
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int newLength)
    {
        if (buffer.Length < newLength)
        {
            var newArray = ArrayPool<T>.Shared.Rent(count * 2);
            ArrayPool<T>.Shared.Return(buffer);
            buffer = newArray;
        }

        count = newLength;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return new Span<T>(buffer, 0, count);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> AsMemory()
    {
        return new Memory<T>(buffer, 0, count);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(buffer, true);
    }
}