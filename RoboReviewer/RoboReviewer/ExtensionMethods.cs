using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoboReviewer
{
    internal static class ExtensionMethods
    {
        internal static bool IsTooDeep(this SyntaxNode node)
        {
            bool isInsideFunction = false;
            int depth = 0;

            while (!(node is MethodDeclarationSyntax))
            {
                if (node.Parent is MethodDeclarationSyntax)
                {
                    isInsideFunction = true;
                }

                node = node.Parent;
                depth++;
            }

            return isInsideFunction && depth > 2;
        }

        private static bool IsDeclaration(this SyntaxNode node)
        {
            SyntaxKind syntaxKind = node.Kind();

            return syntaxKind == SyntaxKind.NamespaceDeclaration ||
                   syntaxKind == SyntaxKind.ClassDeclaration ||
                   syntaxKind == SyntaxKind.MethodDeclaration;
        }

        private static bool IsNotDeclaration(this SyntaxNode node)
        {
            return !node.IsDeclaration();
        }
    }
}