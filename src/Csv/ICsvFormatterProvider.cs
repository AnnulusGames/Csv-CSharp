namespace Csv;

public interface ICsvFormatterProvider
{
    public ICsvFormatter<T>? GetFormatter<T>();
}