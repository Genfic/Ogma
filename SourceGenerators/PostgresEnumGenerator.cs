using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceGenerators.Attributes;
using SourceGenerators.Helpers;
using SourceGenerators.Strings;

namespace SourceGenerators;

[Generator]
public class PostgresEnumSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        context.RegisterForPostInitialization(ctx =>
            ctx.AddSource("PostgresEnumAttribute.cs", SourceText.From(PostgresEnumGeneratorStrings.Attribute, Encoding.UTF8))
        );
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not SyntaxReceiver receiver) return;

        var code = new StringBuilder();
        code.AppendLine("namespace Ogma3.Generated;\n");
        code.AppendLine("public class TestClass // v3");
        code.AppendLine("{");
        code.AppendLine("   public void Testing()");
        code.AppendLine("   {");

        foreach (var workItem in receiver.EnumsFound)
        {
            code.AppendLine($"      var {workItem.Name.ToLower()} = nameof({workItem.Name});");
            code.AppendLine($"      System.Console.WriteLine({workItem.Name.ToLower()});");
        }

        code.AppendLine("   }");
        code.AppendLine("}");
        code.AppendLine("/*");
        code.AppendLine(string.Join(Environment.NewLine, receiver.Log));
        code.AppendLine("*/");

        context.AddSource("PostgresEnums.g.cs", code.ToString());
    }

    private class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<Type> EnumsFound { get; } = new();
        public List<string> Log { get; } = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is not EnumDeclarationSyntax eds) return;

            var attributes = eds.AttributeLists.SelectMany(node => node.Attributes).ToImmutableArray();

            var hasAttribute = attributes.Any(att => att.ToString() == nameof(PgTestAttr));

            if (!hasAttribute) return;

            Log.Add($"Registered enum {eds.Identifier.ToString()} {eds.GetNamespace()}");

            EnumsFound.Add(eds.GetReference().GetType());
        }
    }
}