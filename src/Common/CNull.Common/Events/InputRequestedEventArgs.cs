namespace CNull.Common.Events
{
    public class InputRequestedEventArgs(Lazy<Stream> stream, string? sourcePath = null) : EventArgs
    {
        public Lazy<Stream> Stream => stream;
        public string? SourcePath => sourcePath;
    }
}
