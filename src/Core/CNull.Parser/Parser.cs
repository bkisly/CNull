using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Parser.Exceptions;
using CNull.Parser.Productions;

namespace CNull.Parser
{
    public class Parser : IParser
    {
        private readonly ILexer _lexer;
        private readonly IErrorHandler _errorHandler;
        private Token _currentToken = null!;

        public Parser(ILexer lexer, IErrorHandler errorHandler)
        {
            _lexer = lexer;
            _errorHandler = errorHandler;

            ConsumeToken();
        }

        public Program Parse()
        {
            var importDirectives = new List<ImportDirective>();
            var functionDefinitions = new List<FunctionDefinition>();

            while (ParseImportDirective() is { } directive)
                importDirectives.Add(directive);

            while (ParseFunctionDefinition() is { } functionDefinition)
                functionDefinitions.Add(functionDefinition);

            return new Program(importDirectives, functionDefinitions);
        }

        #region Common methods

        private void ConsumeToken() => _currentToken = _lexer.GetNextToken();

        /// <summary>
        /// Wraps production factory methods in order to cancel creation if an unexpected token has occurred.
        /// </summary>
        /// <typeparam name="T">Type of the production to build.</typeparam>
        /// <param name="productionFactory">Factory method of the production.</param>
        /// <returns>The build production.</returns>
        private static T? BuilderWrapper<T>(Func<T?> productionFactory) where T : class?
        {
            try
            {
                return productionFactory.Invoke();
            }
            catch (UnexpectedTokenException)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the given token matches the expected type.
        /// </summary>
        /// <param name="token">Token to validate.</param>
        /// <param name="expectedType">Expected token type.</param>
        /// <param name="errorToThrow">Error to be passed to the error handler.</param>
        /// <exception cref="UnexpectedTokenException">Thrown when the type of the token doesn't match the expected type.</exception>
        private void ValidateToken(Token token, TokenType expectedType, ICompilationError errorToThrow)
        {
            if (token.TokenType == expectedType)
                return;

            RaiseFactoryError(errorToThrow);
        }

        /// <summary>
        /// Raises an error during creation of a syntactic production.
        /// </summary>
        private void RaiseFactoryError(ICompilationError errorToThrow)
        {
            _errorHandler.RaiseCompilationError(errorToThrow);
            throw new UnexpectedTokenException();
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier;</c>  
        /// </summary>
        /// <returns></returns>
        private ImportDirective? ParseImportDirective() => BuilderWrapper<ImportDirective?>(() =>
        {
            ValidateToken(_currentToken, TokenType.ImportKeyword, null!);
            ConsumeToken();
            ValidateToken(_currentToken, TokenType.Identifier, null!);

            var moduleName = (_currentToken as Token<string>)?.Value;
            ConsumeToken();

            ValidateToken(_currentToken, TokenType.Identifier, null!);

            var functionName = (_currentToken as Token<string>)?.Value;
            ConsumeToken();

            ValidateToken(_currentToken, TokenType.SemicolonOperator, null!);
            ConsumeToken();

            if (moduleName is not null && functionName is not null)
                return new ImportDirective(moduleName, functionName);

            // @TODO: invalid method path
            _errorHandler.RaiseCompilationError(null!);
            return null;

        });

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        /// <returns></returns>
        private FunctionDefinition? ParseFunctionDefinition() => BuilderWrapper<FunctionDefinition?>(() =>
        {
            var returnType = ParseReturnType();

            ValidateToken(_currentToken, TokenType.Identifier, null!);
            var identifier = (_currentToken as Token<string>)?.Value;

            if (string.IsNullOrEmpty(identifier))
            {
                _errorHandler.RaiseCompilationError(null!);
                throw new UnexpectedTokenException();
            }

            ConsumeToken();

            ValidateToken(_currentToken, TokenType.LeftParenthesisOperator, null!);
            ConsumeToken();

            var parameters = ParseParametersList();
            
            ValidateToken(_currentToken, TokenType.RightParenthesisOperator, null!);
            ConsumeToken();

            var functionBody = ParseBlockStatement();

            return new FunctionDefinition(returnType, identifier, parameters, functionBody);
        });

        #endregion

        #region Types and parameters builders

        private ReturnType? ParseReturnType() => BuilderWrapper<ReturnType?>(() =>
        {
            throw new NotImplementedException();
        });

        private IEnumerable<Parameter>? ParseParametersList() => BuilderWrapper<IEnumerable<Parameter>?>(() =>
        {
            throw new NotImplementedException();
        });

        #endregion

        #region Statements builders

        private BlockStatement? ParseBlockStatement() => BuilderWrapper<BlockStatement?>(() =>
        {
            throw new NotImplementedException();
        });

        #endregion
    }
}
