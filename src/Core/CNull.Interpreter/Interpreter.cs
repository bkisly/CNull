using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.Interpreter.Errors;
using CNull.Parser;
using CNull.Parser.Productions;
using CNull.Parser.Visitors;

namespace CNull.Interpreter
{
    public class Interpreter(IParser parser, IErrorHandler errorHandler, IStateManager stateManager) : IInterpreter, IAstVisitor
    {
        private StandardInput? _inputCallback;
        private StandardOutput? _outputCallback;

        private Program _currentProgram = null!;

        private DependencyTree<string> _dependencyTree = new();
        private Queue<ImportDirective> _modulesToVisit = [];

        private Dictionary<string, Program> _parsedProgramsCache = [];
        private Dictionary<string, FunctionDefinition> _functionDefinitions = [];

        public void Execute(StandardInput inputCallback, StandardOutput outputCallback)
        {
            _inputCallback = inputCallback;
            _outputCallback = outputCallback;
            _dependencyTree = new DependencyTree<string>();
            _functionDefinitions = [];
            _modulesToVisit = [];
            _parsedProgramsCache = [];

            var program = ParseAndCacheProgram();
            if (program == null || errorHandler.Errors.Any())
                return;

            //program.Accept(new AstStringifierVisitor());
            program.Accept(this);

            foreach (var functionDefinition in program.FunctionDefinitions)
                RegisterFunction(functionDefinition);

            //_functionDefinitions["Main"].Accept(this);
        }

        public void Visit(Program program)
        {
            _currentProgram = program;

            foreach (var importDirective in program.ImportDirectives)
                importDirective.Accept(this);

            while (_modulesToVisit.Count != 0)
            {
                var module = _modulesToVisit.Dequeue();
                stateManager.NotifyInputRequested(
                    new Lazy<TextReader>(() => new StreamReader($"{module.ModuleName}.cnull")),
                    $"{module.ModuleName}.cnull");

                if (!_parsedProgramsCache.TryGetValue(module.ModuleName, out var importedProgram))
                    importedProgram = ParseAndCacheProgram();

                if (importedProgram == null)
                    throw new NotImplementedException();

                importedProgram.Accept(this);
                var desiredFunction = importedProgram.FunctionDefinitions.SingleOrDefault(f => f.Name == module.FunctionName) 
                                      ?? throw new NotImplementedException();

                RegisterFunction(desiredFunction);
            }
        }

        public void Visit(ImportDirective directive)
        {
            _dependencyTree.AddDependency(_currentProgram.ModuleName, directive.ModuleName);

            if (!_dependencyTree.Build())
                throw errorHandler.RaiseFatalCompilationError(new CircularDependencyError(directive.Position));

            if(!_modulesToVisit.Contains(directive))
                _modulesToVisit.Enqueue(directive);
        }

        public void Visit(FunctionDefinition functionDefinition)
        {
            throw new NotImplementedException();
        }

        public void Visit(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public void Visit(ReturnType returnType)
        {
            throw new NotImplementedException();
        }

        public void Visit(PrimitiveType primitiveType)
        {
            throw new NotImplementedException();
        }

        public void Visit(DictionaryType dictionaryType)
        {
            throw new NotImplementedException();
        }

        public void Visit(BlockStatement statement)
        {
            throw new NotImplementedException();
        }

        public void Visit(VariableDeclaration variableDeclaration)
        {
            throw new NotImplementedException();
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

        private Program? ParseAndCacheProgram()
        {
            var program = parser.Parse();

            if (program == null)
                return program;

            _parsedProgramsCache.TryAdd(program.ModuleName, program);
            return program;
        }

        private void RegisterFunction(FunctionDefinition functionDefinition)
        {
            try
            {
                _functionDefinitions.Add(functionDefinition.Name, functionDefinition);
            }
            catch (ArgumentException)
            {
                throw errorHandler.RaiseFatalCompilationError(new FunctionRedefinitionError(functionDefinition.Name, functionDefinition.Position));
            }
        }
    }
}
