using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mocking.Helpers.Moq
{
    [ExportCompletionProvider(nameof(MoqIsAnyCompletion), LanguageNames.CSharp)]
    public class MoqIsAnyCompletion : CompletionProvider
    {
        private MoqProvider _provider;

        public MoqIsAnyCompletion()
        {
            this._provider = new MoqProvider();
            IsMoqSetupMethod = (InvocationExpressionSyntax invocation) => this._provider.MockingMethodNames.Any(methodName => SyntaxHelpers.IsMethodNamed(invocation, methodName));
        }

        internal readonly Func<InvocationExpressionSyntax, bool> IsMoqSetupMethod;

        public override async Task ProvideCompletionsAsync(CompletionContext context)
        {
            try
            {
                if (!context.Document.SupportsSemanticModel || !context.Document.SupportsSyntaxTree) return;

                var hasMoqReferenced = context.Document.Project.MetadataReferences.Any(r => r.Display.Contains(this._provider.AssemblyName));
                if (!hasMoqReferenced) return;

                var syntaxRoot = await context.Document.GetSyntaxRootAsync();
                var token = SyntaxHelpers.GetSelectedTokens(syntaxRoot, context.Position);

                // Not in an opened method
                if (token.Parent == null) return;

                var mockedMethodArgumentList = token.Parent as ArgumentListSyntax;
                var setupMethodInvocation = mockedMethodArgumentList.Ancestors()
                                                                    .OfType<InvocationExpressionSyntax>()
                                                                    .Where(IsMoqSetupMethod)
                                                                    .FirstOrDefault();

                if (setupMethodInvocation == null) return;

                var semanticModel = await context.Document.GetSemanticModelAsync();
                var matchingMockedMethods = SyntaxHelpers.GetCandidatesMockedMethodSignaturesForLambda(semanticModel, setupMethodInvocation);

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
