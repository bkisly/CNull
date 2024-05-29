using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.Interpreter.Errors;
using CNull.Parser;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Symbols
{
    public class FunctionsRegistryBuilder(IParser parser, IErrorHandler errorHandler, IStateManager stateManager) : IFunctionsRegistryBuilder
    {
        private FunctionsRegistry _functionsRegistry = null!;
        private DependencyTree<string> _dependencyTree = new();
        private Dictionary<string, Program> _parsedProgramsCache = [];

        private Dictionary<string, Stack<ImportDirective>> _importsToProcess = [];
        private HashSet<string> _processedModules = [];

        public string RootModule { get; private set; } = string.Empty;

        public FunctionsRegistry? Build()
        {
            _functionsRegistry = new FunctionsRegistry(errorHandler);
            _parsedProgramsCache = [];
            _dependencyTree = new DependencyTree<string>();
            _importsToProcess = [];
            _processedModules = [];

            var rootProgram = ParseAndCacheProgram();
            if (rootProgram == null)
                return null;

            RootModule = rootProgram.ModuleName;
            RegisterFunctions(rootProgram);
            return _functionsRegistry;
        }

        private void RegisterFunctions(Program program)
        {
            var moduleName = program.ModuleName;
            if (_processedModules.Contains(moduleName))
                return;

            ProcessImports(program);

            foreach (var functionDefinition in program.FunctionDefinitions)
                _functionsRegistry.Register(moduleName, functionDefinition);

            while (_importsToProcess[moduleName].Count != 0)
            {
                var (requestedModule, requestedFunctionName, importPosition) = _importsToProcess[moduleName].Pop();
                if (!_parsedProgramsCache.TryGetValue(requestedModule, out var importedProgram))
                {
                    if (!stateManager.TryOpenModule(requestedModule))
                        throw errorHandler.RaiseFatalCompilationError(new ModuleNotFoundError(requestedModule, importPosition));

                    importedProgram = ParseAndCacheProgram();
                }

                if (importedProgram == null)
                    throw errorHandler.RaiseFatalCompilationError(new ModuleCompilationError(requestedModule, importPosition));

                RegisterFunctions(importedProgram);

                var desiredFunction = importedProgram.FunctionDefinitions.SingleOrDefault(f => f.Name == requestedFunctionName)
                                      ?? throw errorHandler.RaiseFatalCompilationError(
                                          new FunctionNotFoundError(requestedFunctionName, requestedModule, importPosition));

                _functionsRegistry.Register(moduleName, desiredFunction, requestedModule);
            }

            _processedModules.Add(moduleName);
        }

        private void ProcessImports(Program program)
        {
            var moduleName = program.ModuleName;
            if (_importsToProcess.ContainsKey(moduleName))
                return;

            _importsToProcess.Add(moduleName, []);
            var groupedImports = program.ImportDirectives.GroupBy(i => i.ModuleName);

            foreach (var importGroup in groupedImports)
            {
                _dependencyTree.AddDependency(program.ModuleName, importGroup.Key);
                if (!_dependencyTree.Build())
                    throw errorHandler.RaiseFatalCompilationError(new CircularDependencyError(importGroup.First().Position));

                foreach (var importDirective in importGroup.Distinct())
                    _importsToProcess[moduleName].Push(importDirective);
            }
        }

        private Program? ParseAndCacheProgram()
        {
            var program = parser.Parse();

            if (program == null)
                return program;

            _parsedProgramsCache.TryAdd(program.ModuleName, program);
            return program;
        }
    }
}
