namespace CNull.Parser.Productions
{
    /// <summary>
    /// Syntactic production which represents the whole program.
    /// </summary>
    public record Program(IEnumerable<ImportDirective> ImportDirectives,
        IEnumerable<FunctionDefinition> FunctionDefinitions);

    /// <summary>
    /// Syntactic production which represents an import directive.
    /// </summary>
    /// <param name="ModuleName">Name of the module to import from.</param>
    /// <param name="FunctionName">Function to import from the module.</param>
    public record ImportDirective(string ModuleName, string FunctionName);

    /// <summary>
    /// Syntactic production which represents a definition of the function.
    /// </summary>
    /// <param name="ReturnType">The return type of the function.</param>
    /// <param name="Name"></param>
    /// <param name="Parameters"></param>
    public record FunctionDefinition(ReturnType ReturnType, string Name, IEnumerable<Parameter> Parameters);
}
