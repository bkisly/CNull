using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Interpreter.Errors
{
    public record CircularDependencyError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Circular dependency detected.";
    }

    public record FunctionRedefinitionError(string ModuleName, string FunctionName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Function with name: {FunctionName} has already been defined.";
    }

    public record FunctionNotFoundError(string FunctionName, string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"The function: {FunctionName} was not found in module: {ModuleName}.";
    }

    public record ModuleNotFoundError(string ModuleName, string RequestedModule, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Module: {RequestedModule} was not found in the working directory.";
    }

    public record MissingSubmoduleError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Missing submodule specification in standard library function import.";
    }

    public record ModuleCompilationError(string ModuleName, string RequestedModule, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Compilation of module: {RequestedModule} has finished with errors.";
    }

    public record MissingEntryPointError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Entry point ('Main' function) not found in root module: {ModuleName}.";
    }

    public record UnhandledExceptionError(string Exception, IEnumerable<CallStackRecord> CallStack) : IRuntimeError
    {
        public string Message { get; } = Exception;
    }

    public record StackOverflowError(IEnumerable<CallStackRecord> CallStack) : IRuntimeError
    {
        public string Message => "Stack overflow. Showing last 100 call stack entries.";
    }
}
