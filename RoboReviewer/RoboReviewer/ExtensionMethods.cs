using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoboReviewer
{
    internal static class ExtensionMethods
    {
        internal static bool IsTooDeep(this SyntaxNode node)
        {
            if (!node.Ancestors().Any(a => a is MethodDeclarationSyntax))
            {
                return false;
            }

            int depth = 0;

            while (node != null &&
                   !(node is MethodDeclarationSyntax))
            {
                node = node.Parent;
                depth++;
            }

            return depth > 3;
        }
    }
}