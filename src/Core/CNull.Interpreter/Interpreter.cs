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

            PerformCall(mainFunction, (Dictionary<int, string>?)null);
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
            object? initializationValue = null;

            if (variableDeclaration.InitializationExpression != null)
            {
                variableDeclaration.InitializationExpression.Accept(this);
                initializationValue = _environment.ConsumeLastResult();
            }

            var variable = VariableFactory(variableDeclaration.Type, variableDeclaration.Name, initializationValue);
            _environment.CurrentContext.DeclareVariable(variable);
        }

        public void Visit(ExpressionStatement expressionStatement)
        {
            throw new NotImplementedException();
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

        private void PerformCall(IFunction function, params object?[] args)
        {
            if (function.Parameters.Count() != args.Length)
                return;

            var localVariables = new List<IVariable>();
            foreach (var (parameter, argument) in function.Parameters.Zip(args))
                localVariables.Add(VariableFactory(parameter.Type, parameter.Name, argument));

            _environment.EnterCallContext(localVariables);
            function.Accept(this);
            _environment.ExitCallContext();
        }

        private IVariable VariableFactory(IDeclarableType type, string name, object? initializationValue)
        {
            var leftType = _typesResolver.ResolveType(type);
            var resolvedValue = _typesResolver.ResolveAssignment(Activator.CreateInstance(leftType), initializationValue);

            if (!type.IsPrimitive)
                return new ReferenceTypeVariable(leftType.MakeNullableType(), name, resolvedValue);

            return new ValueTypeVariable(leftType.MakeNullableType(), name, resolvedValue);
        }
    }
}
