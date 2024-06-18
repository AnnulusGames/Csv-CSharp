using System.Reflection;
using System.Runtime.CompilerServices;
using Csv.Annotations;

namespace Csv;

public static partial class CsvSerializer
{
    static CsvOptions? defaultOptions;
    public static CsvOptions DefaultOptions
    {
        get => defaultOptions ??= new();
        set => defaultOptions = value;
    }

    static class SerializerCache<T>
    {
        public static ICsvSerializer<T>? value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<T>(ICsvSerializer<T> serializer)
    {
        SerializerCache<T>.value = serializer;
    }

    public static ICsvSerializer<T>? GetSerializer<T>()
    {
        if (SerializerCache<T>.value != null) return SerializerCache<T>.value;

        if (typeof(T).GetCustomAttribute<CsvObjectAttribute>() != null)
        {
            var methodInfo = typeof(T).GetMethod("RegisterCsvSerializer", BindingFlags.NonPublic | BindingFlags.Static);
            methodInfo?.Invoke(null, null); // call Register<T>();
        }

        return SerializerCache<T>.value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ICsvSerializer<T> GetSerializerWithVerify<T>()
    {
        var serializer = GetSerializer<T>();
        if (serializer == null) CsvSerializationException.ThrowSerializerNotRegistered(typeof(T));
        return serializer;
    }
}