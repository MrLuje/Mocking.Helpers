using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mocking.Helpers
{
    public static class SyntaxHelpers
    {
        internal static bool IsMethodNamed(InvocationExpressionSyntax invocation, string expectedMethodName)
        {
            var method = invocation.Expression as MemberAccessExpressionSyntax;
            var methodName = method?.Name.Identifier.ValueText;
            return methodName == expectedMethodName;
        }

        static internal bool IsCommaOrOpenParenthesis(SyntaxToken token)
        {
            return token.IsKind(SyntaxKind.OpenParenToken) || token.IsKind(SyntaxKind.CommaToken);
        }

        /// <summary>
        /// Get token at current position
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentPosition"></param>
        /// <returns></returns>
        static internal SyntaxToken GetSelectedTokens(SyntaxNode node, int currentPosition)
        {
            return node.DescendantNodes(n => n.FullSpan.Contains(currentPosition - 1)) // node under cursor
                       .OfType<ArgumentListSyntax>()
                       .OrderBy(n => n.FullSpan.Length)
                       .SelectMany(n => n.ChildTokens()
                                         .Where(IsCommaOrOpenParenthesis)
                                         .Where(t => t.FullSpan.Contains(currentPosition - 1)))
                       .FirstOrDefault();
        }

        /// <summary>
        /// Get all signature candidates which have parameters in a lambda call
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <param name="setupMethodInvocation"></param>
        /// <returns></returns>
        static internal IEnumerable<IMethodSymbol> GetCandidatesMockedMethodSignaturesForLambda(SemanticModel semanticModel, InvocationExpressionSyntax setupMethodInvocation)
        {
            var setupLambda = setupMethodInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression as LambdaExpressionSyntax;

            var methodToMockInLambda = setupLambda?.Body as InvocationExpressionSyntax;
            return GetCandidatesMockedMethodSignatures(semanticModel, methodToMockInLambda);
        }

        /// <summary>
        /// Get all signature candidates which have parameters in a lambda call
        /// </summary>
        /// <param name="semanticModel"></param>
        /// <param name="methodToMockInLambda"></param>
        /// <returns></returns>
        static internal IEnumerable<IMethodSymbol> GetCandidatesMockedMethodSignatures(SemanticModel semanticModel, InvocationExpressionSyntax methodToMockInLambda)
        {
            if (methodToMockInLambda == null) return Enumerable.Empty<IMethodSymbol>();

            var mockSignatures = new List<IMethodSymbol>();
            var symbolInfo = semanticModel.GetSymbolInfo(methodToMockInLambda);

            // not what are searching but still a method
            if (symbolInfo.CandidateReason == CandidateReason.None && symbolInfo.Symbol is IMethodSymbol methodSymbol)
            {
                mockSignatures.Add(methodSymbol);
            }
            else if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
            {
                mockSignatures.AddRange(symbolInfo.CandidateSymbols.OfType<IMethodSymbol>());
            }
            return mockSignatures.Where(m => m.Parameters.Any());
        }
    }
}
