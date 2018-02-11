using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mocking.Helpers.Moq
{
    public class MoqProvider
    {
        public string MockingMethodName { get; } = "Setup";
        public string AssemblyName { get; } = "Moq";
        public string MockingWildcardMethod { get; } = "It.IsAny<{0}>()";

        string FormatMockingWildcardMethod(string parameterTypeName)
        {
            return string.Format(this.MockingWildcardMethod, parameterTypeName);
        }

        public string GenerateSuggestionForParameterWildcard(SemanticModel model, IMethodSymbol methodSymbol, ArgumentListSyntax arguments)
        {
            var suggestionParameter = methodSymbol.Parameters.Aggregate(String.Empty, (acc, p) => $"{acc}, {this.FormatMockingWildcardMethod(p.Type.ToMinimalDisplayString(model, arguments.SpanStart))}").TrimStart(',').Trim();
            return suggestionParameter;
        }

        public string GenerateSuggestionForParameterWildcardOfType(SemanticModel model, ITypeSymbol typeSymbol, ArgumentListSyntax arguments)
        {
            var suggestionParameter = this.FormatMockingWildcardMethod(typeSymbol.ToMinimalDisplayString(model, arguments.SpanStart));
            return suggestionParameter;
        }
    }
}
