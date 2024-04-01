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
        /// Instantiates a new <see cref="StreamReader"/> based on a given stream object.
        /// </summary>
        /// <param name="stream"></param>
        public void Setup(Stream stream);

        /// <summary>
        /// Reads from the stream reader and returns the result of this operation.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Thrown when StreamReader is not initialized.</exception>
        public int Read();
    }
}
