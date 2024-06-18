using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csv.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CsvAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        DiagnosticDescriptors.TypeMustBeCsvObject,
        DiagnosticDescriptors.PublicMemberNeedsKey,
        DiagnosticDescriptors.InvalidCsvObject,
        DiagnosticDescriptors.KeyAnnotatedMemberInMapMode);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(context =>
        {
            var typeReferences = ReferenceSymbols.Create(context.Compilation);
            if (typeReferences != null)
            {
                context.RegisterSyntaxNodeAction(c => Analyze(c, typeReferences), SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration);
            }
        });
    }

    static void Analyze(SyntaxNodeAnalysisContext context, ReferenceSymbols typeReferences)
    {
        var typeDeclaration = (TypeDeclarationSyntax)context.Node;
        var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclaration);

        if (declaredSymbol == null) return;

        if ((declaredSymbol.TypeKind is TypeKind.Class or TypeKind.Struct) &&
            declaredSymbol.GetAttributes().Any(x2 => SymbolEqualityComparer.Default.Equals(x2.AttributeClass, typeReferences.CsvObjectAttribute)))
        {
            AnalyzeObject(declaredSymbol, context, typeReferences);
        }
    }

    static void AnalyzeObject(INamedTypeSymbol type, SyntaxNodeAnalysisContext context, ReferenceSymbols typeReferences)
    {
        var isClass = !type.IsValueType;

        var contractAttr = type.GetAttributes().FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.CsvObjectAttribute));
        if (contractAttr == null)
        {
            var location = type.Locations[0];
            var targetName = type.Name;

            var typeInfo = ImmutableDictionary.Create<string, string?>().Add("type", type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TypeMustBeCsvObject, location, typeInfo, targetName));
            return;
        }

        var isIntKey = true;
        var intMembers = new HashSet<int>();
        var stringMembers = new HashSet<string>();

        // map mode
        if (contractAttr.ConstructorArguments[0].Value is true)
        {
            isIntKey = false;

            foreach (var item in type.GetAllMembers().OfType<IPropertySymbol>())
            {
                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.IgnoreMemberAttribute)))
                {
                    continue;
                }

                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.ColumnAttribute)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.KeyAnnotatedMemberInMapMode, item.Locations[0]));
                    continue;
                }

                var isReadable = (item.GetMethod != null) && item.GetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;
                var isWritable = (item.SetMethod != null) && item.SetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;

                if (!isReadable && !isWritable)
                {
                    continue;
                }

                stringMembers.Add(item.Name);
            }

            foreach (var item in type.GetAllMembers().OfType<IFieldSymbol>())
            {
                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.IgnoreMemberAttribute)))
                {
                    continue;
                }

                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.ColumnAttribute)))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.KeyAnnotatedMemberInMapMode, item.Locations[0]));
                    continue;
                }

                if (item.IsImplicitlyDeclared)
                {
                    continue;
                }

                var isReadable = item.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;
                var isWritable = item.DeclaredAccessibility == Accessibility.Public && !item.IsReadOnly && !item.IsStatic;

                if (!isReadable && !isWritable)
                {
                    continue;
                }

                stringMembers.Add(item.Name);
            }
        }
        else
        {
            var searchFirst = true;

            foreach (var item in type.GetAllMembers().OfType<IPropertySymbol>())
            {
                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.IgnoreMemberAttribute)))
                {
                    continue;
                }

                var isReadable = (item.GetMethod != null) && item.GetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;
                var isWritable = (item.SetMethod != null) && item.SetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;
                var name = item.Name;
                if (!isReadable && !isWritable)
                {
                    continue;
                }

                var key = item.GetAttributes().FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.ColumnAttribute))?.ConstructorArguments[0];
                if (key == null)
                {
                    var typeInfo = ImmutableDictionary.Create<string, string?>().Add("type", type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PublicMemberNeedsKey, item.Locations[0], typeInfo, type.Name, item.Name));
                    continue;
                }

                var intKey = (key.Value.Value is int v) ? v : (int?)null;
                var stringKey = (key.Value.Value is string v1) ? v1 : null;
                if (intKey == null && stringKey == null)
                {
                    ReportInvalid(context, item, "both IntKey and StringKey are null." + " type: " + type.Name + " member:" + item.Name);
                    break;
                }

                if (searchFirst)
                {
                    searchFirst = false;
                    isIntKey = intKey != null;
                }
                else
                {
                    if ((isIntKey && intKey == null) || (!isIntKey && stringKey == null))
                    {
                        ReportInvalid(context, item, "all members key type must be same." + " type: " + type.Name + " member:" + item.Name);
                        break;
                    }
                }

                if (isIntKey)
                {
                    if (intMembers.Contains(intKey!.Value))
                    {
                        ReportInvalid(context, item, "key is duplicated, all members key must be unique." + " type: " + type.Name + " member:" + item.Name);
                        return;
                    }

                    intMembers.Add((int)intKey);
                }
                else
                {
                    if (stringMembers.Contains(stringKey!))
                    {
                        ReportInvalid(context, item, "key is duplicated, all members key must be unique." + " type: " + type.Name + " member:" + item.Name);
                        return;
                    }

                    stringMembers.Add(stringKey!);
                }
            }

            foreach (var item in type.GetAllMembers().OfType<IFieldSymbol>())
            {
                if (item.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.IgnoreMemberAttribute)))
                {
                    continue;
                }

                if (item.IsImplicitlyDeclared)
                {
                    continue;
                }

                var isReadable = item.DeclaredAccessibility == Accessibility.Public && !item.IsStatic;
                var isWritable = item.DeclaredAccessibility == Accessibility.Public && !item.IsReadOnly && !item.IsStatic;
                var name = item.Name;
                if (!isReadable && !isWritable)
                {
                    continue;
                }

                var key = item.GetAttributes().FirstOrDefault(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, typeReferences.ColumnAttribute))?.ConstructorArguments[0];
                if (key == null)
                {
                    var typeInfo = ImmutableDictionary.Create<string, string?>().Add("type", type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.PublicMemberNeedsKey, item.Locations[0], typeInfo, type.Name, item.Name));
                    continue;
                }

                var intKey = key.Value.Value is int i ? (int?)i : null;
                var stringKey = key.Value.Value as string;
                if (intKey == null && stringKey == null)
                {
                    ReportInvalid(context, item, "both IntKey and StringKey are null." + " type: " + type.Name + " member:" + item.Name);
                    return;
                }

                if (searchFirst)
                {
                    searchFirst = false;
                    isIntKey = intKey != null;
                }
                else
                {
                    if ((isIntKey && intKey == null) || (!isIntKey && stringKey == null))
                    {
                        ReportInvalid(context, item, "all members key type must be same." + " type: " + type.Name + " member:" + item.Name);
                        return;
                    }
                }

                if (intKey.HasValue)
                {
                    if (intMembers.Contains(intKey.Value))
                    {
                        ReportInvalid(context, item, "key is duplicated, all members key must be unique." + " type: " + type.Name + " member:" + item.Name);
                        return;
                    }

                    intMembers.Add(intKey.Value);
                }
                else if (stringKey is not null)
                {
                    if (stringMembers.Contains(stringKey))
                    {
                        ReportInvalid(context, item, "key is duplicated, all members key must be unique." + " type: " + type.Name + " member:" + item.Name);
                        return;
                    }

                    stringMembers.Add(stringKey);
                }
            }
        }
    }

    static void ReportInvalid(SyntaxNodeAnalysisContext context, ISymbol symbol, string message)
    {
        context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InvalidCsvObject, symbol.Locations[0], message));
    }
}