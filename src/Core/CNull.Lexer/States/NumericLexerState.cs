using CNull.Common.Extensions;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Lexer.ServicesContainers;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which integer or floating-point literal is built.
    /// </summary>
    public class NumericLexerState(ILexerStateServicesContainer servicesContainer) : LexerState(servicesContainer)
    {
        private long _integerPart;
        private long _fractionPart;

        private readonly int _maxFractionDigits = long.MaxValue.Length() - 1;

        public override Token BuildToken()
        {
            if (!CurrentCharacter.IsAsciiDigit())
                return TokenFailed(new InvalidTokenStartCharacter(TokenPosition), false);

            if (CurrentCharacter is '0')
            {
                Source.MoveToNext();
                if (CurrentCharacter.IsAsciiDigit())
                    return TokenFailed(new PrefixedZeroError(TokenPosition));
            }
            else if (!TryBuildNumberPart(ref _integerPart, int.MaxValue, Configuration.MaxTokenLength))
                return TokenFailed(new NumericValueOverflowError(TokenPosition));

            if (CurrentCharacter is not '.')
                return new Token<int>((int)_integerPart, TokenType.IntegerLiteral, TokenPosition);

            Source.MoveToNext();

            if(!TryBuildNumberPart(ref _fractionPart, maxDigits: _maxFractionDigits))
                return TokenFailed(new NumericValueOverflowError(TokenPosition));

            var fractionValue = _fractionPart / (decimal)Math.Pow(10, _fractionPart.Length());
            return new Token<float>(_integerPart + (float)fractionValue, TokenType.FloatLiteral, TokenPosition);
        }

        private bool TryBuildNumberPart(ref long partValue, long maxValue = long.MaxValue, int maxDigits = int.MaxValue)
        {
            var digits = 0;

            while (CurrentCharacter.IsAsciiDigit())
            {
                var digitValue = CurrentCharacter!.Value - '0';
                if (digits > maxDigits || partValue > (maxValue - digitValue) / 10)
                    return false;

                partValue = partValue * 10 + digitValue;
                digits++;

                Source.MoveToNext();
            }

            return true;
        }
    }
}
