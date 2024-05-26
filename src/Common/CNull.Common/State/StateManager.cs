using CNull.Common.Events;
using System.Text.RegularExpressions;

namespace CNull.Common.State
{
    /// <summary>
    /// <inheritdoc cref="IStateManager"/>
    /// </summary>
    public class StateManager : IStateManager
    {
        private string _currentSourcePath = "<unknown>";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string CurrentSourcePath
        {
            get => _currentSourcePath;
            private set
            {
                _currentSourcePath = value;
                CurrentModuleName = GetModuleName(value);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string CurrentModuleName { get; private set; } = "Program";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<InputRequestedEventArgs>? InputRequested;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reader"><inheritdoc/></param>
        /// <param name="path"><inheritdoc/></param>
        public void NotifyInputRequested(Lazy<TextReader> reader, string path)
        {
            CurrentSourcePath = Path.GetFullPath(path);
            InputRequested?.Invoke(this, new InputRequestedEventArgs(reader, path));
        }

        private static string GetModuleName(string sourcePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            var cleanedString = Regex.Replace(fileName, "[^a-zA-Z0-9]", " ");
            var words = cleanedString.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var newWords = words.Select(w => char.ToUpper(w[0]) + w[1..].ToLower());
            return string.Join("", newWords);
        }
    }
}
