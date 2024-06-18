namespace Csv.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CsvObjectAttribute(bool keyAsPropertyName = false) : Attribute
{
    public bool KeyAsPropertyName { get; } = keyAsPropertyName;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ColumnAttribute : Attribute
{
    public int Index { get; }
    public string? Key { get; }

    public ColumnAttribute(int index)
    {
        Index = index;
    }

    public ColumnAttribute(string key)
    {
        Key = key;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class IgnoreMemberAttribute : Attribute;