using System.Reflection;
using System.Runtime.Serialization;

namespace Csv.Formatters;

// TODO: optimize

public sealed class EnumFormatter<T> : ICsvFormatter<T>
    where T : struct, Enum
{
    readonly IReadOnlyDictionary<string, T> nameValueMapping;
    readonly IReadOnlyDictionary<T, string> valueNameMapping;
    readonly IReadOnlyDictionary<string, string>? clrToSerializationName;
    readonly IReadOnlyDictionary<string, string>? serializationToClrName;
    readonly bool isFlags;

    public EnumFormatter()
    {
        isFlags = typeof(T).GetCustomAttribute<FlagsAttribute>() is not null;

        var fields = typeof(T).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
        var nameValueMapping = new Dictionary<string, T>(fields.Length);
        var valueNameMapping = new Dictionary<T, string>();
        Dictionary<string, string>? clrToSerializationName = null;
        Dictionary<string, string>? serializationToClrName = null;

        foreach (var enumValueMember in fields)
        {
            var name = enumValueMember.Name;
            var value = (T)enumValueMember.GetValue(null)!;

            var attribute = enumValueMember.GetCustomAttribute<EnumMemberAttribute>();
            if (attribute is { IsValueSetExplicitly: true, Value: not null })
            {
                clrToSerializationName ??= [];
                serializationToClrName ??= [];

                clrToSerializationName.Add(name, attribute.Value);
                serializationToClrName.Add(attribute.Value, name);

                name = attribute.Value;
            }

            nameValueMapping[name] = value;
            valueNameMapping[value] = name;
        }

        this.nameValueMapping = nameValueMapping;
        this.valueNameMapping = valueNameMapping;
        this.clrToSerializationName = clrToSerializationName;
        this.serializationToClrName = serializationToClrName;
    }

    public T Deserialize(ref CsvReader reader)
    {
        var name = reader.ReadString();
        if (name == null) return default;

        if (!nameValueMapping.TryGetValue(name, out T value))
        {
            value = (T)Enum.Parse(typeof(T), GetClrNames(name));
        }

        return value;
    }

    public void Serialize(ref CsvWriter writer, T value)
    {
        if (!valueNameMapping.TryGetValue(value, out string? valueString))
        {
            valueString = GetSerializedNames(value.ToString());
        }

        writer.WriteString(valueString);
    }

    string GetClrNames(string serializedNames)
    {
        if (serializationToClrName is not null && isFlags && serializedNames.IndexOf(", ", StringComparison.Ordinal) >= 0)
        {
            return Translate(serializedNames, serializationToClrName);
        }

        return serializedNames;
    }

    string GetSerializedNames(string clrNames)
    {
        if (clrToSerializationName is not null && isFlags && clrNames.IndexOf(", ", StringComparison.Ordinal) >= 0)
        {
            return Translate(clrNames, clrToSerializationName);
        }

        return clrNames;
    }

    static string Translate(string items, IReadOnlyDictionary<string, string> mapping)
    {
        var elements = items.Split(',');

        for (int i = 0; i < elements.Length; i++)
        {
            if (i > 0 && elements[i].Length > 0 && elements[i][0] == ' ')
            {
                elements[i] = elements[i].Substring(1);
            }

            if (mapping.TryGetValue(elements[i], out string? substituteValue))
            {
                elements[i] = substituteValue;
            }
        }

        return string.Join(", ", elements);
    }
}