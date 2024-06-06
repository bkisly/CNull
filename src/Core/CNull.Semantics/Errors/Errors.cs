using CNull.ErrorHandler.Errors;

namespace CNull.Semantics.Errors
{
    public record CircularDependencyError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Circular dependency detected.";
    }

    public record FunctionRedefinitionError(string ModuleName, string FunctionName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Function with name: {FunctionName} has already been defined.";
    }

    public record VariableRedeclarationError(string ModuleName, string VariableName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Variable with name: {VariableName} has already been defined in the current scope.";
    }

    public record DuplicateParameterNameError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"The function has duplicate parameter names.";
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

    public record MissingReturnStatementError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Missing return statement in non-void function.";
    }

    public record InvalidAssignmentError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Assignment to an element other than a variable is invalid.";
    }

    public record UndefinedVariableError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Tried to access an undefined variable.";
    }

    public record InvalidContinueError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Continue statement is invalid in this context.";
    }

    public record InvalidBreakError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Break statement is invalid in this context.";
    }

    public record VoidReturnError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Tried to return a value from void function.";
    }

    public record MissingExpressionError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Expected an expression.";
    }

    public record InvalidReturnExpressionError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Expression type does not match function return type.";
    }

    public record InvalidArgumentsCountError(int ExpectedCount, int ActualCount, string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => $"Invalid amount of arguments passed to the function. Expected: {ExpectedCount}, actual: {ActualCount}.";
    }

    public record InvalidMemberAccessError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Unsupported member access.";
    }

    public record TypeError : ISemanticError
    {
        public string Message { get; }
        public string ModuleName { get; }
        public int? LineNumber { get; }

        public TypeError(string expectedType, string actualType, string moduleName, int? lineNumber = null)
        {
            ModuleName = moduleName;
            LineNumber = lineNumber;
            Message = $"In the following expression, expected type ({expectedType}) did not match the actual type ({actualType})";
        }

        public TypeError(string operatorString, string leftType, string rightType, string moduleName,
            int? lineNumber = null)
        {
            ModuleName = moduleName;
            LineNumber = lineNumber;
            Message = $"Cannot perform an operation '{operatorString}' between types: {leftType} and {rightType}";
        }
    }

    public record InvalidMainSignatureError(string ModuleName, int? LineNumber = null) : ISemanticError
    {
        public string Message => "Incorrect Main function signature. C? supports either empty Main or containing one dict<int, string> parameter.";
    }
}
