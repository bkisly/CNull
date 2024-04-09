namespace CNull.Common.Extensions
{
    public static class NumericsExtensions
    {
        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static int Length(this int i) => i.ToString().Length;

        /// <summary>
        /// Returns the amount of decimal digits in the number.
        /// </summary>
        public static int Length(this long l) => l.ToString().Length;
    }
}
