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

        /// <summary>
        /// Loads the next available token.
        /// </summary>
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
        /// Helper method that extracts the value from the current token, after successful token type validation.
        /// </summary>
        private T ExtractValueFromCurrentToken<T>(TokenType expectedTokenType)
        {
            ValidateCurrentToken(expectedTokenType, null!);

            var value = _currentToken as Token<T>;

            if (value == null)
                RaiseFactoryError(null!);
            
            ConsumeToken();
            return value!.Value;
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier;</c>  
        /// </summary>
        private ImportDirective? ParseImportDirective() => BuilderWrapper<ImportDirective?>(() =>
        {
            ValidateCurrentToken(TokenType.ImportKeyword, null!);
            ConsumeToken();

            var moduleName = ExtractValueFromCurrentToken<string>(TokenType.Identifier);
            var functionName = ExtractValueFromCurrentToken<string>(TokenType.Identifier);

            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            ConsumeToken();

            return new ImportDirective(moduleName, functionName);
        });

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        private FunctionDefinition? ParseFunctionDefinition() => BuilderWrapper<FunctionDefinition?>(() =>
        {
            var returnType = ParseReturnType();
            var identifier = ExtractValueFromCurrentToken<string>(TokenType.Identifier);

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

        /// <summary>
        /// EBNF: <c>returnType = 'void' | typeName;</c>
        /// </summary>
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

        /// <summary>
        /// EBNF: <c>dictType = 'dict', '&lt;', primitiveType, ',', primitiveType, '&gt;';</c>
        /// </summary>
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

        /// <summary>
        /// EBNF: <c>typeName = primitiveType | dictType;</c>
        /// </summary>
        private IDeclarableType? ParseDeclarableType() => BuilderWrapper<IDeclarableType?>(() =>
        {
            if (_currentToken.TokenType == TokenType.DictKeyword)
                return ParseDictionaryType();

            if (!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            ConsumeToken();
            return new PrimitiveType((PrimitiveTypes)_currentToken.TokenType);
        });
        
        /// <summary>
        /// EBNF: <c>parametersList = [ typeName, identifier, { ',', typeName, identifier } ];</c>
        /// </summary>
        private IEnumerable<Parameter>? ParseParametersList() => BuilderWrapper<IEnumerable<Parameter>?>(() =>
        {
            var parameters = new List<Parameter>();
            var expectNewParameter = true;

            while (expectNewParameter)
            {
                if (!_currentToken.TokenType.IsDeclarableType())
                    RaiseFactoryError(null!);

                var type = ParseDeclarableType();
                var identifier = ExtractValueFromCurrentToken<string>(TokenType.Identifier);

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

        /// <summary>
        /// EBNF: <c>basicStatement = ifStatement | whileStatement | 'continue', ';' | 'break', ';' | tryStatement | 'throw', stringLiteral, ';' | expressionStatement;</c>
        /// </summary>
        private IBasicStatement? ParseBasicStatement() => BuilderWrapper<IBasicStatement?>(() =>
        {
            var statement = _currentToken.TokenType switch
            {
                TokenType.IfKeyword => ParseIfStatement(),
                TokenType.WhileKeyword => ParseWhileStatement(),
                TokenType.ContinueKeyword => ParseSingleLineStatement(() => new ContinueStatement()),
                TokenType.BreakKeyword => ParseSingleLineStatement(() => new BreakStatement()),
                TokenType.ThrowKeyword => ParseThrowStatement(),
                TokenType.TryKeyword => ParseTryStatement(),
                _ => null
            };

            if (statement != null)
                return statement;

            return _currentToken.TokenType.IsDeclarableType() 
                ? ParseSingleLineStatement(ParseVariableDeclaration)
                : ParseSingleLineStatement(ParseExpressionStatement);
        });

        /// <summary>
        /// Wrapper method which ensures that single line statements end with a semicolon.
        /// </summary>
        /// <param name="statementFactory">Factory method for a statement.</param>
        /// <returns>The parsed statement.</returns>
        private IBasicStatement? ParseSingleLineStatement(Func<IBasicStatement?> statementFactory)
        {
            var statement = statementFactory.Invoke();

            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            ConsumeToken();

            return statement;
        }

        /// <summary>
        /// EBNF: <c>ifStatement = 'if', '(', expression, ')', blockStatement, [ 'else', ( ifStatement | blockStatement ) ];</c>
        /// </summary>
        private IfStatement? ParseIfStatement() => BuilderWrapper<IfStatement?>(() =>
        {
            ValidateCurrentToken(TokenType.IfKeyword, null!);
            ConsumeToken();

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            ConsumeToken();

            var expression = ParseExpression();

            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
            ConsumeToken();

            var body = ParseBlockStatement();
            BlockStatement? elseBlock = null;
            IfStatement? elseIfStatement = null;

            if (_currentToken.TokenType == TokenType.ElseKeyword)
            {
                ConsumeToken();

                if (_currentToken.TokenType == TokenType.IfKeyword)
                    elseIfStatement = ParseIfStatement();

                elseBlock = ParseBlockStatement();
            }

            return new IfStatement(expression, body, elseIfStatement, elseBlock);
        });

        /// <summary>
        /// EBNF: <c>whileStatement = 'while', '(', expression, ')', blockStatement;</c>
        /// </summary>
        private WhileStatement? ParseWhileStatement() => BuilderWrapper<WhileStatement?>(() =>
        {
            ValidateCurrentToken(TokenType.WhileKeyword, null!);
            ConsumeToken();
            
            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            ConsumeToken();

            var expression = ParseExpression();

            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
            ConsumeToken();

            var body = ParseBlockStatement();
            return new WhileStatement(expression, body);
        });

        /// <summary>
        /// EBNF: <c>variableDeclaration = typeName, identifier, [ '=', expression ], ';';</c>
        /// </summary>
        private VariableDeclaration? ParseVariableDeclaration() => BuilderWrapper<VariableDeclaration?>(() =>
        {
            var type = ParseDeclarableType();
            var identifier = ExtractValueFromCurrentToken<string>(TokenType.Identifier);

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
                return new VariableDeclaration(type, identifier);

            ConsumeToken();
            var initializationExpression = ParseExpression();

            return new VariableDeclaration(type, identifier, initializationExpression);
        });

        /// <summary>
        /// EBNF: <c>expressionStatement = expression, [ '=', expression ], ';'</c>
        /// </summary>
        private ExpressionStatement? ParseExpressionStatement() => BuilderWrapper<ExpressionStatement?>(() =>
        {
            var expression = ParseExpression();

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
                return new ExpressionStatement(expression);

            ConsumeToken();
            var initializationExpression = ParseExpression();

            return new ExpressionStatement(expression, initializationExpression);
        });

        /// <summary>
        /// EBNF: <c>throwStatement = 'throw', stringLiteral, ';'</c>
        /// </summary>
        private ThrowStatement? ParseThrowStatement() => BuilderWrapper<ThrowStatement?>(() =>
        {
            ValidateCurrentToken(TokenType.ThrowKeyword, null!);
            ConsumeToken();

            ValidateCurrentToken(TokenType.StringLiteral, null!);
            var message = ExtractValueFromCurrentToken<string>(TokenType.StringLiteral);

            return new ThrowStatement(message);
        });

        /// <summary>
        /// EBNF: <c>tryStatement = 'try', blockStatement, catchClause, { catchClause };</c>
        /// </summary>
        private TryStatement? ParseTryStatement() => BuilderWrapper<TryStatement?>(() =>
        {
            ValidateCurrentToken(TokenType.TryKeyword, null!);
            ConsumeToken();

            var body = ParseBlockStatement();
            var catchClauses = new List<CatchClause>();

            while (ParseCatchClause() is { } catchClause)
                catchClauses.Add(catchClause);

            if (catchClauses.Count == 0)
                RaiseFactoryError(null!);

            return new TryStatement(body, catchClauses);
        });

        /// <summary>
        /// EBNF: <c>catchClause = 'catch', '(', identifier, [ expression ] ')', blockStatement;</c>
        /// </summary>
        private CatchClause? ParseCatchClause() => BuilderWrapper<CatchClause?>(() =>
        {
            ValidateCurrentToken(TokenType.CatchKeyword, null!);
            ConsumeToken();

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            ConsumeToken();

            var identifier = ExtractValueFromCurrentToken<string>(TokenType.Identifier);
            ConsumeToken();

            IExpression? filterExpression = null;

            if (_currentToken.TokenType == TokenType.RightParenthesisOperator)
                ConsumeToken();
            else
            {
                filterExpression = ParseExpression();
                ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
                ConsumeToken();
            }

            var body = ParseBlockStatement();
            return new CatchClause(identifier, filterExpression, body);
        });

        #endregion

        #region Expressions builders

        private IExpression? ParseExpression() => BuilderWrapper<IExpression?>(() =>
        {
            throw new NotImplementedException();
        });

        #endregion
    }
}
