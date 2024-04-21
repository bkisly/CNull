namespace CNull.Interpreter
{
    /// <summary>
    /// Represents C? language interpreter.
    /// </summary>
    public interface IInterpreter
    {
        /// <summary>
        /// Executes the program.
        /// </summary>
        void Execute(Func<string, string> inputCallback, Action<string> outputCallback);
    }
}
