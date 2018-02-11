using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mocking.Helpers.Moq;
using System;
using System.Linq;

namespace Mocking.Helpers
{
    public class CompletionService
    {
        private CompletionContext _context;
        private CompletionItemRules _preselectCompletions;
        private CompletionItemRules _defaultCompletions;
        private readonly SyntaxToken _token;
        private readonly SemanticModel _semanticModel;
        private readonly MoqProvider _provider;

        public CompletionService(CompletionContext context, SyntaxToken token, SemanticModel semanticModel, MoqProvider provider)
        {
            _context = context;
            _token = token;
            _semanticModel = semanticModel;

            _preselectCompletions = CompletionItemRules.Default.WithMatchPriority(MatchPriority.Preselect).WithSelectionBehavior(CompletionItemSelectionBehavior.SoftSelection);
            _defaultCompletions = CompletionItemRules.Default.WithSelectionBehavior(CompletionItemSelectionBehavior.SoftSelection);
            _provider = provider;
        }

        public void AddSuggestionsForMethod(IMethodSymbol methodSymbol, ArgumentListSyntax arguments)
        {
            switch (this._token.Kind())
            {
                // Starting parenthesis before parameters
                case SyntaxKind.OpenParenToken:
                    AddSuggestionsForAllParameters(methodSymbol, arguments);
                    break;
                // Somewhere after first parameter
                default:
                    AddSuggestionsForNextParameters(methodSymbol, arguments);
                    break;
            }
        }

        private void AddSuggestionsForAllParameters(IMethodSymbol methodSymbol, ArgumentListSyntax arguments)
        {
            // Suggestion for all parameters
            this.AddSuggestion(this._provider.GenerateSuggestionForParameterWildcard(this._semanticModel, methodSymbol, arguments));

            // Highlight suggestion of first parameter
            if (methodSymbol.Parameters.Length > 1)
            {
                this.AddDefaultSuggestion(this._provider.GenerateSuggestionForParameterWildcardOfType(_semanticModel, methodSymbol.Parameters[0].Type, arguments));
            }
        }

        private void AddSuggestionsForNextParameters(IMethodSymbol methodSymbol, ArgumentListSyntax arguments)
        {
            var currentParamIndex = arguments.ChildTokens()
                                             .Where(t => t.IsKind(SyntaxKind.CommaToken))
                                             .ToList()
                                             .IndexOf(this._token);
            var nextParamIndex = currentParamIndex + 1;

            if (methodSymbol.Parameters.Length <= nextParamIndex) return;

            this.AddDefaultSuggestion(this._provider.GenerateSuggestionForParameterWildcardOfType(_semanticModel, methodSymbol.Parameters[nextParamIndex].Type, arguments));
        }

        void AddSuggestion(string suggestion)
        {
            this._context.AddItem(CompletionItem.Create(suggestion, rules: _preselectCompletions));
        }

        void AddDefaultSuggestion(string suggestion)
        {
            this._context.AddItem(CompletionItem.Create(suggestion, rules: _defaultCompletions));
        }
    }
}
