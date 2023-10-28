using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using System.Linq;

namespace FS.EntityFramework.DeepDive.Extensions;

internal static class FormattingExtensions
{
    private static readonly AdhocWorkspace _workspace = new();

    private static readonly OptionSet _options = _workspace.Options
        .WithChangedOption(CSharpFormattingOptions.IndentBraces, false);

    public static string FormatSourceSnippet(this string? codeSnippet)
    {
        if (codeSnippet is null)
            return string.Empty;

        var nodes = CSharpSyntaxTree.ParseText(codeSnippet).GetRoot().DescendantNodes().ToList();
        var methodSource = (CSharpSyntaxNode?)nodes.OfType<BlockSyntax>().FirstOrDefault() ?? nodes.OfType<ArrowExpressionClauseSyntax>().FirstOrDefault()?.Expression;
        if (methodSource == null)
            return string.Empty;

        var methodInnerSource = methodSource.ToFullString().Trim(' ', '{', '}');
        var sourcecode = CSharpSyntaxTree.ParseText(methodInnerSource).GetRoot();
        var formattedCode = Formatter
            .Format(sourcecode, _workspace, _options)
            .ToFullString()
            .Trim();

        return formattedCode;
    }
}