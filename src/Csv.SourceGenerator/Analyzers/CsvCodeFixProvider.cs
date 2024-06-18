using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Csv.SourceGenerator.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CsvCodeFixProvider)), Shared]
public class CsvCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(DiagnosticDescriptors.PublicMemberNeedsKey.Id, DiagnosticDescriptors.TypeMustBeCsvObject.Id);
        }
    }

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) as CompilationUnitSyntax;
        if (root is null)
        {
            return;
        }

        var model = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (model is null)
        {
            return;
        }

        var targetNode = root.FindNode(context.Span);
        var myTypeInfo = model.GetTypeInfo(targetNode, context.CancellationToken);

        var typeName = context.Diagnostics[0]?.Properties.GetValueOrDefault("type", null);
        var namedSymbol =
            myTypeInfo.Type as INamedTypeSymbol ??
            (typeName is not null ? model.Compilation.GetTypeByMetadataName(typeName.Replace("global::", string.Empty)) : null);

        if (namedSymbol is null)
        {
            var property = targetNode as PropertyDeclarationSyntax;
            var field = targetNode as FieldDeclarationSyntax;
            var dec = targetNode as VariableDeclaratorSyntax;
            var identifierName = targetNode as IdentifierNameSyntax;

            ITypeSymbol? targetType = null;
            if (property == null && field == null)
            {
                var typeDeclare = targetNode as TypeDeclarationSyntax;
                if (typeDeclare != null)
                {
                    targetType = model.GetDeclaredSymbol(typeDeclare);
                }
                else if (dec != null)
                {
                    var fieldOrProperty = model.GetDeclaredSymbol(dec);
                    if (context.Diagnostics[0].Id == DiagnosticDescriptors.TypeMustBeCsvObject.Id)
                    {
                        targetType = (fieldOrProperty as IPropertySymbol)?.Type;
                        if (targetType == null)
                        {
                            targetType = (fieldOrProperty as IFieldSymbol)?.Type;
                        }
                    }
                    else
                    {
                        targetType = (fieldOrProperty as IPropertySymbol)?.ContainingType;
                        if (targetType == null)
                        {
                            targetType = (fieldOrProperty as IFieldSymbol)?.ContainingType;
                        }
                    }
                }
            }
            else
            {
                if (context.Diagnostics[0].Id == DiagnosticDescriptors.TypeMustBeCsvObject.Id)
                {
                    targetType = (property != null)
                        ? model.GetDeclaredSymbol(property)?.Type
                        : (model.GetDeclaredSymbol(field!) as IFieldSymbol)?.Type;
                }
                else
                {
                    targetType = (property != null)
                        ? model.GetDeclaredSymbol(property)?.ContainingType
                        : (model.GetDeclaredSymbol(field!) as IFieldSymbol)?.ContainingType;
                }
            }

            if (targetType == null)
            {
                return;
            }

            if (targetType.TypeKind == TypeKind.Array)
            {
                targetType = ((IArrayTypeSymbol)targetType).ElementType;
            }

            namedSymbol = targetType as INamedTypeSymbol;
            if (namedSymbol == null)
            {
                return;
            }
        }

        var action = CodeAction.Create("Add ColumnAttribute", c => AddColumnAttributeAsync(context.Document, namedSymbol, c), "CsvAnalyzer.AddColumnAttribute");

        context.RegisterCodeFix(action, context.Diagnostics.First()); // use single.
    }

    static async Task<Solution> AddColumnAttributeAsync(Document document, INamedTypeSymbol type, CancellationToken cancellationToken)
    {
        var solutionEditor = new SolutionEditor(document.Project.Solution);

        var targets = type.GetAllMembers()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Where(x => x.Kind == SymbolKind.Property || x.Kind == SymbolKind.Field)
            .Where(x => x.GetAttributes().FindAttributeShortName("IgnoreMemberAttribute") == null)
            .Where(x => !x.IsStatic)
            .Where(x =>
            {
                return x switch
                {
                    IPropertySymbol p => p.ExplicitInterfaceImplementations.Length == 0,
                    IFieldSymbol f => !f.IsImplicitlyDeclared,
                    _ => throw new NotSupportedException("Unsupported member type."),
                };
            })
            .ToArray();

        var startOrder = targets
            .Select(x => x.GetAttributes().FindAttributeShortName("ColumnAttribute"))
            .Where(x => x != null)
            .Select(x => x.ConstructorArguments[0])
            .Where(x => !x.IsNull)
            .Where(x => x.Value is int)
            .Select(x => (int)x.Value!)
            .DefaultIfEmpty(-1) // if empty, start from zero.
            .Max() + 1;

        foreach (var member in targets)
        {
            if (member.GetAttributes().FindAttributeShortName("ColumnAttribute") is null)
            {
                var node = await member.DeclaringSyntaxReferences[0].GetSyntaxAsync(cancellationToken).ConfigureAwait(false);
                var documentEditor = await solutionEditor.GetDocumentEditorAsync(document.Project.Solution.GetDocumentId(node.SyntaxTree), cancellationToken).ConfigureAwait(false);
                var syntaxGenerator = SyntaxGenerator.GetGenerator(documentEditor.OriginalDocument);
                documentEditor.AddAttribute(node, syntaxGenerator.Attribute("Csv.Annotations.ColumnAttribute", syntaxGenerator.LiteralExpression(startOrder++)));
            }
        }

        return solutionEditor.GetChangedSolution();
    }
}