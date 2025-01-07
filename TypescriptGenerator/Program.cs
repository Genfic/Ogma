using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Spectre.Console;

var workspace = MSBuildWorkspace.Create();

var path = AnsiConsole.Ask<string>("Enter path to the project");

var project = await workspace.OpenProjectAsync(path);

var compilation = await project.GetCompilationAsync();

if (compilation is null)
{
	AnsiConsole.MarkupLine("[red]Could not create compilation[/]");
	return;
}

var members = compilation.SyntaxTrees
	.SelectMany(s => s.GetCompilationUnitRoot().Members)
	.Where(m => m.IsKind(SyntaxKind.ClassDeclaration));

AnsiConsole.MarkupLineInterpolated($"{members.Count()}");