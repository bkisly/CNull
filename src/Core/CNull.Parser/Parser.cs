using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors;
using CNull.Lexer;
using CNull.Lexer.Constants;
using CNull.Lexer.Errors;
using CNull.Lexer.Extensions;
using CNull.Parser.Errors;
using CNull.Parser.Exceptions;
using CNull.Parser.Extensions;
using CNull.Parser.Productions;
using Microsoft.Extensions.Logging;

namespace CNull.Parser
{
    /// <summary>
    /// Enumeration of primitive types specifiers.
    /// </summary>
    public enum PrimitiveTypes
    {
        Integer = TokenType.IntKeyword,
        Float = TokenType.FloatKeyword,
        String = TokenType.StringKeyword,
        Char = TokenType.CharKeyword,
        Boolean = TokenType.BoolKeyword,
    }

    /// <summary>
    /// Enumeration of type specifiers.
    /// </summary>
    public enum Types
    {
        Integer = TokenType.IntKeyword,
        Float = TokenType.FloatKeyword,
        String = TokenType.StringKeyword,
        Char = TokenType.CharKeyword,
        Boolean = TokenType.BoolKeyword,
        Dictionary = TokenType.DictKeyword,
        Void = TokenType.VoidKeyword,
    }

    public class Parser(ILexer lexer, IErrorHandler errorHandler, ILogger<IParser> logger) : IParser
    {
        private Token _currentToken = null!;

        private readonly Dictionary<TokenType, BinaryExpressionFactory> _relationalExpressionsFactoryMap = new()
        {
            { TokenType.GreaterThanOperator, (left, right, position) => new GreaterThanExpression(left, right, position) },
            { TokenType.GreaterThanOrEqualOperator, (left, right, position) => new GreaterThanOrEqualExpression(left, right, position) },
            { TokenType.EqualOperator, (left, right, position) => new EqualExpression(left, right, position) },
            { TokenType.NotEqualOperator, (left, right, position) => new NotEqualExpression(left, right, position) },
            { TokenType.LessThanOperator, (left, right, position) => new LessThanExpression(left, right, position) },
            { TokenType.LessThanOrEqualOperator, (left, right, position) => new LessThanOrEqualExpression(left, right, position) }
        };

        private readonly Dictionary<TokenType, BinaryExpressionFactory> _additiveExpressionsFactoryMap = new()
        {
            { TokenType.PlusOperator, (left, right, position) => new AdditionExpression(left, right, position) },
            { TokenType.MinusOperator, (left, right, position) => new SubtractionExpression(left, right, position) }
        };

        private readonly Dictionary<TokenType, BinaryExpressionFactory> _multiplicativeExpressionsFactoryMap = new()
        {
            { TokenType.AsteriskOperator, (left, right, position) => new MultiplicationExpression(left, right, position) },
            { TokenType.SlashOperator, (left, right, position) => new DivisionExpression(left, right, position) },
            { TokenType.PercentOperator, (left, right, position) => new ModuloExpression(left, right, position) },
        };

        private readonly Dictionary<TokenType, UnaryExpressionFactory> _unaryExpressionsFactoryMap = new()
        {
            { TokenType.MinusOperator, (expression, position) => new NegationExpression(expression, position) },
            { TokenType.NegationOperator, (expression, position) => new BooleanNegationExpression(expression, position) }
        };

        public Program? Parse()
        {
            try
            {
                logger.LogInformation("Beginning to parse the program.");

                if (lexer.LastToken == null)
                    ConsumeToken();

                var importDirectives = new List<ImportDirective>();
                var functionDefinitions = new List<FunctionDefinition>();

                while (ParseImportDirective() is { } directive)
                    importDirectives.Add(directive);

                while (ParseFunctionDefinition() is { } functionDefinition)
                    functionDefinitions.Add(functionDefinition);

                logger.LogInformation("Successfully parsed the program.");
                return new Program(importDirectives, functionDefinitions);
            }
            catch (UnexpectedTokenException)
            {
                logger.LogWarning("Failed to parse the program.");
                return null;
            }
        }

        #region Common methods

        /// <summary>
        /// Loads the next available token.
        /// </summary>
        private void ConsumeToken() => _currentToken = lexer.GetNextToken();

        /// <summary>
        /// Checks if the current token matches the expected type.
        /// </summary>
        /// <param name="expectedType">Expected token type.</param>
        private void ValidateCurrentToken(TokenType expectedType)
        {
            if (_currentToken.TokenType != expectedType)
                errorHandler.RaiseCompilationError(
                    new MissingKeywordOrOperatorError(expectedType.ToLiteralString(), _currentToken.Position));
            else ConsumeToken();
        }

        /// <summary>
        /// Checks if the current token matches the expected type and extracts the value from the token, accordingly to the given type.
        /// </summary>
        /// <returns>The value extracted from the token.</returns>
        /// <exception cref="UnexpectedTokenException"/>
        private T ValidateCurrentToken<T>(TokenType expectedType, ICompilationError errorToThrow)
        {
            if (_currentToken.TokenType != expectedType)
                throw ParserError(errorToThrow);

            var token = _currentToken as Token<T> ?? throw ParserError(new InvalidLiteralError<T>(_currentToken.Position));
            ConsumeToken();
            return token.Value;
        }

        /// <summary>
        /// Builds an error during creation of a syntactic production.
        /// </summary>
        private UnexpectedTokenException ParserError(ICompilationError errorToThrow)
        {
            errorHandler.RaiseCompilationError(errorToThrow);
            return new UnexpectedTokenException();
        }

        /// <summary>
        /// Wraps parser factory method to log information before and after its execution.
        /// </summary>
        /// <typeparam name="T">Return type of the production factory.</typeparam>
        /// <param name="productionFactory">Factory method of the production.</param>
        /// <returns>The result of the wrapped factory.</returns>
        private T LoggingWrapper<T>(Func<T> productionFactory) where T : ISyntacticProduction?
            => LoggingWrapper(typeof(T).Name, productionFactory);


        /// <summary>
        /// <inheritdoc cref="LoggingWrapper{T}(System.Func{T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The result of the wrapped factory.</returns>
        private T LoggingWrapper<T>(string builtTypeName, Func<T> productionFactory) where T : ISyntacticProduction?
        {
            var position = _currentToken.Position;
            var logHeader = $"[Parser factory at: {position}]";

            logger.LogInformation($"{logHeader} Beginning to build a production of type: {builtTypeName} at {position}.");
            var production = productionFactory.Invoke();

            logger.LogInformation(production != null
                ? $"{logHeader} Successfully built a production of type: {builtTypeName}."
                : $"{logHeader} Production factory of type: {builtTypeName} returned null.");

            return production;
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier, ';';</c>  
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ImportDirective? ParseImportDirective() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.ImportKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            var moduleName = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));
            ValidateCurrentToken(TokenType.DotOperator);
            var functionName = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));

            ValidateCurrentToken(TokenType.SemicolonOperator);
            return new ImportDirective(moduleName, functionName, position);
        });

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private FunctionDefinition? ParseFunctionDefinition() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType == TokenType.End)
                return null;

            var position = _currentToken.Position;
            var returnType = ParseReturnType() ?? throw ParserError(new InvalidReturnTypeError(_currentToken.Position));
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));

            ValidateCurrentToken(TokenType.LeftParenthesisOperator);
            var parameters = ParseParametersList();
            ValidateCurrentToken(TokenType.RightParenthesisOperator);

            var functionBody = ParseBlockStatement();
            return functionBody == null
                ? throw ParserError(new ExpectedBlockStatementError(_currentToken.Position))
                : new FunctionDefinition(returnType, identifier, parameters, functionBody, position);
        });
        
        #endregion

        #region Types and parameters builders

        /// <summary>
        /// EBNF: <c>returnType = 'void' | typeName;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnType? ParseReturnType()
            => LoggingWrapper(() => ParseVoidType()
                                    ?? ParsePrimitiveType() as ReturnType
                                    ?? ParseDictionaryType());

        private ReturnType? ParseVoidType() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.VoidKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            return new ReturnType(position);
        });

        private PrimitiveType? ParsePrimitiveType() => LoggingWrapper(() =>
        {
            if (!_currentToken.TokenType.IsPrimitiveType())
                return null;

            var position = _currentToken.Position;
            var type = (PrimitiveTypes)_currentToken.TokenType;
            ConsumeToken();
            return new PrimitiveType(type, position);
        });

        /// <summary>
        /// EBNF: <c>dictType = 'dict', '&lt;', primitiveType, ',', primitiveType, '&gt;';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private DictionaryType? ParseDictionaryType() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.DictKeyword)
                return null;

            var dictPosition = _currentToken.Position;
            ConsumeToken();
            ValidateCurrentToken(TokenType.LessThanOperator);

            var keyType = ParsePrimitiveType() ?? throw ParserError(new TypeNotPrimitiveError(_currentToken.Position));
            ValidateCurrentToken(TokenType.CommaOperator);
            var valueType = ParsePrimitiveType() ??
                            throw ParserError(new TypeNotPrimitiveError(_currentToken.Position));

            ValidateCurrentToken(TokenType.GreaterThanOperator);
            return new DictionaryType(keyType, valueType, dictPosition);
        });

        /// <summary>
        /// EBNF: <c>typeName = primitiveType | dictType;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IDeclarableType? ParseDeclarableType() =>
            LoggingWrapper(() => ParseDictionaryType() as IDeclarableType ?? ParsePrimitiveType()); 

        /// <summary>
        /// EBNF: <c>parametersList = [ parameter, { ',', parameter } ];</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private List<Parameter> ParseParametersList()
        {
            var parameters = new List<Parameter>();

            if (ParseParameter() is not { } parameter)
                return parameters;

            parameters.Add(parameter);

            while (_currentToken.TokenType == TokenType.CommaOperator)
            {
                ConsumeToken();
                var nextParameter = ParseParameter() ?? throw ParserError(new ExpectedParameterError(_currentToken.Position));
                parameters.Add(nextParameter);
            }

            return parameters;
        }

        /// <summary>
        /// EBNF: <c>parameter = typeName, identifier;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private Parameter? ParseParameter() => LoggingWrapper(() =>
        {
            if (!_currentToken.TokenType.IsDeclarableType())
                return null;

            var position = _currentToken.Position;
            var type = ParseDeclarableType() ?? throw ParserError(new TypeNotValidError(position));
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));
            return new Parameter(type, identifier, position);
        });

        #endregion

        #region Statements builders

        /// <summary>
        /// EBNF: <c>blockStatement = '{', { basicStatement }, '}';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BlockStatement? ParseBlockStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.OpenBlockOperator)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            var statementsList = new List<IBasicStatement>();

            while (ParseBasicStatement() is { } statement)
                statementsList.Add(statement);

            ValidateCurrentToken(TokenType.CloseBlockOperator);
            return new BlockStatement(statementsList, position);
        });

        /// <summary>
        /// EBNF: <c>basicStatement = ifStatement | whileStatement | continueStatement | breakStatement | tryStatement |
        /// throwStatement | expressionStatement | returnStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IBasicStatement? ParseBasicStatement()
            => LoggingWrapper(() => ParseIfStatement()
                                    ?? ParseWhileStatement()
                                    ?? ParseContinueStatement()
                                    ?? ParseBreakStatement()
                                    ?? ParseThrowStatement()
                                    ?? ParseTryStatement()
                                    ?? ParseReturnStatement()
                                    ?? ParseVariableDeclaration() as IBasicStatement
                                    ?? ParseExpressionStatement());

        /// <summary>
        /// EBNF: <c>ifStatement = 'if', '(', expression, ')', blockStatement, [ 'else', ( ifStatement | blockStatement ) ];</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IfStatement? ParseIfStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.IfKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            ValidateCurrentToken(TokenType.LeftParenthesisOperator);
            var expression = ParseExpression() ?? throw ParserError(new InvalidExpressionError(_currentToken.Position));
            ValidateCurrentToken(TokenType.RightParenthesisOperator);

            var body = ParseBlockStatement()
                       ?? throw ParserError(new ExpectedBlockStatementError(_currentToken.Position));

            BlockStatement? elseBlock = null;
            IfStatement? elseIfStatement = null;

            if (_currentToken.TokenType != TokenType.ElseKeyword)
                return new IfStatement(expression, body, elseIfStatement, elseBlock, position);

            ConsumeToken();
            if (_currentToken.TokenType == TokenType.IfKeyword)
                elseIfStatement = ParseIfStatement();

            elseBlock = ParseBlockStatement();
            return new IfStatement(expression, body, elseIfStatement, elseBlock, position);
        });

        /// <summary>
        /// EBNF: <c>whileStatement = 'while', '(', expression, ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private WhileStatement? ParseWhileStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.WhileKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();

            ValidateCurrentToken(TokenType.LeftParenthesisOperator);
            var expression = ParseExpression() ?? throw ParserError(new InvalidExpressionError(_currentToken.Position));
            ValidateCurrentToken(TokenType.RightParenthesisOperator);

            var body = ParseBlockStatement() 
                       ?? throw ParserError(new ExpectedBlockStatementError(_currentToken.Position));

            return new WhileStatement(expression, body, position);
        });

        /// <summary>
        /// EBNF: <c>variableDeclaration = typeName, identSifier, [ '=', expression ], ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private VariableDeclaration? ParseVariableDeclaration() => LoggingWrapper(() =>
        {
            if (!_currentToken.TokenType.IsDeclarableType())
                return null;

            var position = _currentToken.Position;
            var type = ParseDeclarableType() ?? throw ParserError(new TypeNotValidError(position));
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));
            IExpression? initializationExpression = null;

            if (_currentToken.TokenType == TokenType.AssignmentOperator)
            {
                ConsumeToken();
                initializationExpression = ParseExpression();
            }

            ValidateCurrentToken(TokenType.SemicolonOperator);
            return new VariableDeclaration(type, identifier, position, initializationExpression);
        });

        /// <summary>
        /// EBNF: <c>expressionStatement = expression, [ '=', expression ], ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ExpressionStatement? ParseExpressionStatement() => LoggingWrapper(() =>
        {
            var position = _currentToken.Position;
            var expression = ParsePrimaryExpression();

            if (expression == null)
                return null;

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
            {
                ValidateCurrentToken(TokenType.SemicolonOperator);
                return new ExpressionStatement(expression, position);
            }

            ConsumeToken();
            var initializationExpression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator);

            return new ExpressionStatement(expression, position, initializationExpression);
        });

        /// <summary>
        /// EBNF: <c>continueStatement = 'continue', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ContinueStatement? ParseContinueStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.ContinueKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            ValidateCurrentToken(TokenType.SemicolonOperator);
            return new ContinueStatement(position);
        });

        /// <summary>
        /// EBNF: <c>breakStatement = 'break', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BreakStatement? ParseBreakStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.BreakKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            ValidateCurrentToken(TokenType.SemicolonOperator);
            return new BreakStatement(position);
        });

        /// <summary>
        /// EBNF: <c>throwStatement = 'throw', stringLiteral, ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ThrowStatement? ParseThrowStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.ThrowKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();
            var message = ValidateCurrentToken<string>(TokenType.StringLiteral, new ExpectedStringLiteralError(_currentToken.Position));
            ValidateCurrentToken(TokenType.SemicolonOperator);

            return new ThrowStatement(message, position);
        });

        /// <summary>
        /// EBNF: <c>tryStatement = 'try', blockStatement, catchClause, { catchClause };</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private TryStatement? ParseTryStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.TryKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();

            var body = ParseBlockStatement() 
                       ?? throw ParserError(new ExpectedBlockStatementError(_currentToken.Position));

            var catchClauses = new List<CatchClause>();
            while (ParseCatchClause() is { } catchClause)
                catchClauses.Add(catchClause);

            if (catchClauses.Count == 0)
                throw ParserError(new MissingCatchClauseError(_currentToken.Position));

            return new TryStatement(body, catchClauses, position);
        });

        /// <summary>
        /// EBNF: <c>catchClause = 'catch', '(', identifier, [ expression ] ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private CatchClause? ParseCatchClause() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.CatchKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();

            ValidateCurrentToken(TokenType.LeftParenthesisOperator);
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, new ExpectedIdentifierError(_currentToken.Position));

            IExpression? filterExpression = null;

            if (_currentToken.TokenType == TokenType.RightParenthesisOperator)
                ConsumeToken();
            else
            {
                filterExpression = ParseExpression();
                ValidateCurrentToken(TokenType.RightParenthesisOperator);
            }

            var body = ParseBlockStatement();
            return body == null
                ? throw ParserError(new ExpectedBlockStatementError(_currentToken.Position))
                : new CatchClause(identifier, filterExpression, body, position);
        });

        /// <summary>
        /// EBNF: <c>returnStatement = 'return', expression, ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnStatement? ParseReturnStatement() => LoggingWrapper(() =>
        {
            if (_currentToken.TokenType != TokenType.ReturnKeyword)
                return null;

            var position = _currentToken.Position;
            ConsumeToken();

            var expression = ParseExpression() ?? throw ParserError(new InvalidExpressionError(_currentToken.Position));
            ValidateCurrentToken(TokenType.SemicolonOperator);
            return new ReturnStatement(expression, position);
        });

        #endregion

        #region Expressions builders

        /// <summary>
        /// Helper method which parses a binary expression, that does not support connectivity.
        /// </summary>
        /// <param name="tokenTypeToFactories">Map of token types to corresponding factory methods of binary expressions.</param>
        /// <param name="innerFactory">Factory method which parses an expression in the inner tree.</param>
        /// <param name="connective">Determines whether the expression supports connectivity.</param>
        /// <returns>The parsed binary expression.</returns>
        private IExpression? ParseBinaryExpression(Dictionary<TokenType, BinaryExpressionFactory> tokenTypeToFactories,
            Func<IExpression?> innerFactory, bool connective)
        {
            var leftFactor = innerFactory.Invoke();

            if (leftFactor == null)
                return null;

            while (tokenTypeToFactories.TryGetValue(_currentToken.TokenType, out var factory))
            {
                var position = _currentToken.Position;
                ConsumeToken();
                var rightFactor = innerFactory.Invoke() ?? throw ParserError(new InvalidExpressionError(_currentToken.Position));
                leftFactor = factory.Invoke(leftFactor, rightFactor, position);

                if (!connective)
                    break;
            }

            return leftFactor;
        }

        /// <summary>
        /// Helper method which parses a binary expression, that does not support connectivity.
        /// </summary>
        /// <param name="operatorType">Type of the operator of the expression.</param>
        /// <param name="expressionFactory">Factory method of the parsed expression.</param>
        /// <param name="innerFactory">Factory method which parses an expression in the inner tree.</param>
        /// <param name="connective">Determines whether the expression supports connectivity.</param>
        /// <returns>The parsed binary expression.</returns>
        private IExpression? ParseBinaryExpression(TokenType operatorType, BinaryExpressionFactory expressionFactory,
            Func<IExpression?> innerFactory, bool connective)
        {
            return ParseBinaryExpression(
                new Dictionary<TokenType, BinaryExpressionFactory> { [operatorType] = expressionFactory }, 
                innerFactory,
                connective);
        }

        /// <summary>
        /// EBNF: <c>expression = conditionalAndExpression, { '||', conditionalAndExpression };</c>
        /// </summary>
        private IExpression? ParseExpression()
        {
            return ParseBinaryExpression(
                TokenType.OrOperator,
                (left, right, position) => new OrExpression(left, right, position),
                ParseAndExpression,
                true);
        }

        /// <summary>
        /// EBNF: <c>conditionalAndExpression = relationalExpression, { '&amp;&amp;', relationalExpression };</c>
        /// </summary>
        private IExpression? ParseAndExpression()
        {
            return ParseBinaryExpression(
                TokenType.AndOperator,
                (left, right, position) => new AndExpression(left, right, position),
                ParseRelationalExpression,
                true);
        }

        /// <summary>
        /// EBNF: <c>relationalExpression = additiveExpression, [ relationalOperator, additiveExpression ];</c>
        /// </summary>
        private IExpression? ParseRelationalExpression() => LoggingWrapper("RelationalExpression", ()
                => ParseBinaryExpression(_relationalExpressionsFactoryMap, ParseAdditiveExpression, false));

        /// <summary>
        /// EBNF: <c>additiveExpression = multiplicativeExpression, [ additiveOperator, multiplicativeExpression ];</c>
        /// </summary>
        private IExpression? ParseAdditiveExpression() => LoggingWrapper("AdditiveExpression", ()
                => ParseBinaryExpression(_additiveExpressionsFactoryMap, ParseMultiplicativeExpression, true));

        /// <summary>
        /// EBNF: <c>multiplicativeExpression = unaryExpression, [ multiplicativeOperator, unaryExpression ];</c>
        /// </summary>
        private IExpression? ParseMultiplicativeExpression() => LoggingWrapper("MultiplicativeExpression", ()
                => ParseBinaryExpression(_multiplicativeExpressionsFactoryMap, ParseUnaryExpression, true));

        /// <summary>
        /// EBNF: <c>unaryExpression = [ ( '!' | '-' ) ], secondaryExpression;</c>
        /// </summary>
        private IExpression? ParseUnaryExpression() => LoggingWrapper("UnaryExpression", () =>
        {
            var hasOperator = _unaryExpressionsFactoryMap.TryGetValue(_currentToken.TokenType, out var factory);
            var position = _currentToken.Position;

            if (hasOperator)
                ConsumeToken();

            var innerExpression = ParseSecondaryExpression();
            if (innerExpression != null)
                return factory?.Invoke(innerExpression, position) ?? innerExpression;

            if (hasOperator)
                throw ParserError(new InvalidExpressionError(_currentToken.Position));

            return null;
        });

        /// <summary>
        /// EBNF: <c>secondaryExpression = primaryExpression, [ '?' ];</c>
        /// </summary>
        private IExpression? ParseSecondaryExpression() => LoggingWrapper("SecondaryExpression", () =>
        {
            if (ParsePrimaryExpression() is not { } innerExpression)
                return null;

            if (_currentToken.TokenType != TokenType.IsNullOperator)
                return innerExpression;

            var position = _currentToken.Position;
            ConsumeToken();
            return new NullCheckExpression(innerExpression, position);
        });

        /// <summary>
        /// EBNF: <c>primaryExpression = ( literal | identifierOrCall | parenthesisedExpression ), { memberAccess };</c>
        /// </summary>
        /// <returns></returns>
        private IExpression? ParsePrimaryExpression()
        {
            var expression = ParseLiteral() ?? ParseIdentifierOrCall() ?? ParseParenthesisedExpression();
            if (expression == null)
                return null;

            if (_currentToken.TokenType != TokenType.DotOperator)
                return expression;

            while (_currentToken.TokenType == TokenType.DotOperator)
            {
                ConsumeToken();
                expression = ParseIdentifierOrCall(expression);
            }

            return expression;
        }

        /// <summary>
        /// EBNF: <c>identifierOrCall = identifier, [ '(', { expression }, ')' ];</c>
        /// </summary>
        private IExpression? ParseIdentifierOrCall(IExpression? parentExpression = null) 
            => LoggingWrapper<IExpression?>("IdentifierOrCall", () =>
        {
            if (_currentToken.TokenType != TokenType.Identifier)
                return null;

            var position = _currentToken.Position;
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, new InvalidIdentifierError(_currentToken.Position));

            if (_currentToken.TokenType != TokenType.LeftParenthesisOperator)
                return new IdentifierExpression(identifier, position, parentExpression);

            ConsumeToken();
            var argumentsList = ParseArgumentsList();

            ValidateCurrentToken(TokenType.RightParenthesisOperator);
            return new CallExpression(identifier, argumentsList, position, parentExpression);
        });

        private List<IExpression> ParseArgumentsList()
        {
            var arguments = new List<IExpression>();

            if (ParseExpression() is not { } argument)
                return arguments;

            arguments.Add(argument);

            while (_currentToken.TokenType == TokenType.CommaOperator)
            {
                ConsumeToken();
                var nextArgument = ParseExpression() ?? throw ParserError(new ExpectedArgumentError(_currentToken.Position));
                arguments.Add(nextArgument);
            }

            return arguments;
        }

        private IExpression? ParseLiteral() => LoggingWrapper<IExpression?>("LiteralExpression", () =>
        {
            return _currentToken.TokenType switch
            {
                TokenType.IntegerLiteral => BuildLiteral<int>(),
                TokenType.FloatLiteral => BuildLiteral<float>(),
                TokenType.StringLiteral => BuildLiteral<string>(),
                TokenType.CharLiteral => BuildLiteral<char>(),
                TokenType.TrueKeyword => BuildLiteral<bool>(),
                TokenType.FalseKeyword => BuildLiteral<bool>(),
                TokenType.NullKeyword => BuildLiteral<object?>(),
                _ => null
            };
        });

        /// <summary>
        /// Builds a literal expression, basing on the expected type.
        /// </summary>
        /// <typeparam name="T">Type of the literal to build.</typeparam>
        /// <returns>The built LiteralExpression.</returns>
        private LiteralExpression<T> BuildLiteral<T>()
        {
            var position = _currentToken.Position;
            var value = ((Token<T>)_currentToken).Value;
            ConsumeToken();
            return new LiteralExpression<T>(value, position);
        }

        /// <summary>
        /// EBNF: <c>parenthesisedExpression = '(', expression, ')';</c>
        /// </summary>
        /// <returns></returns>
        private IExpression? ParseParenthesisedExpression() => LoggingWrapper("ParenthesisedExpression", () =>
        {
            if (_currentToken.TokenType != TokenType.LeftParenthesisOperator)
                return null;

            ConsumeToken();
            var innerExpression = ParseExpression() ?? throw ParserError(new InvalidExpressionError(_currentToken.Position));
            ValidateCurrentToken(TokenType.RightParenthesisOperator);
            return innerExpression;
        });

        #endregion
    }
}
