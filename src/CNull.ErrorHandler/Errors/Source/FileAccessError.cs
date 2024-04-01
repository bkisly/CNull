namespace CNull.ErrorHandler.Errors.Source
{
    /// <summary>
    /// Represents an error which occurrs when the source file could not be found or accessed.
    /// </summary>
    /// <param name="filePath">Path of the requested file.</param>
    public class FileAccessError(string filePath) : ISourceError
    {
        public string Message => $"Given file could not be found or accessed. Requested path: {FilePath}";
        public string FilePath => Path.GetFullPath(filePath);
    }
}
