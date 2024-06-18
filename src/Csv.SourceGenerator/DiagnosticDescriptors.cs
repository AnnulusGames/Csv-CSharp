#pragma warning disable RS2008

using Microsoft.CodeAnalysis;

namespace Csv.SourceGenerator;

public static class DiagnosticDescriptors
{
    const string Category = "Csv.SourceGenerator";
    const string UsageCategory = "Usage";

    internal static readonly DiagnosticDescriptor PublicMemberNeedsKey = new(
        id: "CSV001",
        title: "Attribute public members of Csv objects",
        category: UsageCategory,
        messageFormat: "Public members of CsvObject-attributed types require either ColumnAttribute or IgnoreMemberAttribute: {0}.{1}", // type.Name + "." + item.Name
        description: "Public member must be marked with ColumnAttribute or IgnoreMemberAttribute.",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor TypeMustBeCsvObject = new(
        id: "CSV002",
        title: "Use CsvObjectAttribute",
        category: UsageCategory,
        messageFormat: "Type must be marked with CsvObjectAttribute: {0}", // type.Name
        description: "Type must be marked with CsvObjectAttribute.",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    const string InvalidCsvObjectId = "CSV003";
    const string InvalidCsvObjectTitle = "CsvObject validation";

    internal static readonly DiagnosticDescriptor InvalidCsvObject = new(
        id: InvalidCsvObjectId,
        title: InvalidCsvObjectTitle,
        category: UsageCategory,
        messageFormat: "Invalid CsvObject definition: {0}",
        description: "Invalid CsvObject definition.",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor KeyAnnotatedMemberInMapMode = new(
        id: InvalidCsvObjectId,
        title: InvalidCsvObjectTitle,
        category: Category,
        messageFormat: "Types in map mode should not annotate members with ColumnAttribute",
        description: "When in map mode (by compilation setting or with [CsvObject(true)]), internal and public members are automatically included in serialization and should not be annotated with ColumnAttribute.",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MustBePartial = new(
        id: "CSV004",
        title: "Csv serializable type must be partial",
        messageFormat: "The Csv serializable object '{0}' must be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NestedNotAllowed = new(
        id: "CSV005",
        title: "Csv serializable type must not be nested",
        messageFormat: "The Csv serializable object '{0}' must be not nested",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AbstractNotAllowed = new(
        id: "CSV006",
        title: "Csv serializable type must not abstract",
        messageFormat: "The Csv serializable object '{0}' must be not abstract",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}