namespace CNull.Parser.Productions
{
    /// <summary>
    /// Syntactic production that represents a parameter.
    /// </summary>
    /// <param name="Type">Type of the parameter.</param>
    /// <param name="Name">Name of the parameter.</param>
    public record Parameter(IDeclarableType Type, string Name);
}
