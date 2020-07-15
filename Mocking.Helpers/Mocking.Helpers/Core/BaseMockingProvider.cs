using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mocking.Helpers.Interfaces
{
    public abstract class BaseMockingProvider
    {
        public abstract string AssemblyName { get; }
        public abstract string[] MockingMethodNames { get; }
        public abstract string MockingWildcardMethod { get; }

        protected string FormatMockingWildcardMethod(string parameterTypeName)
        {
            return string.Format(this.MockingWildcardMethod, parameterTypeName);
        }

        public virtual string GenerateSuggestionForParameterWildcard(SemanticModel model, IMethodSymbol methodSymbol, ArgumentListSyntax arguments)
        {
            var suggestionParameter = methodSymbol.Parameters.Aggregate(string.Empty, (acc, p) => $"{acc}, {this.FormatMockingWildcardMethod(p.Type.ToMinimalDisplayString(model, arguments.SpanStart))}").TrimStart(',').Trim();
            return suggestionParameter;
        }

        public virtual string GenerateSuggestionForParameterWildcardOfType(SemanticModel model, ITypeSymbol typeSymbol, ArgumentListSyntax arguments)
        {
            var suggestionParameter = this.FormatMockingWildcardMethod(typeSymbol.ToMinimalDisplayString(model, arguments.SpanStart));
            return suggestionParameter;
        }
    }
}
