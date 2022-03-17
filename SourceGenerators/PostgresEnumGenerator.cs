using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Helpers;
using SourceGenerators.Strings;

namespace SourceGenerators;

[Generator]
public class PostgresEnumSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        context.RegisterPostInitializationOutput(ctx =>
            ctx.AddSource("PostgresEnumAttribute.cs", SourceText.From(PostgresEnumGeneratorStrings.Attribute, Encoding.UTF8))
        );

        IncrementalValuesProvider<EnumDeclarationSyntax> enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => ctx.GetSemanticTargetForGeneration()
            )
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> compilationAndEnums
            = context.CompilationProvider.Combine(enumDeclarations.Collect());
        
        context.RegisterSourceOutput(compilationAndEnums, static (spc, source) 
            => Execute(source.Item1, source.Item2, spc));
    }

    private static string GenerateExtensionClass(List<EnumToGenerate> enumsToGenerate)
    {
        var sb = new StringBuilder();
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Npgsql;");
        sb.AppendLine("using Npgsql.TypeMapping;");
        sb.AppendLine("namespace PostgresEnumHelpers.Generated;\n");
        sb.AppendLine("public static class PostgresEnumExtensions");
        sb.AppendLine("{");

        sb.AppendLine("\tpublic static INpgsqlTypeMapper MapPostgresEnums(this INpgsqlTypeMapper mapper, INpgsqlNameTranslator? translator = null)");
        sb.AppendLine("\t{");
        foreach (var e in enumsToGenerate)
        {
            sb.AppendLine($"\t\tmapper.MapEnum<{e.EnumName}>(translator);");
        }
        sb.AppendLine("\t}");

        sb.AppendLine("\tpublic static void RegisterPostgresEnums(this ModelBuilder builder, string? schema = null, INpgsqlNameTranslator? translator = null)");
        sb.AppendLine("\t{");
        foreach (var e in enumsToGenerate)
        {
            sb.AppendLine($"\t\tbuilder.HasPostgresEnum<{e.EnumName}>(schema, \"{e.Alias}\", translator);");
        }
        sb.AppendLine("\t}");

        sb.Append("}");

        return sb.ToString();
    }


    private static void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context)
    {
        if (enums.IsDefaultOrEmpty) return;

        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        var distinctEnums = enums.Distinct();

        // Convert each EnumDeclarationSyntax to an EnumToGenerate
        var enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

        // If there were errors in the EnumDeclarationSyntax, we won't create an
        // EnumToGenerate for it, so make sure we have something to generate
        if (enumsToGenerate.Count <= 0) return;
        
        // generate the source code and add it to the output
        var result = GenerateExtensionClass(enumsToGenerate);
        context.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
    }

    private static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
    {
        // Create a list to hold our output
        var enumsToGenerate = new List<EnumToGenerate>();
        // Get the semantic representation of our marker attribute 
        var enumAttribute = compilation.GetTypeByMetadataName("PostgresEnumHelpers.Generated.PostgresEnumAttribute");

        if (enumAttribute is null) return enumsToGenerate;

        foreach (var enumDeclarationSyntax in enums)
        {
            ct.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            var semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol) continue;

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            var enumName = enumSymbol.ToString();

            var alias = (string?)null;
            foreach (var attributeData in enumSymbol.GetAttributes())
            {
                if (!enumAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default)) continue;

                foreach (var argument in attributeData.NamedArguments)
                {
                    if (argument.Key == "Alias" && argument.Value.Value?.ToString() is { } n)
                    {
                        alias = n;
                    }
                }
            }

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new EnumToGenerate(enumName, alias));
        }

        return enumsToGenerate;
    }


    public readonly struct EnumToGenerate
    {
        public readonly string EnumName;
        public readonly string? Alias;

        public EnumToGenerate(string enumName, string? alias)
        {
            EnumName = enumName;
            Alias = alias;
        }
    }


    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is EnumDeclarationSyntax { AttributeLists.Count: > 0 };

}