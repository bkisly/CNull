namespace CNull.Common.Extensions
{
    public static class CharExtensions
    {
        /// <summary>
        /// Checks if the value is recognized as an ASCII digit.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsAsciiDigit(this char? c) => c.HasValue && char.IsAsciiDigit(c.Value);

        /// <summary>
        /// Checks if the value is recognized as a white space character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsWhiteSpace(this char? c) => c.HasValue && char.IsWhiteSpace(c.Value);
    }
}
