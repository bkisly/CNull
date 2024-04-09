using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    public class InvalidTokenLengthError(Position position, int expectedLength) : CompilationError(position)
    {
        public override string Message =>
            $"Invalid length of analyzed literal/identifier. Maximum accepted length: {expectedLength}";
    }
}
