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
            if (!TryRegister(moduleName, functionDefinition, externalModuleName))
                throw errorHandler.RaiseFatalCompilationError(
                    new FunctionRedefinitionError(functionDefinition.Name, functionDefinition.Position));
        }

        public bool TryRegister(string moduleName, FunctionDefinition functionDefinition, string? externalModuleName = null)
        {
            try
            {
                _functionDefinitions.Add((moduleName, functionDefinition.Name),
                    new FunctionsRegistryEntry(functionDefinition, externalModuleName));
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public bool ContainsModule(string moduleName) => _functionDefinitions.Keys.Any(s => s.ModuleName == moduleName);
    }
}
