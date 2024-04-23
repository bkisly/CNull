namespace CNull.ErrorHandler.Exceptions
{
    /// <summary>
    /// Indicates that error(s) which occurred during program execution made its continuation impossible.
    /// </summary>
    public class FatalErrorException(string message) : Exception(message);
}
