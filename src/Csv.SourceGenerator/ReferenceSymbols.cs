using Microsoft.CodeAnalysis;

namespace Csv.SourceGenerator;

public sealed class ReferenceSymbols
{
    public static ReferenceSymbols? Create(Compilation compilation)
    {
        var csvObjectAttribute = compilation.GetTypeByMetadataName("Csv.Annotations.CsvObjectAttribute");
        if (csvObjectAttribute == null) return null;

        return new ReferenceSymbols
        {
            CsvObjectAttribute = csvObjectAttribute,
            ColumnAttribute = compilation.GetTypeByMetadataName("Csv.Annotations.ColumnAttribute")!,
            IgnoreMemberAttribute = compilation.GetTypeByMetadataName("Csv.Annotations.IgnoreMemberAttribute")!,
        };
    }

    public INamedTypeSymbol CsvObjectAttribute { get; private set; } = default!;
    public INamedTypeSymbol ColumnAttribute { get; private set; } = default!;
    public INamedTypeSymbol IgnoreMemberAttribute { get; private set; } = default!;
}