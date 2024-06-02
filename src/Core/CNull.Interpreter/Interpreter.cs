using CNull.ErrorHandler;
using CNull.Interpreter.Context;
using CNull.Interpreter.Errors;
using CNull.Interpreter.Extensions;
using CNull.Interpreter.Resolvers;
using CNull.Interpreter.Symbols;
using CNull.Parser.Productions;
using CNull.Parser.Visitors;

namespace CNull.Interpreter
{
    public class Interpreter(IFunctionsRegistryBuilder functionsRegistryBuilder, IErrorHandler errorHandler) : IInterpreter, IAstVisitor
    {
        private StandardInput? _inputCallback;
        private StandardOutput? _outputCallback;

        private FunctionsRegistry _functionsRegistry = new(errorHandler);
        private string _rootModule = null!;

        private TypesResolver _typesResolver = null!;
        private InterpreterExecutionEnvironment _environment = null!;

        public void Execute(StandardInput inputCallback, StandardOutput outputCallback)
        {
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _typesResolver = new TypesResolver(errorHandler);
            _environment = new InterpreterExecutionEnvironment();

            if (functionsRegistryBuilder.Build() is not { } functionsRegistry)
                return;

            _rootModule = functionsRegistryBuilder.RootModule;
            _functionsRegistry = functionsRegistry;

            var mainFunction = _functionsRegistry.GetEntryPoint(_rootModule) 
                               ?? throw errorHandler.RaiseRuntimeError(new MissingEntryPointError(_rootModule));

            var dictContainer = new ReferenceTypeContainer(typeof(Dictionary<int, string>),
                new Dictionary<int, string> { [0] = "first", [1] = "second" });
            PerformCall(mainFunction, dictContainer);
        }

        public void Visit(Program program)
        { }

        public void Visit(ImportDirective directive)
        { }

        public void Visit(FunctionDefinition functionDefinition)
        {
            functionDefinition.FunctionBody.Accept(this);
        }

        public void Visit(StandardLibraryFunction standardLibraryFunction)
        {
            throw new NotImplementedException("Create a registry for stdlib functions.");
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
                statement.Accept(this);
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            IValueContainer initializationValueContainer;

            if (variableDeclaration.InitializationExpression != null)
            {
                variableDeclaration.InitializationExpression.Accept(this);
                initializationValueContainer = _environment.ConsumeLastResult();
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
                throw new NotImplementedException("Cannot assign to something different than identifier.");

            if (expressionStatement.AssignmentValue == null)
            {
                expressionStatement.Expression.Accept(this);
            }
            else
            {
                var leftIdentifier = (IdentifierExpression)expressionStatement.Expression;
                var leftVariable = _environment.CurrentContext.TryGetVariable(leftIdentifier.Identifier) 
                                   ?? throw new NotImplementedException("Undefined variable");

                var rightValue = VisitExpression(expressionStatement.AssignmentValue);
                var assignmentValue = _typesResolver.ResolveAssignment(leftVariable.ValueContainer.Value, rightValue.Value);
                leftVariable.ValueContainer.Value = assignmentValue;
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

        private void ProcessScope(BlockStatement body)
        {
            _environment.CurrentContext.EnterScope();
            body.Accept(this);
            _environment.CurrentContext.ExitScope();
        }

        public void Visit(WhileStatement whileStatement)
        {
            while (VisitBooleanExpression(whileStatement.BooleanExpression))
            {
                _environment.CurrentContext.EnterLoopScope();
                whileStatement.Body.Accept(this);
                _environment.CurrentContext.ExitLoopScope();
            }
        }

        private bool VisitBooleanExpression(IExpression expression)
        {
            var result = VisitExpression(expression);
            return _typesResolver.EnsureBoolean(result.Value);
        }

        private IValueContainer VisitExpression(IExpression expression)
        {
            expression.Accept(this);
            return _environment.ConsumeLastResult();
        }
          
        public void Visit(TryStatement tryCatchStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(CatchClause catchClause)
        {
            throw new NotImplementedException();
        }

        public void Visit(ContinueStatement continueStatement)
        {
            if (_environment.CurrentContext.LoopCounter <= 0)
                throw new NotImplementedException("Invalid continue");

            _environment.CurrentContext.IsContinuing = true;
        }

        public void Visit(BreakStatement breakStatement)
        {
            if (_environment.CurrentContext.LoopCounter <= 0)
                throw new NotImplementedException("Invalid break");

            _environment.CurrentContext.IsBreaking = true;
        }

        public void Visit(ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatement returnStatement)
        {
            if (_environment.CurrentContext.ExpectedReturnType == null && returnStatement.ReturnExpression != null)
                throw new NotImplementedException("Tried to return a value from void function");


            if (_environment.CurrentContext.ExpectedReturnType != null)
            {
                if (returnStatement.ReturnExpression == null)
                    throw new NotImplementedException("Expected an expression");

                returnStatement.ReturnExpression.Accept(this);
                var returnValue = _environment.ConsumeLastResult();

                if (_environment.CurrentContext.ExpectedReturnType != returnValue.Type)
                    throw new NotImplementedException("Expression return type does not match expected return type.");

                _environment.SaveResult(returnValue);
            }

            _environment.CurrentContext.IsReturning = true;
        }

        public void Visit(OrExpression orExpression)
        {
            var leftValue = VisitBooleanExpression(orExpression.LeftFactor);
            var result = leftValue || VisitBooleanExpression(orExpression.RightFactor);
            _environment.SaveResult(new ValueTypeContainer(typeof(bool?), result));
        }

        public void Visit(AndExpression andExpression)
        {
            var leftValue = VisitBooleanExpression(andExpression.LeftFactor);
            var result = leftValue && VisitBooleanExpression(andExpression.RightFactor);
            _environment.SaveResult(new ValueTypeContainer(typeof(bool?), result));
        }

        public void Visit(GreaterThanExpression greaterThanExpression)
        {
            VisitRelationalExpression(greaterThanExpression, _typesResolver.ResolveGreaterThan);
        }

        public void Visit(LessThanExpression lessThanExpression)
        {
            VisitRelationalExpression(lessThanExpression,
                (l, r) => !_typesResolver.ResolveGreaterThan(l, r) && !_typesResolver.ResolveEqualTo(l, r));
        }

        public void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            VisitRelationalExpression(greaterThanOrEqualExpression,
                (l, r) => _typesResolver.ResolveGreaterThan(l, r) || _typesResolver.ResolveEqualTo(l, r));
        }

        public void Visit(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            VisitRelationalExpression(lessThanOrEqualExpression, (l, r) => !_typesResolver.ResolveGreaterThan(l, r));
        }

        private void VisitRelationalExpression(IBinaryExpression binaryExpression, Func<object?, object?, bool> resolver)
        {
            var leftValue = VisitExpression(binaryExpression.LeftFactor);
            var rightValue = VisitExpression(binaryExpression.RightFactor);

            var result = resolver.Invoke(leftValue.Value, rightValue.Value);
            _environment.SaveResult(new ValueTypeContainer(typeof(bool?), result));
        }

        private void VisitArithmeticalExpression(IBinaryExpression binaryExpression,
            Func<object?, object?, object> resolver)
        {
            var leftValue = VisitExpression(binaryExpression.LeftFactor);
            var rightValue = VisitExpression(binaryExpression.RightFactor);

            var result = resolver.Invoke(leftValue.Value, rightValue.Value);
            _environment.SaveResult(new ValueTypeContainer(result.GetType().MakeNullableType(), result));
        }

        public void Visit(EqualExpression equalExpression)
        {
            VisitRelationalExpression(equalExpression, _typesResolver.ResolveEqualTo);
        }

        public void Visit(NotEqualExpression notEqualExpression)
        {
            VisitRelationalExpression(notEqualExpression, (l, r) => !_typesResolver.ResolveEqualTo(l, r));
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
            _environment.SaveResult(new ValueTypeContainer(typeof(bool?), !value));
        }

        public void Visit(NegationExpression negationExpression)
        {
            var value = VisitExpression(negationExpression.Expression);
            _environment.SaveResult(new ValueTypeContainer(value.Value?.GetType().MakeNullableType(),
                _typesResolver.ResolveNegation(value.Value)));
        }

        public void Visit(NullCheckExpression nullCheckExpression)
        {
            var value = VisitExpression(nullCheckExpression.Expression);
            _environment.SaveResult(new ValueTypeContainer(typeof(bool?), value.Value == null));
        }

        public void Visit<T>(LiteralExpression<T> literalExpression)
        {
            var type = literalExpression.Value?.GetType() ?? typeof(object).MakeNullableType();
            _environment.SaveResult(new ValueTypeContainer(type, literalExpression.Value));
        }

        public void Visit(IdentifierExpression identifierExpression)
        {
            var variable = _environment.CurrentContext.TryGetVariable(identifierExpression.Identifier) 
                           ?? throw new NotImplementedException("Undefined variable...");

            _environment.SaveResult(variable.ValueContainer);
        }

        public void Visit(CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        private void PerformCall(IFunction function, params IValueContainer[] args)
        {
            if (function.Parameters.Count() != args.Length)
                throw new NotImplementedException("Not matching amount of args...");

            var returnType = _typesResolver.ResolveReturnType(function.ReturnType);
            var localVariables = new List<Variable>();
            foreach (var (parameter, argument) in function.Parameters.Zip(args))
                localVariables.Add(new Variable(parameter.Name, argument));

            _environment.EnterCallContext(returnType, localVariables);
            function.Accept(this);
             _environment.ExitCallContext();
        }

        private IValueContainer ValueContainerFactory(IDeclarableType type, object? initializationValue)
        {
            var leftType = _typesResolver.ResolveDeclarableType(type);
            var resolvedValue = _typesResolver.ResolveAssignment(Activator.CreateInstance(leftType), initializationValue);
            return leftType.IsPrimitive
                ? new ValueTypeContainer(leftType.MakeNullableType(), resolvedValue)
                : new ReferenceTypeContainer(leftType.MakeNullableType(), resolvedValue);
        }
    }
}
