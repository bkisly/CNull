using CNull.Common;
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
    public class Parser(ILexer lexer, IErrorHandler errorHandler) : IParser
    {
        private Token _currentToken = null!;

        public Program? Parse()
        {
            try
            {
                if (lexer.LastToken == null)
                    ConsumeToken();

                var importDirectives = new List<ImportDirective>();
                var functionDefinitions = new List<FunctionDefinition>();

                while (ParseImportDirective() is { } directive)
                    importDirectives.Add(directive);

                while (ParseFunctionDefinition() is { } functionDefinition)
                    functionDefinitions.Add(functionDefinition);

                return new Program(importDirectives, functionDefinitions);
            }
            catch (UnexpectedTokenException)
            {
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
        /// <param name="errorToThrow">Error to be passed to the error handler.</param>
        /// <exception cref="UnexpectedTokenException"/>
        private void ValidateCurrentToken(TokenType expectedType, ICompilationError errorToThrow)
        {
            if (_currentToken.TokenType != expectedType)
                RaiseFactoryError(errorToThrow);

            ConsumeToken();
        }

        /// <summary>
        /// Checks if the current token matches the expected type and extracts the value from the token, accordingly to the given type.
        /// </summary>
        /// <returns>The value extracted from the token.</returns>
        /// <exception cref="UnexpectedTokenException"/>
        private T ValidateCurrentToken<T>(TokenType expectedType, ICompilationError errorToThrow)
        {
            if (_currentToken.TokenType != expectedType)
                RaiseFactoryError(errorToThrow);

            var value = (_currentToken as Token<T>)!.Value;
            ConsumeToken();
            return value;
        }

        /// <summary>
        /// Raises an error during creation of a syntactic production.
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private void RaiseFactoryError(ICompilationError errorToThrow)
        {
            errorHandler.RaiseCompilationError(errorToThrow);
            throw new UnexpectedTokenException();
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier;</c>  
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ImportDirective? ParseImportDirective()
        {
            var position = _currentToken.Position;
            if (_currentToken.TokenType != TokenType.ImportKeyword)
                return null;

            ConsumeToken();
            var moduleName = ValidateCurrentToken<string>(TokenType.Identifier, null!);
            ValidateCurrentToken(TokenType.DotOperator, null!);
            var functionName = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new ImportDirective(moduleName, functionName, position);
        }

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private FunctionDefinition? ParseFunctionDefinition()
        {
            var position = _currentToken.Position;
            var returnType = ParseReturnType();

            if (returnType == null)
                return null;

            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var parameters = ParseParametersList();
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);

            var functionBody = ParseBlockStatement();
            return new FunctionDefinition(returnType, identifier, parameters, functionBody, position);
        }

        #endregion

        #region Types and parameters builders

        /// <summary>
        /// EBNF: <c>returnType = 'void' | typeName;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnType? ParseReturnType()
        {
            if (!_currentToken.TokenType.IsReturnType())
                return null;

            return _currentToken.TokenType switch
            {
                TokenType.VoidKeyword => ParseVoidType(),
                TokenType.DictKeyword => ParseDictionaryType(),
                _ => ParsePrimitiveType()
            };
        }

        private ReturnType ParseVoidType()
        {
            var position = _currentToken.Position;
            ConsumeToken();
            return new ReturnType(position);
        }

        private PrimitiveType ParsePrimitiveType()
        {
            var position = _currentToken.Position;
            var type = (PrimitiveTypes)_currentToken.TokenType;
            ConsumeToken();
            return new PrimitiveType(type, position);
        }

        /// <summary>
        /// EBNF: <c>dictType = 'dict', '&lt;', primitiveType, ',', primitiveType, '&gt;';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private DictionaryType ParseDictionaryType()
        {
            var dictPosition = _currentToken.Position;
            ValidateCurrentToken(TokenType.DictKeyword, null!);
            ValidateCurrentToken(TokenType.LessThanOperator, null!);

            if(!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var keyType = ParsePrimitiveType();

            ValidateCurrentToken(TokenType.CommaOperator, null!);

            if (!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var valueType = ParsePrimitiveType();

            ValidateCurrentToken(TokenType.GreaterThanOperator, null!);
            return new DictionaryType(keyType, valueType, dictPosition);
        }

        /// <summary>
        /// EBNF: <c>typeName = primitiveType | dictType;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IDeclarableType ParseDeclarableType()
        {
            if (_currentToken.TokenType == TokenType.DictKeyword)
                return ParseDictionaryType();

            if (!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var position = _currentToken.Position;
            var type = (PrimitiveTypes)_currentToken.TokenType;
            ConsumeToken();
            return new PrimitiveType(type, position);
        }

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
                var nextParameter = ParseParameter();

                if (nextParameter == null)
                    RaiseFactoryError(null!);

                parameters.Add(nextParameter!);
            }

            return parameters;
        }

        /// <summary>
        /// EBNF: <c>parameter = typeName, identifier;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private Parameter? ParseParameter()
        {
            if (!_currentToken.TokenType.IsDeclarableType())
                return null;

            var position = _currentToken.Position;
            var type = ParseDeclarableType();
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);
            return new Parameter(type, identifier, position);
        }

        #endregion

        #region Statements builders

        /// <summary>
        /// EBNF: <c>blockStatement = '{', { basicStatement }, '}';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BlockStatement? ParseBlockStatement()
        {
            if(_currentToken.TokenType != TokenType.OpenBlockOperator)
                return null;

            ConsumeToken();
            var statementsList = new List<IBasicStatement>();

            while (ParseBasicStatement() is { } statement)
                statementsList.Add(statement);

            ValidateCurrentToken(TokenType.CloseBlockOperator, null!);
            return new BlockStatement(statementsList);
        }

        /// <summary>
        /// EBNF: <c>basicStatement = ifStatement | whileStatement | continueStatement | breakStatement | tryStatement |
        /// throwStatement | expressionStatement | returnStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IBasicStatement? ParseBasicStatement()
        {
            IBasicStatement? statement = _currentToken.TokenType switch
            {
                TokenType.IfKeyword => ParseIfStatement(),
                TokenType.WhileKeyword => ParseWhileStatement(),
                TokenType.ContinueKeyword => ParseContinueStatement(),
                TokenType.BreakKeyword => ParseBreakStatement(),
                TokenType.ThrowKeyword => ParseThrowStatement(),
                TokenType.TryKeyword => ParseTryStatement(),
                TokenType.ReturnKeyword => ParseReturnStatement(),
                _ => null
            };

            if (statement != null)
                return statement;

            return _currentToken.TokenType.IsDeclarableType() 
                ? ParseVariableDeclaration()
                : ParseExpressionStatement();
        }

        /// <summary>
        /// EBNF: <c>ifStatement = 'if', '(', expression, ')', blockStatement, [ 'else', ( ifStatement | blockStatement ) ];</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IfStatement ParseIfStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.IfKeyword, null!);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var expression = ParseExpression();
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);

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

            return new IfStatement(expression, body, elseIfStatement, elseBlock, position);
        }

        /// <summary>
        /// EBNF: <c>whileStatement = 'while', '(', expression, ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private WhileStatement ParseWhileStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.WhileKeyword, null!);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var expression = ParseExpression();
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);

            var body = ParseBlockStatement();
            return new WhileStatement(expression, body, position);
        }

        /// <summary>
        /// EBNF: <c>variableDeclaration = typeName, identifier, [ '=', expression ], ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private VariableDeclaration ParseVariableDeclaration()
        {
            var position = _currentToken.Position;
            var type = ParseDeclarableType();
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
            {
                ValidateCurrentToken(TokenType.SemicolonOperator, null!);
                return new VariableDeclaration(type, identifier, position);
            }

            ConsumeToken();
            var initializationExpression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new VariableDeclaration(type, identifier, position, initializationExpression);
        }

        /// <summary>
        /// EBNF: <c>expressionStatement = expression, [ '=', expression ], ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ExpressionStatement? ParseExpressionStatement()
        {
            var position = _currentToken.Position;
            var expression = ParseExpression();

            if (expression == null)
                return null;

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
            {
                ValidateCurrentToken(TokenType.SemicolonOperator, null!);
                return new ExpressionStatement(expression, position);
            }

            ConsumeToken();
            var initializationExpression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ExpressionStatement(expression, position, initializationExpression);
        }

        /// <summary>
        /// EBNF: <c>continueStatement = 'continue', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ContinueStatement ParseContinueStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.ContinueKeyword, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new ContinueStatement(position);
        }

        /// <summary>
        /// EBNF: <c>breakStatement = 'break', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BreakStatement ParseBreakStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.BreakKeyword, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new BreakStatement(position);
        }

        /// <summary>
        /// EBNF: <c>throwStatement = 'throw', stringLiteral, ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ThrowStatement ParseThrowStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.ThrowKeyword, null!);
            var message = ValidateCurrentToken<string>(TokenType.StringLiteral, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ThrowStatement(message, position);
        }

        /// <summary>
        /// EBNF: <c>tryStatement = 'try', blockStatement, catchClause, { catchClause };</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private TryStatement ParseTryStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.TryKeyword, null!);

            var body = ParseBlockStatement();
            var catchClauses = new List<CatchClause>();

            while (ParseCatchClause() is { } catchClause)
                catchClauses.Add(catchClause);

            if (catchClauses.Count == 0)
                RaiseFactoryError(null!);

            return new TryStatement(body, catchClauses, position);
        }

        /// <summary>
        /// EBNF: <c>catchClause = 'catch', '(', identifier, [ expression ] ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private CatchClause? ParseCatchClause()
        {
            if (_currentToken.TokenType != TokenType.CatchKeyword)
                return null;

            var position = _currentToken.Position;

            ConsumeToken();
            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            IExpression? filterExpression = null;

            if (_currentToken.TokenType == TokenType.RightParenthesisOperator)
                ConsumeToken();
            else
            {
                filterExpression = ParseExpression();
                ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
            }

            var body = ParseBlockStatement();
            return new CatchClause(identifier, filterExpression, body, position);
        }

        /// <summary>
        /// EBNF: <c>returnStatement = 'return', expression, ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnStatement ParseReturnStatement()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.ReturnKeyword, null!);
            var expression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ReturnStatement(expression, position);
        }

        #endregion

        #region Expressions builders

        /// <summary>
        /// Helper method which parses a binary expression, that does not support connectivity.
        /// </summary>
        /// <param name="tokenTypeToFactories">Map of token types to corresponding factory methods of binary expressions.</param>
        /// <param name="innerFactory">Factory method which parses an expression in the inner tree.</param>
        /// <param name="errorOnInvalidInner">Error thrown when inner expression factory returned null.</param>
        /// <param name="connective">Determines whether the expression supports connectivity.</param>
        /// <returns>The parsed binary expression.</returns>
        private IExpression? ParseBinaryExpression(Dictionary<TokenType, BinaryExpressionFactory> tokenTypeToFactories,
            Func<IExpression?> innerFactory, ICompilationError errorOnInvalidInner, bool connective)
        {
            var leftFactor = innerFactory.Invoke();

            if (leftFactor == null)
                return null;

            while (tokenTypeToFactories.TryGetValue(_currentToken.TokenType, out var factory))
            {
                var position = _currentToken.Position;
                ConsumeToken();
                var rightFactor = innerFactory.Invoke();

                if (rightFactor == null)
                    RaiseFactoryError(errorOnInvalidInner);

                leftFactor = factory.Invoke(leftFactor, rightFactor!, position);

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
        /// <param name="errorOnInvalidInner">Error thrown when inner expression factory returned null.</param>
        /// <param name="connective">Determines whether the expression supports connectivity.</param>
        /// <returns>The parsed binary expression.</returns>
        private IExpression? ParseBinaryExpression(TokenType operatorType, BinaryExpressionFactory expressionFactory,
            Func<IExpression?> innerFactory, ICompilationError errorOnInvalidInner, bool connective)
        {
            return ParseBinaryExpression(
                new Dictionary<TokenType, BinaryExpressionFactory> { [operatorType] = expressionFactory }, 
                innerFactory,
                errorOnInvalidInner, 
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
                null!, 
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
                null!,
                true);
        }

        /// <summary>
        /// EBNF: <c>relationalExpression = additiveExpression, [ relationalOperator, additiveExpression ];</c>
        /// </summary>
        private IExpression? ParseRelationalExpression()
        {
            var relationalExpressionsFactory = new Dictionary<TokenType, BinaryExpressionFactory>
            {
                { TokenType.GreaterThanOperator, (left, right, position) => new GreaterThanExpression(left, right, position) },
                { TokenType.GreaterThanOrEqualOperator, (left, right, position) => new GreaterThanOrEqualExpression(left, right, position) },
                { TokenType.EqualOperator, (left, right, position) => new EqualExpression(left, right, position) },
                { TokenType.NotEqualOperator, (left, right, position) => new NotEqualExpression(left, right, position) },
                { TokenType.LessThanOperator, (left, right, position) => new LessThanExpression(left, right, position) },
                { TokenType.LessThanOrEqualOperator, (left, right, position) => new LessThanOrEqualExpression(left, right, position) }
            };

            return ParseBinaryExpression(relationalExpressionsFactory, ParseAdditiveExpression, null!, false);
        }

        /// <summary>
        /// EBNF: <c>additiveExpression = multiplicativeExpression, [ additiveOperator, multiplicativeExpression ];</c>
        /// </summary>
        private IExpression? ParseAdditiveExpression()
        {
            var additiveExpressionsFactory = new Dictionary<TokenType, BinaryExpressionFactory>
            {
                { TokenType.PlusOperator, (left, right, position) => new AdditionExpression(left, right, position) },
                { TokenType.MinusOperator, (left, right, position) => new SubtractionExpression(left, right, position) }
            };

            return ParseBinaryExpression(additiveExpressionsFactory, ParseMultiplicativeExpression, null!, true);
        }

        /// <summary>
        /// EBNF: <c>multiplicativeExpression = unaryExpression, [ multiplicativeOperator, unaryExpression ];</c>
        /// </summary>
        private IExpression? ParseMultiplicativeExpression()
        {
            var multiplicativeExpressionsFactory = new Dictionary<TokenType, BinaryExpressionFactory>
            {
                { TokenType.AsteriskOperator, (left, right, position) => new MultiplicationExpression(left, right, position) },
                { TokenType.SlashOperator, (left, right, position) => new DivisionExpression(left, right, position) },
                { TokenType.PercentOperator, (left, right, position) => new ModuloExpression(left, right, position) },
            };

            return ParseBinaryExpression(multiplicativeExpressionsFactory, ParseUnaryExpression, null!, true);
        }

        /// <summary>
        /// EBNF: <c>unaryExpression = [ ( '!' | '-' ) ], secondaryExpression;</c>
        /// </summary>
        private IExpression? ParseUnaryExpression()
        {
            var unaryExpressionsFactory = new Dictionary<TokenType, UnaryExpressionFactory>
            {
                { TokenType.MinusOperator, (expression, position) => new NegationExpression(expression, position) },
                { TokenType.NegationOperator, (expression, position) => new BooleanNegationExpression(expression, position) }
            };

            var hasOperator = unaryExpressionsFactory.TryGetValue(_currentToken.TokenType, out var factory);
            var position = _currentToken.Position;

            if (hasOperator)
                ConsumeToken();

            var innerExpression = ParseSecondaryExpression();
            if (innerExpression != null) 
                return factory?.Invoke(innerExpression, position) ?? innerExpression;

            if (hasOperator)
                RaiseFactoryError(null!);

            return null;
        }

        /// <summary>
        /// EBNF: <c>secondaryExpression = primaryExpression, [ '?' ];</c>
        /// </summary>
        private IExpression? ParseSecondaryExpression()
        {
            var innerExpression = ParsePrimaryExpression();
            if (innerExpression == null)
                return null;

            if (_currentToken.TokenType != TokenType.IsNullOperator)
                return innerExpression;

            var position = _currentToken.Position;
            ConsumeToken();
            return new NullCheckExpression(innerExpression, position);
        }

        /// <summary>
        /// EBNF: <c>primaryExpression = ( literal | identifierOrCall | parenthesisedExpression ), { memberAccess };</c>
        /// </summary>
        /// <returns></returns>
        private IExpression? ParsePrimaryExpression()
        {
            var position = _currentToken.Position;
            IExpression? firstExpression;

            if (_currentToken.TokenType.IsLiteral())
                firstExpression = ParseLiteral(_currentToken.TokenType);
            else
                firstExpression = _currentToken.TokenType switch
                {
                    TokenType.LeftParenthesisOperator => ParseParenthesisedExpression(),
                    TokenType.Identifier => ParseIdentifierOrCall(),
                    _ => null
                };

            if (firstExpression == null)
                return null;

            if (_currentToken.TokenType != TokenType.DotOperator)
                return firstExpression;

            var parentMember = new MemberAccessExpression(firstExpression, position);
            while (_currentToken.TokenType == TokenType.DotOperator)
            {
                position = _currentToken.Position;
                ConsumeToken();
                var identifierOrCall = ParseIdentifierOrCall();
                parentMember = new MemberAccessExpression(identifierOrCall, position, parentMember);
            }

            return parentMember;
        }

        /// <summary>
        /// EBNF: <c>identifierOrCall = identifier, [ '(', { expression }, ')' ];</c>
        /// </summary>
        private IExpression ParseIdentifierOrCall()
        {
            var position = _currentToken.Position;
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            return _currentToken.TokenType != TokenType.LeftParenthesisOperator 
                ? new IdentifierExpression(identifier, position) 
                : ParseCallExpression(identifier, position);
        }

        /// <summary>
        /// Parses a function call. Can be executed only by identifier or call parser.
        /// </summary>
        /// <param name="functionName">The name of the function to parse.</param>
        /// <param name="position">Position of the function.</param>
        private CallExpression ParseCallExpression(string functionName, Position position)
        {
            ConsumeToken();
            var argumentsList = new List<IExpression>();

            if (_currentToken.TokenType == TokenType.RightParenthesisOperator)
            {
                ConsumeToken();
                return new CallExpression(functionName, argumentsList, position);
            }

            var argument = ParseExpression();
            if (argument == null)
                RaiseFactoryError(null!);

            argumentsList.Add(argument!);

            while (_currentToken.TokenType == TokenType.CommaOperator)
            {
                ConsumeToken();
                argument = ParseExpression();
                if (argument == null)
                    RaiseFactoryError(null!);

                argumentsList.Add(argument!);
            }

            return new CallExpression(functionName, argumentsList, position);
        }

        private IExpression? ParseLiteral(TokenType literalType)
        {
            IExpression? value = literalType switch
            {
                TokenType.IntegerLiteral => BuildLiteral<int>(TokenType.IntegerLiteral, null!),
                TokenType.FloatLiteral => BuildLiteral<float>(TokenType.FloatLiteral, null!),
                TokenType.StringLiteral => BuildLiteral<string>(TokenType.StringLiteral, null!),
                TokenType.CharLiteral => BuildLiteral<char>(TokenType.CharLiteral, null!),
                TokenType.TrueKeyword => BuildLiteral<bool>(TokenType.TrueKeyword, null!),
                TokenType.FalseKeyword => BuildLiteral<bool>(TokenType.FalseKeyword, null!),
                TokenType.NullKeyword => BuildLiteral<object?>(TokenType.NullKeyword, null!),
                _ => null
            };

            return value;
        }

        /// <summary>
        /// Builds a literal expression basing on the expected type.
        /// </summary>
        /// <typeparam name="T">Type of the literal to build.</typeparam>
        /// <param name="literalType">Token type of the expected literal.</param>
        /// <param name="errorToThrow">Error to throw when building a literal failed.</param>
        /// <returns></returns>
        private LiteralExpression<T> BuildLiteral<T>(TokenType literalType, ICompilationError errorToThrow)
        {
            var position = _currentToken.Position;
            var value = ValidateCurrentToken<T>(literalType, errorToThrow);
            return new LiteralExpression<T>(value, position);
        }

        /// <summary>
        /// EBNF: <c>parenthesisedExpression = '(', expression, ')';</c>
        /// </summary>
        /// <returns></returns>
        private ParenthesisedExpression ParseParenthesisedExpression()
        {
            var position = _currentToken.Position;
            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var innerExpression = ParseExpression();

            if (innerExpression == null)
                RaiseFactoryError(null!);

            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);
            return new ParenthesisedExpression(innerExpression!, position);
        }

        #endregion
    }
}
