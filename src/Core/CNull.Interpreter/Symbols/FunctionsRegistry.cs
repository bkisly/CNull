using CNull.ErrorHandler;
using CNull.Interpreter.Errors;
using CNull.Parser.Productions;

namespace CNull.Interpreter.Symbols
{
    public record FunctionsRegistryEntry(FunctionDefinition FunctionDefinition, string? ExternalModuleName);

    public class FunctionsRegistry(IErrorHandler errorHandler)
    {
        private readonly Dictionary<(string ModuleName, string FunctionName), FunctionsRegistryEntry> _functionDefinitions = [];

        public FunctionsRegistryEntry this[string moduleName, string functionName] => _functionDefinitions[(moduleName, functionName)];

        public void Register(string moduleName, FunctionDefinition functionDefinition, string? externalModuleName = null)
        {
            try
            {
                _functionDefinitions.Add((moduleName, functionDefinition.Name),
                    new FunctionsRegistryEntry(functionDefinition, externalModuleName));
            }
            catch (ArgumentException)
            {
                throw errorHandler.RaiseFatalCompilationError(
                    new FunctionRedefinitionError(functionDefinition.Name, functionDefinition.Position));
            }
        }

        public FunctionDefinition? GetEntryPoint(string rootModule)
        {
            return _functionDefinitions.TryGetValue((rootModule, "Main"), out var functionDefinition)
                ? functionDefinition.FunctionDefinition
                : null;
        }

        public bool ContainsModule(string moduleName) => _functionDefinitions.Keys.Any(s => s.ModuleName == moduleName);
    }
}
