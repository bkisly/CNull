using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.Interpreter.Errors;
using CNull.Interpreter.Symbols.StandardLibrary;
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

        private StandardLibrary.StandardLibrary _standardLibrary = null!;

        public string RootModule { get; private set; } = string.Empty;

        public FunctionsRegistry? Build(StandardLibrary.StandardLibrary standardLibrary)
        {
            _functionsRegistry = new FunctionsRegistry(errorHandler);
            _parsedProgramsCache = [];
            _dependencyTree = new DependencyTree<string>();
            _importsToProcess = [];
            _processedModules = [];
            _standardLibrary = standardLibrary;

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

            var moduleImports = _importsToProcess[moduleName];
            while (moduleImports.Count != 0)
            {
                var (requestedModule, requestedFunctionName, importPosition, _) = moduleImports.Pop();
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
                {
                    if (importDirective.ModuleName == StandardLibrary.StandardLibrary.CNullModule)
                        ProcessStdlibImport(moduleName, importDirective);
                    else _importsToProcess[moduleName].Push(importDirective);
                }
            }
        }

        private void ProcessStdlibImport(string currentModule, ImportDirective importDirective)
        {
            if (importDirective.SubmoduleName == null)
                throw errorHandler.RaiseFatalCompilationError(new MissingSubmoduleError(importDirective.Position));

            var requestedSignature = new StandardLibrarySignature(importDirective.SubmoduleName, importDirective.FunctionName);
            if (!_standardLibrary.StandardLibraryFunctions.TryGetValue(requestedSignature, out var function))
                throw errorHandler.RaiseFatalCompilationError(new FunctionNotFoundError(importDirective.FunctionName,
                    $"${StandardLibrary.StandardLibrary.CNullModule}.{importDirective.SubmoduleName}", importDirective.Position));

            _functionsRegistry.Register(currentModule, function, StandardLibrary.StandardLibrary.CNullModule);
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
