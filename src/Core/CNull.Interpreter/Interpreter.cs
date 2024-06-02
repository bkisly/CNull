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

                expressionStatement.AssignmentValue.Accept(this);
                var rightValue = _environment.ConsumeLastResult();
                var assignmentValue = _typesResolver.ResolveAssignment(leftVariable.ValueContainer.Value, rightValue.Value);
                leftVariable.ValueContainer.Value = assignmentValue;
            }
        }

        public void Visit(IfStatement ifStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(WhileStatement whileStatement)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void Visit(BreakStatement breakStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(ThrowStatement throwStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnStatement returnStatement)
        {
            throw new NotImplementedException();
        }

        public void Visit(OrExpression orExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(AndExpression andExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThanExpression greaterThanExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThanExpression lessThanExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThanOrEqualExpression greaterThanOrEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThanOrEqualExpression lessThanOrEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(EqualExpression equalExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NotEqualExpression notEqualExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(AdditionExpression additionExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(SubtractionExpression subtractionExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(MultiplicationExpression multiplicationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(DivisionExpression divideExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(ModuloExpression moduloExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(BooleanNegationExpression booleanNegationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NegationExpression negationExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(NullCheckExpression nullCheckExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit<T>(LiteralExpression<T> literalExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(IdentifierExpression identifierExpression)
        {
            throw new NotImplementedException();
        }

        public void Visit(CallExpression callExpression)
        {
            throw new NotImplementedException();
        }

        // call dostaje liste zmiennych, nie tworzy ich sam (powinny być tworzone przed callem)
        // wszystkie referencje można przechowywać w środowisku i przypisywać im hash cody jako klucz
        // albo kopiować explicite przy typach kopiowalnych podczas calla i returna...
        private void PerformCall(IFunction function, params IValueContainer[] args)
        {
            if (function.Parameters.Count() != args.Length)
                throw new NotImplementedException();

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
