namespace CNull.Common.Events.Args
{
    public class FileInputRequestedEventArgs(string sourcePath) : EventArgs
    {
        public string SourcePath => sourcePath;
    }
}
