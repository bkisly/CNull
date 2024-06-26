﻿using CNull.ErrorHandler;
using CNull.Semantics.Errors;
using CNull.Parser.Productions;

namespace CNull.Semantics.Symbols
{
    public record FunctionsRegistryEntry(IFunction FunctionDefinition, string? ExternalModuleName);

    public class FunctionsRegistry(IErrorHandler errorHandler)
    {
        private readonly Dictionary<(string ModuleName, string FunctionName), FunctionsRegistryEntry> _functionDefinitions = [];

        public FunctionsRegistryEntry this[string moduleName, string functionName] => _functionDefinitions[(moduleName, functionName)];

        public bool TryGetValue(string moduleName, string functionName, out FunctionsRegistryEntry entry)
        {
            return _functionDefinitions.TryGetValue((moduleName, functionName), out entry!);
        }

        public void Register(string moduleName, IFunction functionDefinition, string? externalModuleName = null)
        {
            try
            {
                _functionDefinitions.Add((moduleName, functionDefinition.Name),
                    new FunctionsRegistryEntry(functionDefinition, externalModuleName));
            }
            catch (ArgumentException)
            {
                throw errorHandler.RaiseSemanticError(new FunctionRedefinitionError(moduleName, functionDefinition.Name));
            }
        }

        public IFunction? GetEntryPoint(string rootModule)
        {
            return _functionDefinitions.TryGetValue((rootModule, "Main"), out var functionDefinition)
                ? functionDefinition.FunctionDefinition
                : null;
        }

        public bool ContainsModule(string moduleName) => _functionDefinitions.Keys.Any(s => s.ModuleName == moduleName);
    }
}
