using CNull.Common.Extensions;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which integer or floating-point literal is built.
    /// </summary>
    public class NumericLexerState(ICodeSource source) : LexerState(source)
    {
        private long _integerPart;
        private long _fractionPart;

        private const int MaxFractionDigits = 18;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.IsAsciiDigit())
                return TokenFailed(out token);

            if (CurrentCharacter is '0')
            {
                Source.MoveToNext();
                if (CurrentCharacter.IsAsciiDigit())
                    return TokenFailed(out token);
            }
            else if (!TryBuildNumberPart(ref _integerPart, int.MaxValue))
                return TokenFailed(out token);

            if (CurrentCharacter is not '.')
            {
                token = new Token<int>((int)_integerPart, TokenType.IntegerLiteral);
                return true;
            }

            Source.MoveToNext();

            if(!TryBuildNumberPart(ref _fractionPart, maxDigits: MaxFractionDigits))
                return TokenFailed(out token);

            var fractionValue = _fractionPart / (decimal)Math.Pow(10, _fractionPart.Length());
            token = new Token<float>(_integerPart + (float)fractionValue, TokenType.FloatLiteral);
            return true;
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

                source.MoveToNext();
            }

            return true;
        }
    }
}
