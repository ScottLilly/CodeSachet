using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeSachet
{
    internal static class ExtensionMethods
    {
        internal static bool IsTooDeep(this SyntaxNode node)
        {
            var methodNode = 
                node.Ancestors().FirstOrDefault(a => a is MethodDeclarationSyntax);

            if (methodNode == null)
            {
                return false;
            }

            return node.Parent != methodNode && 
                   node.Parent.Parent != methodNode && 
                   node.Parent.Parent.Parent != methodNode &&
                   node.Parent.Parent.Parent.Parent != methodNode;
        }
    }
}