using CNull.ErrorHandler;
using CNull.Interpreter.Errors;
using CNull.Parser.Productions;

namespace CNull.Interpreter
{
    internal class FunctionsRegistry(IErrorHandler errorHandler)
    {
        private readonly Dictionary<string, FunctionDefinition> _functionDefinitions = [];
        private readonly Dictionary<FunctionDefinition, string> _funtionsToModuleNames = [];

        public FunctionDefinition this[string functionName] => _functionDefinitions[functionName];

        public void Register(FunctionDefinition functionDefinition, string moduleName)
        {
            try
            {
                _functionDefinitions.Add(functionDefinition.Name, functionDefinition);
                _funtionsToModuleNames.Add(functionDefinition, moduleName);
            }
            catch (ArgumentException)
            {
                throw errorHandler.RaiseFatalCompilationError(new FunctionRedefinitionError(functionDefinition.Name, functionDefinition.Position));
            }
        }

        public string GetFunctionModuleName(FunctionDefinition functionDefinition) =>
            _funtionsToModuleNames[functionDefinition];
    }
}
