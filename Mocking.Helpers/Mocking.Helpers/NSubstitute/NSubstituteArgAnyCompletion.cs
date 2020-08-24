using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mocking.Helpers.NSubstitute
{
    [ExportCompletionProvider(nameof(NSubstituteArgAnyCompletion), LanguageNames.CSharp)]
    public class NSubstituteArgAnyCompletion : CompletionProvider
    {
        private NSubstituteProvider _provider;

        public NSubstituteArgAnyCompletion()
        {
            this._provider = new NSubstituteProvider();
            IsSubstituteForMethod = (InvocationExpressionSyntax invocation) => this._provider.MockingMethodNames.Any(methodName => SyntaxHelpers.IsMethodNamed(invocation, methodName));
        }

        internal readonly Func<InvocationExpressionSyntax, bool> IsSubstituteForMethod;

        public override async Task ProvideCompletionsAsync(CompletionContext context)
        {
            try
            {
                if (!context.Document.SupportsSemanticModel || !context.Document.SupportsSyntaxTree) return;

                var hasNSubstituteReferenced = context.Document.Project.MetadataReferences.Any(r => r.Display.Contains(this._provider.AssemblyName));
                if (!hasNSubstituteReferenced) return;

                var syntaxRoot = await context.Document.GetSyntaxRootAsync();
                var token = SyntaxHelpers.GetSelectedTokens(syntaxRoot, context.Position);

                // Not in an opened method
                if (token.Parent == null) return;

                var mockedMethodArgumentList = token.Parent as ArgumentListSyntax;
                var mockedMethodInvocation = mockedMethodArgumentList.Ancestors()
                                                                     .OfType<InvocationExpressionSyntax>()
                                                                     .FirstOrDefault();

                if (mockedMethodInvocation == null) return;

                var semanticModel = await context.Document.GetSemanticModelAsync();
                var matchingMockedMethods = SyntaxHelpers.GetCandidatesMockedMethodSignatures(semanticModel, mockedMethodInvocation);

                var completionService = new CompletionService(context, token, semanticModel, this._provider);

                foreach (IMethodSymbol matchingMockedMethodSymbol in matchingMockedMethods)
                {
                    completionService.AddSuggestionsForMethod(matchingMockedMethodSymbol, mockedMethodArgumentList);
                }
            }
            catch
            {
            }
        }
    }
}

