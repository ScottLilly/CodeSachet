using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Threading;

namespace RoboReviewer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RoboReviewerAnalyzer : DiagnosticAnalyzer
    {
        private const string CATEGORY = "Quality";

        private static readonly LocalizableString s_title = 
            new LocalizableResourceString(
                nameof(Resources.AnalyzerTitle), 
                Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_messageFormat = 
            new LocalizableResourceString(
                nameof(Resources.AnalyzerMessageFormat), 
                Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_description = 
            new LocalizableResourceString(
                nameof(Resources.AnalyzerDescription), 
                Resources.ResourceManager, typeof(Resources));
        private static readonly DiagnosticDescriptor s_rule = 
            new DiagnosticDescriptor(
                DiagnosticId, s_title, s_messageFormat, CATEGORY, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true, 
                description: s_description);

        public const string DiagnosticId = "RoboReviewer";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(s_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

            context.RegisterSyntaxTreeAction(syntaxTreeContext =>
            {
                var root = 
                    syntaxTreeContext.Tree.GetRoot(syntaxTreeContext.CancellationToken);

                foreach (var statement in root.DescendantNodes().OfType<StatementSyntax>())
                {
                    if (statement is BlockSyntax)
                    {
                        continue;
                    }

                    if (statement.Parent is StatementSyntax &&
                        !(statement.Parent is BlockSyntax))
                    {
                        syntaxTreeContext
                            .ReportDiagnostic(
                                Diagnostic.Create(
                                    s_rule, 
                                    statement.GetFirstToken().GetLocation()));
                    }
                }
            });
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Find just those named type symbols with names containing lowercase letters.
            if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(s_rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
