using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeSachet
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CodeSachetAnalyzer : DiagnosticAnalyzer
    {
        private const string CATEGORY = "CognitiveLoad";

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

        public const string DiagnosticId = "SLCS1001";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
            ImmutableArray.Create(s_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(DetectExcessiveIndentation);
        }

        private static void DetectExcessiveIndentation(SyntaxTreeAnalysisContext syntaxTreeContext)
        {
            var root =
                syntaxTreeContext.Tree.GetRoot(syntaxTreeContext.CancellationToken);

            var statements =
                root.DescendantNodes().OfType<StatementSyntax>();

            foreach (var statement in statements)
            {
                if (statement is BlockSyntax)
                {
                    // We only want to check the "if", "foreach", etc line,
                    // not the code block under it
                    continue;
                }

                // Check if there is a child block of code or statement
                bool hasSubsequentBlock = false;

                foreach (var subsequentBlock in statement.DescendantNodes())
                {
                    if (subsequentBlock is BlockSyntax ||
                        subsequentBlock is StatementSyntax)
                    {
                        hasSubsequentBlock = true;
                        break;
                    }
                }

                // There is no "next level", so check the next line
                if (!hasSubsequentBlock)
                {
                    continue;
                }

                if (statement.IsTooDeep())
                {
                    syntaxTreeContext
                        .ReportDiagnostic(
                            Diagnostic.Create(
                                s_rule,
                                statement.GetFirstToken().GetLocation()));
                }
            }
        }
    }
}
