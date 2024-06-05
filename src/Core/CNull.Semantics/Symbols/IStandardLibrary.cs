using CNull.Parser.Productions;

namespace CNull.Semantics.Symbols
{
    public record StandardLibraryHeader(string SubmoduleName, string FunctionName);

    public interface IStandardLibrary
    {
        Dictionary<StandardLibraryHeader, StandardLibraryFunction> StandardLibraryFunctions { get; }
    }
}
