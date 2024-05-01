using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Parser.Enums;
using CNull.Parser.Exceptions;
using CNull.Parser.Extensions;
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
        private T? BuilderWrapper<T>(Func<T?> productionFactory) where T : class?
        {
            try
            {
                return _currentToken.TokenType == TokenType.End ? null : productionFactory.Invoke();
            }
            catch (UnexpectedTokenException)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the current token matches the expected type.
        /// </summary>
        /// <param name="expectedType">Expected token type.</param>
        /// <param name="errorToThrow">Error to be passed to the error handler.</param>
        /// <exception cref="UnexpectedTokenException">Thrown when the type of the token doesn't match the expected type.</exception>
        private void ValidateCurrentToken(TokenType expectedType, ICompilationError errorToThrow)
        {
            if (_currentToken.TokenType == expectedType)
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

        /// <summary>
        /// Helper method that validates and builds an identifier.
        /// </summary>
        private string ParseIdentifier(Token token)
        {
            ValidateCurrentToken(TokenType.Identifier, null!);

            var identifier = (token as Token<string>)?.Value;

            if (!string.IsNullOrEmpty(identifier))
            {
                ConsumeToken();
                return identifier;
            }

            RaiseFactoryError(null!);
            return null!;
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier;</c>  
        /// </summary>
        /// <returns></returns>
        private ImportDirective? ParseImportDirective() => BuilderWrapper<ImportDirective?>(() =>
        {
            ValidateCurrentToken(TokenType.ImportKeyword, null!);
            ConsumeToken();

            var moduleName = ParseIdentifier(_currentToken);
            ConsumeToken();

            var functionName = ParseIdentifier(_currentToken);
            ConsumeToken();

            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            ConsumeToken();

            return new ImportDirective(moduleName, functionName);
        });

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        /// <returns></returns>
        private FunctionDefinition? ParseFunctionDefinition() => BuilderWrapper<FunctionDefinition?>(() =>
        {
            var returnType = ParseReturnType();
            var identifier = ParseIdentifier(_currentToken);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            ConsumeToken();

            var parameters = ParseParametersList();
            
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
            ConsumeToken();

            var functionBody = ParseBlockStatement();

            return new FunctionDefinition(returnType, identifier, parameters, functionBody);
        });

        #endregion

        #region Types and parameters builders

        private ReturnType? ParseReturnType() => BuilderWrapper<ReturnType?>(() =>
        {
            if (!_currentToken.TokenType.IsReturnType())
                RaiseFactoryError(null!);

            return _currentToken.TokenType switch
            {
                TokenType.VoidKeyword => new ReturnType(),
                TokenType.DictKeyword => ParseDictionaryType(),
                _ => new PrimitiveType((PrimitiveTypes)_currentToken.TokenType)
            };
        });

        private DictionaryType? ParseDictionaryType() => BuilderWrapper<DictionaryType?>(() =>
        {
            ValidateCurrentToken(TokenType.DictKeyword, null!);
            ConsumeToken();

            ValidateCurrentToken(TokenType.LessThanOperator, null!);
            ConsumeToken();

            if(!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var keyType = new PrimitiveType((PrimitiveTypes)_currentToken.TokenType);
            ConsumeToken();

            ValidateCurrentToken(TokenType.CommaOperator, null!);
            ConsumeToken();

            if (!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var valueType = new PrimitiveType((PrimitiveTypes)_currentToken.TokenType);
            ConsumeToken();

            return new DictionaryType(keyType, valueType);
        });

        private IDeclarableType? ParseDeclarableType() => BuilderWrapper<IDeclarableType?>(() =>
        {
            throw new NotImplementedException();
        });

        private IEnumerable<Parameter>? ParseParametersList() => BuilderWrapper<IEnumerable<Parameter>?>(() =>
        {
            var parameters = new List<Parameter>();
            var expectNewParameter = true;

            while (expectNewParameter)
            {
                if(!_currentToken.TokenType.IsDeclarableType())
                    RaiseFactoryError(null!);

                var type = ParseDeclarableType();
                var identifier = ParseIdentifier(_currentToken);

                expectNewParameter = _currentToken.TokenType == TokenType.CommaOperator;
                parameters.Add(new Parameter(type, identifier));
            }

            return parameters;
        });

        #endregion

        #region Statements builders

        /// <summary>
        /// EBNF: <c>blockStatement = '{', { basicStatement }, '}';</c>
        /// </summary>
        private BlockStatement? ParseBlockStatement() => BuilderWrapper<BlockStatement?>(() =>
        {
            ValidateCurrentToken(TokenType.OpenBlockOperator, null!);
            ConsumeToken();

            var statementsList = new List<IBasicStatement>();

            while (ParseBasicStatement() is { } statement)
                statementsList.Add(statement);

            ValidateCurrentToken(TokenType.CloseBlockOperator, null!);
            ConsumeToken();

            return new BlockStatement(statementsList);
        });

        private IBasicStatement? ParseBasicStatement() => BuilderWrapper<IBasicStatement?>(() =>
        {
            IBasicStatement? statement = _currentToken.TokenType switch
            {
                TokenType.IfKeyword => ParseIfStatement(),
                TokenType.WhileKeyword => ParseWhileStatement(),
                TokenType.ContinueKeyword => new ContinueStatement(),
                TokenType.BreakKeyword => new BreakStatement(),
                _ => null
            };

            if (statement != null)
                return statement;

            if (_currentToken.TokenType.IsDeclarableType())
                return ParseVariableDeclaration();

            return ParseExpressionStatement();
        });

        private IfStatement? ParseIfStatement() => BuilderWrapper<IfStatement?>(() =>
        {
            throw new NotImplementedException();
        });

        private WhileStatement? ParseWhileStatement() => BuilderWrapper<WhileStatement?>(() =>
        {
            throw new NotImplementedException();
        });

        private VariableDeclaration? ParseVariableDeclaration() => BuilderWrapper<VariableDeclaration?>(() =>
        {
            throw new NotImplementedException();
        });

        private ExpressionStatement? ParseExpressionStatement() => BuilderWrapper<ExpressionStatement?>(() =>
        {
            throw new NotImplementedException();
        });

        #endregion
    }
}
