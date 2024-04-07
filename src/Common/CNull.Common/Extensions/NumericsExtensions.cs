namespace CNull.Common.Extensions
{
    public static class PrimitivesExtensions
    {
        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static int Length(this int i) => i.ToString().Length;

        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static long Length(this long l) => l.ToString().Length;

        /// <summary>
        /// Checks if the value is recognized as an ASCII digit.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsAsciiDigit(this char? c) => c.HasValue && char.IsAsciiDigit(c.Value);
    }
}
