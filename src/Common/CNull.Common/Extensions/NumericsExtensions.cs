namespace CNull.Common.Extensions
{
    public static class NumericsExtensions
    {
        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static int Length(this int i) => i == 0 ? 1 : (int)Math.Log10(Math.Abs(i)) + 1;

        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static long Length(this long l) => l == 0 ? 1 : (long)Math.Log10(Math.Abs(l)) + 1;
    }
}
