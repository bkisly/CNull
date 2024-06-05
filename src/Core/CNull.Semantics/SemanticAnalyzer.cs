using CNull.Common.State;
using CNull.ErrorHandler;
using CNull.Semantics.Symbols;
using CNull.Parser;
using CNull.Parser.Productions;
using CNull.Semantics.Errors;

namespace CNull.Semantics
{
    public class SemanticAnalyzer(IParser parser, IErrorHandler errorHandler, IStateManager stateManager) : ISemanticAnalyzer
    {
        private FunctionsRegistry _functionsRegistry = null!;
        private DependencyTree<string> _dependencyTree = new();
        private Dictionary<string, Program> _parsedProgramsCache = [];

        private Dictionary<string, Stack<ImportDirective>> _importsToProcess = [];
        private HashSet<string> _processedModules = [];

        private const string CNullModule = "CNull";
        private IStandardLibrary _standardLibrary = null!;

        public string RootModule { get; private set; } = string.Empty;

        public FunctionsRegistry? Analyze(IStandardLibrary standardLibrary)
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
                        throw errorHandler.RaiseSemanticError(new ModuleNotFoundError(program.ModuleName,
                            requestedModule, importPosition.LineNumber));

                    importedProgram = ParseAndCacheProgram();
                }

                if (importedProgram == null)
                    throw errorHandler.RaiseSemanticError(new ModuleCompilationError(program.ModuleName,
                        requestedModule, importPosition.LineNumber));

                RegisterFunctions(importedProgram);

                var desiredFunction = importedProgram.FunctionDefinitions.SingleOrDefault(f => f.Name == requestedFunctionName)
                                      ?? throw errorHandler.RaiseSemanticError(
                                          new FunctionNotFoundError(requestedFunctionName, requestedModule, importPosition.LineNumber));

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
                    throw errorHandler.RaiseSemanticError(new CircularDependencyError(program.ModuleName, importGroup.First().Position.LineNumber));

                foreach (var importDirective in importGroup.Distinct())
                {
                    if (importDirective.ModuleName == CNullModule)
                        ProcessStdlibImport(moduleName, importDirective);
                    else _importsToProcess[moduleName].Push(importDirective);
                }
            }
        }

        private void ProcessStdlibImport(string currentModule, ImportDirective importDirective)
        {
            if (importDirective.SubmoduleName == null)
                throw errorHandler.RaiseSemanticError(new MissingSubmoduleError(currentModule, importDirective.Position.LineNumber));

            var requestedHeader = new StandardLibraryHeader(importDirective.SubmoduleName, importDirective.FunctionName);

            if (!_standardLibrary.StandardLibraryFunctions.TryGetValue(requestedHeader, out var function))
                throw errorHandler.RaiseSemanticError(new FunctionNotFoundError(importDirective.FunctionName,
                    $"${CNullModule}.{importDirective.SubmoduleName}", importDirective.Position.LineNumber));

            _functionsRegistry.Register(currentModule, function, CNullModule);
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
