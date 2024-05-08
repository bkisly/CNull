using CNull.Common;
using CNull.Parser.Visitors;

namespace CNull.Parser.Productions
{
    /// <summary>
    /// Represents a general type of a syntactic production.
    /// </summary>
    public interface ISyntacticProduction : IAstVisitable
    {
        /// <summary>
        /// Position in text of a production.
        /// </summary>
        Position Position { get; }
    }

    /// <summary>
    /// Syntactic production which represents the whole program.
    /// </summary>
    public record Program(IEnumerable<ImportDirective> ImportDirectives,
        IEnumerable<FunctionDefinition> FunctionDefinitions) : IAstVisitable
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents an import directive.
    /// </summary>
    /// <param name="ModuleName">Name of the module to import from.</param>
    /// <param name="FunctionName">Function to import from the module.</param>
    public record ImportDirective(string ModuleName, string FunctionName, Position Position) : ISyntacticProduction
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production which represents a definition of the function.
    /// </summary>
    /// <param name="ReturnType">The return type of the function.</param>
    /// <param name="Name">The name of the function.</param>
    /// <param name="Parameters">Parameters list.</param>
    /// <param name="FunctionBody">Block statement, which is the body of the function.</param>
    public record FunctionDefinition(ReturnType ReturnType, string Name, IEnumerable<Parameter> Parameters,
        BlockStatement FunctionBody, Position Position) : ISyntacticProduction
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Syntactic production that represents a parameter.
    /// </summary>
    /// <param name="Type">Type of the parameter.</param>
    /// <param name="Name">Name of the parameter.</param>
    public record Parameter(IDeclarableType Type, string Name, Position Position) : ISyntacticProduction
    {
        public void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
