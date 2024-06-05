using CNull.Common;
using CNull.ErrorHandler;
using CNull.Interpreter.Context;
using CNull.Interpreter.Errors;
using CNull.Interpreter.Extensions;
using CNull.Interpreter.Resolvers;
using CNull.Interpreter.Symbols;
using CNull.Parser;
using CNull.Parser.Productions;
using CNull.Parser.Visitors;

namespace CNull.Interpreter
{
    public class Interpreter(IFunctionsRegistryBuilder functionsRegistryBuilder, IErrorHandler errorHandler) : IInterpreter, IAstVisitor
    {
        private FunctionsRegistry _functionsRegistry = new(errorHandler);

        private TypesResolver _typesResolver = null!;
        private InterpreterExecutionEnvironment _environment = null!;
        private StandardLibrary _standardLibrary = null!;

        public void Execute(string[] args, StandardInput inputCallback, StandardOutput outputCallback)
        {
            _environment = new InterpreterExecutionEnvironment();
            _typesResolver = new TypesResolver(_environment, errorHandler);
            _standardLibrary = new StandardLibrary(_environment, inputCallback, outputCallback);

            if (functionsRegistryBuilder.Build(_standardLibrary) is not { } functionsRegistry)
                return;

            _functionsRegistry = functionsRegistry;

            _environment.CurrentModule = functionsRegistryBuilder.RootModule;
            _environment.StackOverflowOccurred += (_, _)
                => throw errorHandler.RaiseRuntimeError(
                    new StackOverflowError(_environment.GetRecentCallStackRecords(100)));

            var mainFunction = _functionsRegistry.GetEntryPoint(_environment.CurrentModule)
                               ?? throw errorHandler.RaiseSemanticError(new MissingEntryPointError(_environment.CurrentModule));

            var dictContainer = ProcessArgs(args, mainFunction);
            PerformCall(mainFunction, dictContainer != null ? [dictContainer] : []);

            if (_environment.ActiveException != null)
                throw errorHandler.RaiseRuntimeError(new UnhandledExceptionError(
                    _environment.ActiveException.Exception, _environment.ActiveException.StackTrace));
        }

        #region Statements visitors

        public void Visit(Program program)
        { }

        public void Visit(ImportDirective directive)
        { }

        public void Visit(FunctionDefinition functionDefinition)
        {
            functionDefinition.FunctionBody.Accept(this);

            if (_environment is
                { CurrentContext: { IsReturning: false, ExpectedReturnType: not null }, ActiveException: null })
                throw errorHandler.RaiseSemanticError(new MissingReturnStatementError(_environment.CurrentModule, functionDefinition.Position.LineNumber));
        }

        public void Visit(StandardLibraryFunction standardLibraryFunction)
        {
            standardLibraryFunction.Body.Invoke();
        }

        public void Visit(EmbeddedFunction embeddedFunction)
        {
            embeddedFunction.Body.Invoke();
        }

        public void Visit(Parameter parameter)
        { }

        public void Visit(ReturnType returnType)
        { }

        public void Visit(PrimitiveType primitiveType)
        { }

        public void Visit(DictionaryType dictionaryType)
        { }

        public void Visit(BlockStatement blockStatement)
        {
            foreach (var statement in blockStatement.StatementsList)
            {
                if (_environment.CurrentContext.IsJumping || _environment.ActiveException != null)
                    break;

                statement.Accept(this);
            }
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            ValueContainer initializationValueContainer;

            if (variableDeclaration.InitializationExpression != null)
            {
                try
                {
                    initializationValueContainer = VisitExpression(variableDeclaration.InitializationExpression);
                }
                catch (ErrorInExpressionException)
                {
                    return;
                }
            }
            else if (!variableDeclaration.Type.IsPrimitive)
            {
                var type = _typesResolver.ResolveDeclarableType(variableDeclaration.Type);
                initializationValueContainer = ValueContainerFactory(variableDeclaration.Type, Activator.CreateInstance(type));
            }
            else
            {
                initializationValueContainer = ValueContainerFactory(variableDeclaration.Type, null);
            }

            var variable = new Variable(variableDeclaration.Name, initializationValueContainer);
            _environment.CurrentContext.DeclareVariable(variable);
        }

        public void Visit(ExpressionStatement expressionStatement)
        {
            if (expressionStatement is { Expression: not IdentifierExpression, AssignmentValue: not null })
                throw errorHandler.RaiseSemanticError(new InvalidAssignmentError(_environment.CurrentModule, expressionStatement.Position.LineNumber));

            if (expressionStatement.AssignmentValue == null)
            {
                try
                {
                    expressionStatement.Expression.Accept(this);
                }
                catch (ErrorInExpressionException)
                { }
            }
            else
            {
                var leftIdentifier = (IdentifierExpression)expressionStatement.Expression;
                var leftVariable = _environment.CurrentContext.TryGetVariable(leftIdentifier.Identifier) 
                                   ?? throw errorHandler.RaiseSemanticError(new UndefinedVariableError(_environment.CurrentModule, expressionStatement.Position.LineNumber));

                ValueContainer rightValue;
                try
                {
                    rightValue = VisitExpression(expressionStatement.AssignmentValue);
                }
                catch (ErrorInExpressionException)
                {
                    return;
                }
                
                var assignmentValue = _typesResolver.ResolveAssignment(leftVariable.ValueContainer, rightValue, leftIdentifier.Position.LineNumber);
                var (type, _, primitive) = leftVariable.ValueContainer;
                leftVariable.ValueContainer = new ValueContainer(type, assignmentValue, primitive);
            }
        }

        public void Visit(IfStatement ifStatement)
        {
            if (VisitBooleanExpression(ifStatement.BooleanExpression))
            {
                ProcessScope(ifStatement.Body);
            }
            else
            {
                ifStatement.ElseIfStatement?.Accept(this);
                if (ifStatement.ElseBlock != null)
                {
                    ProcessScope(ifStatement.ElseBlock);
                }
            }
        }

        public void Visit(WhileStatement whileStatement)
        {
            while (VisitBooleanExpression(whileStatement.BooleanExpression))
            {
                _environment.CurrentContext.EnterLoopScope();
                whileStatement.Body.Accept(this);
                _environment.CurrentContext.ExitLoopScope();

                if (_environment.CurrentContext.IsBreaking)
                    break;

                _environment.CurrentContext.IsContinuing = false;
            }

            _environment.CurrentContext.IsContinuing = false;
            _environment.CurrentContext.IsBreaking = false;
        }
          
        public void Visit(TryStatement tryCatchStatement)
        {
            ProcessScope(tryCatchStatement.TryBlock);
            if (_environment.ActiveException == null)
                return;

            foreach (var catchClause in tryCatchStatement.CatchClauses)
            {
                _environment.CurrentContext.EnterScope();
                var container = new ValueContainer(typeof(string), _environment.ActiveException.Exception);
                _environment.CurrentContext.DeclareVariable(new Variable(catchClause.Identifier, container));

                bool filterAccepted;
                try
                {
                    var caughtException = _environment.ActiveException;
                    _environment.ActiveException = null;
                    filterAccepted = catchClause.FilterExpression == null || VisitBooleanExpression(catchClause.FilterExpression);
                    _environment.ActiveException = caughtException;
                }
                catch (ErrorInExpressionException)
                {
                    return;
                }

                if (filterAccepted)
                {
                    _environment.ActiveException = null;
                    catchClause.Body.Accept(this);
                    _environment.CurrentContext.ExitScope();
                    break;
                }

                _environment.CurrentContext.ExitScope();
            }
        }

        public void Visit(CatchClause catchClause)
        { }

        public void Visit(ContinueStatement continueStatement)
        {
            if (_environment.CurrentContext.LoopCounter <= 0)
                throw errorHandler.RaiseSemanticError(new InvalidContinueError(_environment.CurrentModule, continueStatement.Position.LineNumber));

            _environment.CurrentContext.IsContinuing = true;
        }

        public void Visit(BreakStatement breakStatement)
        {
            if (_environment.CurrentContext.LoopCounter <= 0)
                throw errorHandler.RaiseSemanticError(new InvalidBreakError(_environment.CurrentModule, breakStatement.Position.LineNumber));

            _environment.CurrentContext.IsBreaking = true;
        }

        public void Visit(ThrowStatement throwStatement)
        {
            _environment.ActiveException = new ExceptionInfo(throwStatement.Message, throwStatement.Position.LineNumber);
        }

        public void Visit(ReturnStatement returnStatement)
        {
            if (_environment.CurrentContext.ExpectedReturnType == null && returnStatement.ReturnExpression != null)
                throw errorHandler.RaiseSemanticError(new VoidReturnError(_environment.CurrentModule, returnStatement.Position.LineNumber));

            if (_environment.CurrentContext.ExpectedReturnType != null)
            {
                if (returnStatement.ReturnExpression == null)
                    throw errorHandler.RaiseSemanticError(new MissingExpressionError(_environment.CurrentModule, returnStatement.Position.LineNumber));

                ValueContainer returnValue; 
                try
                {
                    returnValue = VisitExpression(returnStatement.ReturnExpression);
                }
                catch (ErrorInExpressionException)
                {
                    return;
                }

                if (returnValue.Value != null && _environment.CurrentContext.ExpectedReturnType != returnValue.Type)
                    throw errorHandler.RaiseSemanticError(new InvalidReturnExpressionError(_environment.CurrentModule, returnStatement.Position.LineNumber));

                _environment.SaveResult(returnValue);
            }

            _environment.CurrentContext.IsReturning = true;
        }

        #endregion

        #region Expressions visitors

        public void Visit(OrExpression orExpression)
        {
            var leftValue = VisitBooleanExpression(orExpression.LeftFactor);
            var result = leftValue || VisitBooleanExpression(orExpression.RightFactor);
            _environment.SaveResult(new ValueContainer(typeof(bool?), result));
        }

        public void Visit(AndExpression andExpression)
        {
            var leftValue = VisitBooleanExpression(andExpression.LeftFactor);
            var result = leftValue && VisitBooleanExpression(andExpression.RightFactor);
            _environment.SaveResult(new ValueContainer(typeof(bool?), result));
        }

        public void Visit(GreaterThanExpression greaterThanExpression)
        {
            VisitRelationalExpression(greaterThanExpression,
                (l, r, line) => _typesResolver.ResolveRelational(l, r, line) > 0);
        }

        public void Visit(LessThanExpression lessThanExpression)
        {
            VisitRelationalExpression(lessThanExpression,
                (l, r, line) => _typesResolver.ResolveRelational(l, r, line) < 0);
        }

        public void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            VisitRelationalExpression(greaterThanOrEqualExpression,
                (l, r, line) => _typesResolver.ResolveRelational(l, r, line) >= 0);
        }

        public void Visit(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            VisitRelationalExpression(lessThanOrEqualExpression, (l, r, line) => _typesResolver.ResolveRelational(l, r, line) <= 0);
        }

        public void Visit(EqualExpression equalExpression)
        {
            VisitRelationalExpression(equalExpression, _typesResolver.ResolveEquality);
        }

        public void Visit(NotEqualExpression notEqualExpression)
        {
            VisitRelationalExpression(notEqualExpression, (l, r, line) => !_typesResolver.ResolveEquality(l, r, line));
        }

        public void Visit(AdditionExpression additionExpression)
        {
            VisitArithmeticalExpression(additionExpression, _typesResolver.ResolveAddition);
        }

        public void Visit(SubtractionExpression subtractionExpression)
        {
            VisitArithmeticalExpression(subtractionExpression, _typesResolver.ResolveSubtraction);
        }

        public void Visit(MultiplicationExpression multiplicationExpression)
        {
            VisitArithmeticalExpression(multiplicationExpression, _typesResolver.ResolveMultiplication);
        }

        public void Visit(DivisionExpression divideExpression)
        {
            VisitArithmeticalExpression(divideExpression, _typesResolver.ResolveDivision);
        }

        public void Visit(ModuloExpression moduloExpression)
        {
            VisitArithmeticalExpression(moduloExpression, _typesResolver.ResolveModulo);
        }

        public void Visit(BooleanNegationExpression booleanNegationExpression)
        {
            var value = VisitBooleanExpression(booleanNegationExpression.Expression);
            _environment.SaveResult(new ValueContainer(typeof(bool?), !value));
        }

        public void Visit(NegationExpression negationExpression)
        {
            var value = VisitExpression(negationExpression.Expression);
            _environment.SaveResult(new ValueContainer(
                value.Value?.GetType().MakeNullableType() ?? typeof(object).MakeNullableType(),
                _typesResolver.ResolveNegation(value.Value, negationExpression.Position.LineNumber)));
        }

        public void Visit(NullCheckExpression nullCheckExpression)
        {
            var value = VisitExpression(nullCheckExpression.Expression);
            _environment.SaveResult(new ValueContainer(typeof(bool?), value.Value == null));
        }

        public void Visit<T>(LiteralExpression<T> literalExpression)
        {
            var type = literalExpression.Value?.GetType().MakeNullableType() ?? typeof(object).MakeNullableType();
            _environment.SaveResult(new ValueContainer(type, literalExpression.Value));
        }

        public void Visit(IdentifierExpression identifierExpression)
        {
            var variable = _environment.CurrentContext.TryGetVariable(identifierExpression.Identifier) 
                           ?? throw errorHandler.RaiseSemanticError(new UndefinedVariableError(_environment.CurrentModule, identifierExpression.Position.LineNumber));

            _environment.SaveResult(variable.ValueContainer.Move());
        }

        public void Visit(CallExpression callExpression)
        {
            var arguments = callExpression.Arguments.Select(VisitExpression).ToArray();

            switch (callExpression.ParentExpression)
            {
                case IdentifierExpression or LiteralExpression<string>:
                {
                    var parentValue = VisitExpression(callExpression.ParentExpression);
                    var function = _standardLibrary.GetEmbeddedFunction(callExpression.FunctionName, parentValue, callExpression.Position.LineNumber);

                    if (_environment.ActiveException != null)
                    {
                        _environment.SaveResult(parentValue);
                        break;
                    }

                    if (function == null)
                        throw errorHandler.RaiseSemanticError(
                            new FunctionNotFoundError(callExpression.FunctionName, _environment.CurrentModule,
                                callExpression.Position.LineNumber));

                    PerformCall(function, arguments, callExpression.Position.LineNumber);
                    break;
                }
                case null:
                {
                    if (!_functionsRegistry.TryGetValue(_environment.CurrentModule, callExpression.FunctionName, out var functionsRegistryEntry))
                        throw errorHandler.RaiseSemanticError(new FunctionNotFoundError(callExpression.FunctionName,
                            _environment.CurrentModule, callExpression.Position.LineNumber));

                    PerformCall(functionsRegistryEntry.FunctionDefinition, arguments,
                        callExpression.Position.LineNumber, functionsRegistryEntry.ExternalModuleName);

                    break;
                }
                default:
                    throw errorHandler.RaiseSemanticError(new InvalidMemberAccessError(_environment.CurrentModule, callExpression.Position.LineNumber));
            }
        }

        #endregion

        #region Helper methods

        private ValueContainer? ProcessArgs(string[] args, IFunction mainFunction)
        {
            var parameters = mainFunction.Parameters.ToList();
            if (parameters.Count == 0)
                return null;

            var validSignature = parameters is
            [
                {
                    Type: DictionaryType
                    {
                        KeyType.TypeSpecifier: PrimitiveTypes.Integer, ValueType.TypeSpecifier:
                        PrimitiveTypes.String
                    }
                }
            ];

            if (!validSignature)
                throw errorHandler.RaiseSemanticError(new InvalidMainSignatureError(_environment.CurrentModule));

            var dictionary = new Dictionary<int, string>();
            var index = 0;
            foreach (var arg in args)
            {
                dictionary.Add(index, arg);
                index++;
            }

            return new ValueContainer(typeof(Dictionary<int, string>), dictionary, IsPrimitive: false);
        }

        private void PerformCall(IFunction function, ValueContainer[] args, int callingLineNumber = 0, string? requestedModule = null)
        {
            if (function.Parameters.Count() != args.Length)
                throw errorHandler.RaiseSemanticError(new InvalidArgumentsCountError(function.Parameters.Count(),
                    args.Length, _environment.CurrentModule, callingLineNumber));

            var returnType = _typesResolver.ResolveReturnType(function.ReturnType);
            var localVariables = new List<Variable>();
            foreach (var (parameter, argument) in function.Parameters.Zip(args))
                localVariables.Add(new Variable(parameter.Name, argument.Move()));

            var lastModule = _environment.CurrentModule;
            var lastFunction = _environment.CurrentFunction;
            if (requestedModule != null)
                _environment.CurrentModule = requestedModule;

            _environment.CurrentFunction = function.Name;
            _environment.EnterCallContext(returnType?.MakeNullableType(), localVariables, new CallStackRecord(lastModule, lastFunction, callingLineNumber));
            function.Accept(this);
             _environment.ExitCallContext();

             _environment.CurrentModule = lastModule;
             _environment.CurrentFunction = lastFunction;
        }

        private void ProcessScope(BlockStatement body)
        {
            _environment.CurrentContext.EnterScope();
            body.Accept(this);
            _environment.CurrentContext.ExitScope();
        }

        private ValueContainer VisitExpression(IExpression expression)
        {
            expression.Accept(this);

            if (_environment.ActiveException == null)
                return _environment.ConsumeLastResult();

            _environment.SaveResult(null);
            throw new ErrorInExpressionException();
        }

        private bool VisitBooleanExpression(IExpression expression)
        {
            var result = VisitExpression(expression);
            return _typesResolver.EnsureBoolean(result.Value, expression.Position.LineNumber);
        }

        private void VisitRelationalExpression(IBinaryExpression binaryExpression, BooleanBinaryOperationResolver resolver)
        {
            var leftValue = VisitExpression(binaryExpression.LeftFactor);
            var rightValue = VisitExpression(binaryExpression.RightFactor);

            var result = resolver.Invoke(leftValue.Value, rightValue.Value, binaryExpression.Position.LineNumber);
            _environment.SaveResult(new ValueContainer(typeof(bool?), result));
        }

        private void VisitArithmeticalExpression(IBinaryExpression binaryExpression,
            BinaryOperationResolver resolver)
        {
            var leftValue = VisitExpression(binaryExpression.LeftFactor);
            var rightValue = VisitExpression(binaryExpression.RightFactor);

            var result = resolver.Invoke(leftValue.Value, rightValue.Value, binaryExpression.Position.LineNumber);
            _environment.SaveResult(new ValueContainer(result.GetType().MakeNullableType(), result));
        }

        private ValueContainer ValueContainerFactory(IDeclarableType type, object? initializationValue)
        {
            var leftType = _typesResolver.ResolveDeclarableType(type);
            var resolvedValue = _typesResolver.ResolveAssignment(
                leftType == typeof(string) ? string.Empty : Activator.CreateInstance(leftType), initializationValue,
                type.Position.LineNumber);
            return new ValueContainer(leftType.MakeNullableType(), resolvedValue, type.IsPrimitive);
        }

        #endregion
    }
}
