namespace CNull.ErrorHandler.Events.Args
{
    /// <summary>
    /// Args for the event raised when an error occurred.
    /// </summary>
    /// <param name="message">Message to display for the user.</param>
    public class ErrorOccurredEventArgs(string message) : EventArgs
    {
        /// <summary>
        /// Message to display for the user.
        /// </summary>
        public string Message => message;
    }
}
