namespace CNull.ErrorHandler.Errors.Source
{
    /// <summary>
    /// Error raised when input stream was not initialized.
    /// </summary>
    public class StreamNotInitializedError : ISourceError
    {
        public string Message => "Input stream is not initialized for the selected source.";
    }
}
