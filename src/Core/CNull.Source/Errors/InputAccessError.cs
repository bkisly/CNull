using CNull.ErrorHandler.Errors;

namespace CNull.Source.Errors
{
    /// <summary>
    /// Represents an error which occurrs when the source input could not be found or accessed.
    /// </summary>
    /// <param name="path">Path of the requested source.</param>
    public class InputAccessError(string path) : ISourceError
    {
        public string Message => $"Given file could not be found or accessed. Requested path: {SourcePath}";
        public string SourcePath => path;
    }
}
