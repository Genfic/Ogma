using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Ogma3.Codegen.PostgresEnumRegistrationGenerator
{
    [Generator]
    public class PostgresEnumRegistrationGenerator : ISourceGenerator
    {
        private readonly List<string> _logs = new();
        private const string ClassName = "GeneratedDbContext";

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            var sb = new StringBuilder();
            
            _logs.Add($"Looking for enums in {context.Compilation.Assembly.Name} with {nameof(PostgresEnumAttribute)[..^9]} attribute");

            
            var enums = new List<EnumDeclarationSyntax>();
            foreach (var symbol in receiver.Enums)
            {
                var attributes = symbol.AttributeLists
                    .SelectMany(x => x.Attributes)
                    .ToImmutableArray();

                if (!attributes.Any(x => x.Name.ToString() == nameof(PostgresEnumAttribute)[..^9])) continue;
                
                _logs.Add($"\tFound enum {symbol.Identifier.Text}");
                enums.Add(symbol);
            }

            sb.Append('\n');
            sb.Append(@"using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;");
            sb.Append('\n');

            sb.Append(string.Join('\n', enums.Select(e => $"using {e.Identifier.Parent.Parent.ToString().Split('{')[0].Replace("namespace", "").Trim()};")));
            sb.Append('\n');
            
            sb.Append($"internal partial class {ClassName}<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : IdentityDbContext <TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>");
            sb.Append('\n');
            sb.Append(@"        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>");
            sb.Append("\n{\n");

            sb.Append(@$"
    public {ClassName}(DbContextOptions options) : base(options)
    {{
        NpgsqlConnection.GlobalTypeMapper
            {string.Join("\n\t\t\t", enums.Select(e => $".MapEnum<{e.Identifier.Text}>()"))};
    }}");
            
            sb.Append("\n");
            
            sb.Append(@$"
    protected override void OnModelCreating(ModelBuilder builder)
    {{
        base.OnModelCreating(builder);
        builder
            {string.Join("\n\t\t\t", enums.Select(e => $".HasPostgresEnum<{e.Identifier.Text}>()"))};
    }}");

            sb.Append("\n}");
            
            context.AddSource("GeneratedDbContext", SourceText.From($"/*\n{string.Join('\n', _logs)}\n*/{sb}", Encoding.UTF8));
        }
        
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<EnumDeclarationSyntax> Enums { get; } = new();
            
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is EnumDeclarationSyntax { AttributeLists: { Count: > 0 } } eds)
                {
                    Enums.Add(eds);
                }
            }
        }
    }
}