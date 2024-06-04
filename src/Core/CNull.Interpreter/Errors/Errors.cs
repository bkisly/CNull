﻿using CNull.Common;
using CNull.ErrorHandler.Errors;

namespace CNull.Interpreter.Errors
{
    public record CircularDependencyError(Position Position) : ICompilationError
    {
        public string Message => "Circular dependency detected.";
    }

    public record FunctionRedefinitionError(string FunctionName) : ICompilationError
    {
        public Position Position => Position.FirstCharacter;
        public string Message => $"Function with name: {FunctionName} has already been defined.";
    }

    public record FunctionNotFoundError(string FunctionName, string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"The function: {FunctionName} was not found in module: {ModuleName}.";
    }

    public record ModuleNotFoundError(string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"Module: {ModuleName} was not found in the working directory.";
    }

    public record MissingSubmoduleError(Position Position) : ICompilationError
    {
        public string Message => $"Missing submodule specification in standard library function import.";
    }

    public record ModuleCompilationError(string ModuleName, Position Position) : ICompilationError
    {
        public string Message => $"Compilation of module: {ModuleName} has finished with errors.";
    }

    public record MissingEntryPointError(string ModuleName) : IRuntimeError
    {
        public int LineNumber => 0;
        public string Message => $"Entry point ('Main' function) not found in root module: {ModuleName}.";
    }
}
