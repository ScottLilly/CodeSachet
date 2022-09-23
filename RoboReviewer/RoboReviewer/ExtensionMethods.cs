using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoboReviewer
{
    internal static class ExtensionMethods
    {
        internal static bool IsTooDeep(this SyntaxNode node)
        {
            bool isInsideFunction = false;
            int depth = 0;

            while (node != null &&
                   !(node is MethodDeclarationSyntax))
            {
                if (node.Parent is MethodDeclarationSyntax)
                {
                    isInsideFunction = true;
                }

                node = node.Parent;
                depth++;
            }

            return isInsideFunction && depth > 3;
        }
    }
}