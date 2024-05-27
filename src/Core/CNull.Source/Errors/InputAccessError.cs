using CNull.ErrorHandler.Errors;

namespace CNull.Source.Errors
{
    /// <summary>
    /// Represents an error which occurrs when the source input could not be found or accessed.
    /// </summary>
    public record InputAccessError : ISourceError
    {
        public string Message { get; }

        public InputAccessError(IOException exception)
        {
            Message = $"Given stream could not be opened or accessed. Inner error message: {exception.Message}";
        }

        public InputAccessError(string path)
        {
            Message = $"Given file could not be opened or accessed. Requested path: {Path.GetFullPath(path)}";
        }
    }
}
