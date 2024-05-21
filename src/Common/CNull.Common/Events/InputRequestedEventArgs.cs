namespace CNull.Common.Events
{
    public class InputRequestedEventArgs(Lazy<TextReader> reader, string path) : EventArgs
    {
        public Lazy<TextReader> Reader => reader;
        public string Path => path;
    }
}
