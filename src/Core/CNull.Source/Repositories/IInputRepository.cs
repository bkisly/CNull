namespace CNull.Source.Repositories
{
    /// <summary>
    /// Provides operations on stream readers for code input logic.
    /// </summary>
    public interface IInputRepository : IDisposable
    {
        /// <summary>
        /// Indicates whether StreamReader instance has been created.
        /// </summary>
        public bool IsInitialized { get; }

        /// <summary>
        /// Sets up the reader based on the given reader object.
        /// </summary>
        /// <param name="reader">The reader object.</param>
        public void SetupStream(TextReader reader);

        /// <summary>
        /// Instantiates a new <see cref="StreamReader"/> for reading from a file.
        /// </summary>
        /// <param name="path">Path to the file to read from.</param>
        public void SetupFileStream(string path);

        /// <summary>
        /// Reads from the stream reader and returns the result of this operation.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Thrown when StreamReader is not initialized.</exception>
        public int Read();

        /// <summary>
        /// Returns the next character from the stream without advancing it.
        /// </summary>
        /// <returns>The peeked character.</returns>
        public int Peek();
    }
}
