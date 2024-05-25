namespace CNull.Interpreter
{
    /// <summary>
    /// Callback method delegate for the standard input.
    /// </summary>
    /// <param name="header">Message to write before requesting input.</param>
    /// <returns>The read input.</returns>
    public delegate string? StandardInput(string header);

    /// <summary>
    /// Callback method delegate for the standard output.
    /// </summary>
    /// <param name="content">Content to write.</param>
    public delegate void StandardOutput(string content);

    /// <summary>
    /// Callback method delegate used for outputting errors.
    /// </summary>
    /// <param name="message">Error to display.</param>
    public delegate void StandardError(string message);

    /// <summary>
    /// Represents C? language interpreter.
    /// </summary>
    public interface IInterpreter
    {
        /// <summary>
        /// Executes the program.
        /// </summary>
        void Execute(StandardInput inputCallback, StandardOutput outputCallback);
    }
}
