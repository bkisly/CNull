namespace CNull.Common.Events.Args
{
    public class InputSourceRequestedEventArgs(string sourcePath) : EventArgs
    {
        public string SourcePath => sourcePath;
    }
}
