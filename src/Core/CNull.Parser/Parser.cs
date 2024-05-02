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

        public Program? Parse()
        {
            try
            {
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
        private void ConsumeToken() => _currentToken = _lexer.GetNextToken();

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
            _errorHandler.RaiseCompilationError(errorToThrow);
            throw new UnexpectedTokenException();
        }

        #endregion

        #region Top level productions builders

        /// <summary>
        /// EBNF: <c>importDirective = 'import', identifier, '.', identifier;</c>  
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ImportDirective ParseImportDirective()
        {
            ValidateCurrentToken(TokenType.ImportKeyword, null!);

            var moduleName = ValidateCurrentToken<string>(TokenType.Identifier, null!);
            var functionName = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new ImportDirective(moduleName, functionName);
        }

        /// <summary>
        /// EBNF: <c>functionDefinition = typeName, identifier, '(', [ parameter ], ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private FunctionDefinition ParseFunctionDefinition()
        {
            var returnType = ParseReturnType();
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var parameters = ParseParametersList();
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);

            var functionBody = ParseBlockStatement();
            return new FunctionDefinition(returnType, identifier, parameters, functionBody);
        }

        #endregion

        #region Types and parameters builders

        /// <summary>
        /// EBNF: <c>returnType = 'void' | typeName;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnType ParseReturnType()
        {
            if (!_currentToken.TokenType.IsReturnType())
                RaiseFactoryError(null!);

            return _currentToken.TokenType switch
            {
                TokenType.VoidKeyword => new ReturnType(),
                TokenType.DictKeyword => ParseDictionaryType(),
                _ => new PrimitiveType((PrimitiveTypes)_currentToken.TokenType)
            };
        }

        /// <summary>
        /// EBNF: <c>dictType = 'dict', '&lt;', primitiveType, ',', primitiveType, '&gt;';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private DictionaryType ParseDictionaryType()
        {
            ValidateCurrentToken(TokenType.DictKeyword, null!);
            ValidateCurrentToken(TokenType.LessThanOperator, null!);

            if(!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var keyType = new PrimitiveType((PrimitiveTypes)_currentToken.TokenType);
            ConsumeToken();

            ValidateCurrentToken(TokenType.CommaOperator, null!);

            if (!_currentToken.TokenType.IsPrimitiveType())
                RaiseFactoryError(null!);

            var valueType = new PrimitiveType((PrimitiveTypes)_currentToken.TokenType);
            ConsumeToken();

            return new DictionaryType(keyType, valueType);
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

            var type = (PrimitiveTypes)_currentToken.TokenType;
            ConsumeToken();
            return new PrimitiveType(type);
        }

        /// <summary>
        /// EBNF: <c>parametersList = [ parameter, { ',', parameter } ];</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private IEnumerable<Parameter> ParseParametersList()
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

            var type = ParseDeclarableType();
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);
            return new Parameter(type, identifier);
        }

        #endregion

        #region Statements builders

        /// <summary>
        /// EBNF: <c>blockStatement = '{', { basicStatement }, '}';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BlockStatement ParseBlockStatement()
        {
            ValidateCurrentToken(TokenType.OpenBlockOperator, null!);
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

            return new IfStatement(expression, body, elseIfStatement, elseBlock);
        }

        /// <summary>
        /// EBNF: <c>whileStatement = 'while', '(', expression, ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private WhileStatement ParseWhileStatement()
        {
            ValidateCurrentToken(TokenType.WhileKeyword, null!);

            ValidateCurrentToken(TokenType.LeftParenthesisOperator, null!);
            var expression = ParseExpression();
            ValidateCurrentToken(TokenType.RightParenthesisOperator, null!);

            var body = ParseBlockStatement();
            return new WhileStatement(expression, body);
        }

        /// <summary>
        /// EBNF: <c>variableDeclaration = typeName, identifier, [ '=', expression ], ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private VariableDeclaration ParseVariableDeclaration()
        {
            var type = ParseDeclarableType();
            var identifier = ValidateCurrentToken<string>(TokenType.Identifier, null!);

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
            {
                ValidateCurrentToken(TokenType.SemicolonOperator, null!);
                return new VariableDeclaration(type, identifier);
            }

            ConsumeToken();
            var initializationExpression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new VariableDeclaration(type, identifier, initializationExpression);
        }

        /// <summary>
        /// EBNF: <c>expressionStatement = expression, [ '=', expression ], ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ExpressionStatement ParseExpressionStatement()
        {
            var expression = ParseExpression();

            if (_currentToken.TokenType != TokenType.AssignmentOperator)
            {
                ValidateCurrentToken(TokenType.SemicolonOperator, null!);
                return new ExpressionStatement(expression);
            }

            ConsumeToken();
            var initializationExpression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ExpressionStatement(expression, initializationExpression);
        }

        /// <summary>
        /// EBNF: <c>continueStatement = 'continue', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ContinueStatement ParseContinueStatement()
        {
            ValidateCurrentToken(TokenType.ContinueKeyword, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new ContinueStatement();
        }

        /// <summary>
        /// EBNF: <c>breakStatement = 'break', ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private BreakStatement ParseBreakStatement()
        {
            ValidateCurrentToken(TokenType.BreakKeyword, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);
            return new BreakStatement();
        }

        /// <summary>
        /// EBNF: <c>throwStatement = 'throw', stringLiteral, ';'</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ThrowStatement ParseThrowStatement()
        {
            ValidateCurrentToken(TokenType.ThrowKeyword, null!);
            var message = ValidateCurrentToken<string>(TokenType.StringLiteral, null!);
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ThrowStatement(message);
        }

        /// <summary>
        /// EBNF: <c>tryStatement = 'try', blockStatement, catchClause, { catchClause };</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private TryStatement ParseTryStatement()
        {
            ValidateCurrentToken(TokenType.TryKeyword, null!);

            var body = ParseBlockStatement();
            var catchClauses = new List<CatchClause>();

            while (ParseCatchClause() is { } catchClause)
                catchClauses.Add(catchClause);

            if (catchClauses.Count == 0)
                RaiseFactoryError(null!);

            return new TryStatement(body, catchClauses);
        }

        /// <summary>
        /// EBNF: <c>catchClause = 'catch', '(', identifier, [ expression ] ')', blockStatement;</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private CatchClause? ParseCatchClause()
        {
            if (_currentToken.TokenType != TokenType.CatchKeyword)
                return null;

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
            return new CatchClause(identifier, filterExpression, body);
        }

        /// <summary>
        /// EBNF: <c>returnStatement = 'return', expression, ';';</c>
        /// </summary>
        /// <exception cref="UnexpectedTokenException"/>
        private ReturnStatement ParseReturnStatement()
        {
            ValidateCurrentToken(TokenType.ReturnKeyword, null!);
            var expression = ParseExpression();
            ValidateCurrentToken(TokenType.SemicolonOperator, null!);

            return new ReturnStatement(expression);
        }

        #endregion

        #region Expressions builders

        private IExpression ParseExpression()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
