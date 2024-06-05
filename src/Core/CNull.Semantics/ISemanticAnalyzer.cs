using CNull.Semantics.Symbols;

namespace CNull.Semantics
{
    /// <summary>
    /// Builds functions registry and performs semantic analysis of the code.
    /// </summary>
    public interface ISemanticAnalyzer
    {
        string RootModule { get; }
        FunctionsRegistry? Analyze(IStandardLibrary standardLibrary);
    }
}
