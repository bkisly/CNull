using CNull.Common.Events;
using System.Text.RegularExpressions;

namespace CNull.Common.State
{
    public class StateManager : IStateManager
    {
        public const string DefaultModuleName = "Program";
        public const string DefaultSourcePath = $"<module {DefaultModuleName}>";

        private string? _currentWorkingDirectory;

        public string CurrentSourcePath { get; private set; } = DefaultSourcePath;
        public string CurrentModuleName => GetModuleName(CurrentSourcePath);

        public event EventHandler<InputRequestedEventArgs>? InputRequested;

        public void NotifyInputRequested(string path)
        {
            CurrentSourcePath = Path.GetFullPath(path);
            _currentWorkingDirectory = Path.GetDirectoryName(CurrentSourcePath);
            OnInputRequested(new Lazy<Stream>(() => new FileStream(path, FileMode.Open)), path);
        }

        public void NotifyInputRequested(Lazy<Stream> stream)
        {
            CurrentSourcePath = DefaultSourcePath;
            _currentWorkingDirectory = null;
            OnInputRequested(stream);
        }

        public bool TryOpenModule(string moduleName)
        {
            if (_currentWorkingDirectory == null)
                return false;

            var path = Path.Combine(_currentWorkingDirectory, $"{moduleName}.cnull");

            try
            {
                NotifyInputRequested(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetModuleName(string sourcePath)
        {
            if (sourcePath == DefaultSourcePath)
                return DefaultModuleName;

            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var cleanedString = Regex.Replace(fileName, "[^a-zA-Z0-9]", " ");
            var words = cleanedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var newWords = words.Select(w => char.ToUpper(w[0]) + w[1..].ToLower());
            return string.Join("", newWords);
        }

        private void OnInputRequested(Lazy<Stream> stream, string? path = null) 
            => InputRequested?.Invoke(this, new InputRequestedEventArgs(stream, path));
    }
}
