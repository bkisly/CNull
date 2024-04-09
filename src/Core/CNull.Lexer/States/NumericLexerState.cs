using CNull.Common.Configuration;
using CNull.Common.Extensions;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Compilation;
using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which integer or floating-point literal is built.
    /// </summary>
    public class NumericLexerState(ICodeSource source, IErrorHandler errorHandler, ICompilerConfiguration configuration) 
        : LexerState(source, errorHandler, configuration)
    {
        private long _integerPart;
        private long _fractionPart;

        private readonly int _maxFractionDigits = long.MaxValue.Length() - 1;

        public override bool TryBuildToken(out Token token)
        {
            if (!CurrentCharacter.IsAsciiDigit())
                return TokenFailed(out token, new InvalidTokenStartCharacter(TokenPosition), false);

            if (CurrentCharacter is '0')
            {
                Source.MoveToNext();
                if (CurrentCharacter.IsAsciiDigit())
                    return TokenFailed(out token, new PrefixedZeroError(TokenPosition));
            }
            else if (!TryBuildNumberPart(ref _integerPart, int.MaxValue, configuration.MaxTokenLength))
                return TokenFailed(out token, new NumericValueOverflowError(TokenPosition));

            if (CurrentCharacter is not '.')
            {
                token = new Token<int>((int)_integerPart, TokenType.IntegerLiteral, TokenPosition);
                return true;
            }

            Source.MoveToNext();

            if(!TryBuildNumberPart(ref _fractionPart, maxDigits: _maxFractionDigits))
                return TokenFailed(out token, new NumericValueOverflowError(TokenPosition));

            var fractionValue = _fractionPart / (decimal)Math.Pow(10, _fractionPart.Length());
            token = new Token<float>(_integerPart + (float)fractionValue, TokenType.FloatLiteral, TokenPosition);
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

                Source.MoveToNext();
            }

            return true;
        }
    }
}
