namespace CNull.Interpreter.Errors
{
    /// <summary>
    /// Contains all constant string values for built-in exceptions.
    /// </summary>
    public class RuntimeErrors
    {
        /// <summary>
        /// Thrown when null value was used in non-nullable context.
        /// </summary>
        public const string NullValueException = "NullValueException";

        /// <summary>
        /// Thrown when given index value is outside the bounds of the collection.
        /// </summary>
        public const string IndexOutOfRangeException = "IndexOutOfRangeException";

        /// <summary>
        /// Thrown after trying to add an item, which already exists in a collection.
        /// </summary>
        public const string ItemAlreadyAddedException = "ItemAlreadyAddedException";

        /// <summary>
        /// Thrown when trying to divide a number by zero.
        /// </summary>
        public const string DivisionByZeroException = "DivisionByZeroException";

        /// <summary>
        /// Thrown when input value is not in the correct format.
        /// </summary>
        public const string FormatException = "FormatException";
    }
}
