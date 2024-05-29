namespace CNull.Interpreter.Symbols
{
    /// <summary>
    /// Represents a component capable of building function registry.
    /// </summary>
    public interface IFunctionsRegistryBuilder
    {
        /// <summary>
        /// Name of the module, which contains the entry point.
        /// </summary>
        string RootModule { get; }

        /// <summary>
        /// Builds the functions registry for the specified program.
        /// </summary>
        /// <returns><see langword="null"/> if building failed, otherwise the registry.</returns>
        FunctionsRegistry? Build();
    }
}
